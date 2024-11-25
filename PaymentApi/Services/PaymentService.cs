using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace PaymentApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbContext;
        private readonly ServiceBusClient _serviceBusClient;  // Service Bus Client
        private readonly ServiceBusSender _serviceBusSender;  // Sender for the Topic
        private readonly Container _cosmosContainer;
        private readonly ServiceBusReceiver _serviceBusReceiver;  // Receiver for Service Bus messages
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(AppDbContext dbContext, IConfiguration configuration, CosmosClient cosmosClient, ILogger<PaymentService> logger)
        {
            _dbContext = dbContext;
            _cosmosContainer = cosmosClient.GetContainer(configuration["CosmosDb:DatabaseName"], configuration["CosmosDb:ContainerName"]);
            _serviceBusClient = new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]);
            _serviceBusSender = _serviceBusClient.CreateSender(configuration["AzureServiceBus:TopicName"]);
            _serviceBusReceiver = _serviceBusClient.CreateReceiver(configuration["AzureServiceBus:TopicName"], configuration["AzureServiceBus:SubscriptionName"]);
            _logger = logger;
        }

        // Push data to Azure Service Bus
        public async Task PushDataToServiceBusAsync(string containerId)
        {
            var messageBody = new { ContainerId = containerId };
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody)))
            {
                ContentType = "application/json"
            };

            try
            {
                await _serviceBusSender.SendMessageAsync(message);
                _logger.LogInformation($"Message with ContainerId {containerId} sent to Service Bus.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message to Service Bus: {ex.Message}");
                throw;
            }
        }

        // Process message from Service Bus and update SQL and Cosmos DB
        public async Task ProcessMessageFromServiceBusAsync(int userId)
        {
            try
            {
                // Receive a single message from Service Bus
                ServiceBusReceivedMessage receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();

                if (receivedMessage != null)
                {
                    // Deserialize the message
                    string messageBody = receivedMessage.Body.ToString();
                    var messageData = JsonConvert.DeserializeObject<dynamic>(messageBody);
                    string containerId = messageData?.ContainerId;

                    if (string.IsNullOrEmpty(containerId))
                    {
                        _logger.LogWarning("Received message with missing or invalid ContainerId.");
                        // Abandon the message to not remove it from the Service Bus
                        await _serviceBusReceiver.AbandonMessageAsync(receivedMessage);
                        return;
                    }

                    _logger.LogInformation($"Processing message for ContainerId: {containerId}");

                    // Step 1: Update SQL (mark the container as paid)
                    var result = await MarkAsPaidAndNotifyAsync(userId, containerId);
                    if (!result)
                    {
                        _logger.LogWarning($"Failed to mark container {containerId} as paid.");
                        // Abandon the message if processing failed
                        await _serviceBusReceiver.AbandonMessageAsync(receivedMessage);
                        return;
                    }

                    // Step 2: Update Cosmos DB (update the 'Holds' field)
                    await UpdateContainerInCosmosDb(containerId);

                    // After successful processing, complete the message to acknowledge and remove it from Service Bus
                    await _serviceBusReceiver.CompleteMessageAsync(receivedMessage);
                    _logger.LogInformation($"Message for ContainerId {containerId} processed and removed from Service Bus.");
                }
                else
                {
                    _logger.LogInformation("No messages received from Service Bus.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing Service Bus message: {ex.Message}");
            }
        }

        // Mark the container as paid and send a notification via Service Bus (already implemented earlier)
        public async Task<bool> MarkAsPaidAndNotifyAsync(int userId, string containerId)
        {
            // Query the database to get the UserContainerData for the specified user and containerId
            var userContainerData = await _dbContext.UserContainerData
                .Where(x => x.UserId == Convert.ToString(userId) && x.ContainerId == containerId)
                .FirstOrDefaultAsync(); // Fetch the first matching result (or null if not found)

            // If no matching user container data is found, log a warning and return false
            if (userContainerData == null)
            {
                _logger.LogWarning($"User with ID {userId} and Container ID {containerId} not found.");
                return false;
            }

            // Mark the container as paid by setting the 'IsPaid' field to true
            userContainerData.IsPaid = true;

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            // Prepare a message to send to the Service Bus (JSON format with the containerId)
            var messageBody = new { ContainerId = containerId };
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody)))
            {
                ContentType = "application/json"
            };

            // Send the message to the Service Bus
            await _serviceBusSender.SendMessageAsync(message);

            // Log that the message was successfully sent
            _logger.LogInformation($"Message sent to Service Bus for Container ID: {containerId}");

            return true; // Return true indicating success
        }


        // Update the 'Holds' field in Cosmos DB (already implemented earlier)
        public async Task UpdateContainerInCosmosDb(string containerId)
        {
            try
            {
                if (string.IsNullOrEmpty(containerId))
                {
                    _logger.LogError("The containerId parameter is null or empty, which is invalid.");
                    return;
                }

                _logger.LogInformation($"Attempting to update container with ID: {containerId}");

                var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.ContainerId = @containerId")
                    .WithParameter("@containerId", containerId);

                var queryIterator = _cosmosContainer.GetItemQueryIterator<CosmosContainer>(
                    queryDefinition,
                    requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(containerId) });

                var response = await queryIterator.ReadNextAsync();

                if (response.Count == 0)
                {
                    _logger.LogWarning($"No container found with ID: {containerId}. Please verify the input.");
                    return;
                }

                var cosmosItem = response.FirstOrDefault();
                if (cosmosItem != null)
                {
                    _logger.LogInformation($"Document retrieved with ID: {containerId}. Preparing to update.");

                    cosmosItem.Holds = true;

                    await _cosmosContainer.UpsertItemAsync(cosmosItem, new PartitionKey(containerId));

                    _logger.LogInformation($"Container with ID: {containerId} successfully updated in Cosmos DB.");
                }
                else
                {
                    _logger.LogWarning($"Container with ID: {containerId} was not found in Cosmos DB.");
                }
            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError($"Cosmos DB error: {cosmosEx.Message}, Stack Trace: {cosmosEx.StackTrace}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the container in Cosmos DB: {ex.Message}, Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
