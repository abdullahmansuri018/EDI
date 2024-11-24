using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using PaymentApi.Models;

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

        // Mark the container as paid and send a notification via Service Bus
        public async Task<bool> MarkAsPaidAndNotifyAsync(int userId, string containerId)
        {
            var userContainerData = await _dbContext.UserContainerData
                .Where(x => x.UserId == Convert.ToString(userId) && x.ContainerId == containerId)
                .FirstOrDefaultAsync();

            if (userContainerData == null)
            {
                _logger.LogWarning($"User with ID {userId} and Container ID {containerId} not found.");
                return false;
            }

            userContainerData.IsPaid = true;
            await _dbContext.SaveChangesAsync();

            var messageBody = new { ContainerId = containerId };
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody)))
            {
                ContentType = "application/json"
            };

            await _serviceBusSender.SendMessageAsync(message);
            _logger.LogInformation($"Message sent to Service Bus for Container ID: {containerId}");
            return true;
        }

        // Receive and process messages from Service Bus
        public async Task ReceiveAndProcessMessageAsync()
        {
            ServiceBusReceivedMessage receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();

            if (receivedMessage != null)
            {
                try
                {
                    string messageBody = receivedMessage.Body.ToString();
                    var messageData = JsonConvert.DeserializeObject<dynamic>(messageBody);
                    string containerId = messageData?.ContainerId;

                    if (!string.IsNullOrEmpty(containerId))
                    {
                        _logger.LogInformation($"Processing message for Container ID: {containerId}");
                        await UpdateContainerInCosmosDb(containerId);
                        await _serviceBusReceiver.CompleteMessageAsync(receivedMessage);
                    }
                    else
                    {
                        _logger.LogWarning("Received message with missing or invalid ContainerId.");
                        await _serviceBusReceiver.AbandonMessageAsync(receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing Service Bus message: {ex.Message}");
                    await _serviceBusReceiver.AbandonMessageAsync(receivedMessage);
                }
            }
            else
            {
                _logger.LogInformation("No messages received from Service Bus.");
            }
        }

        // Update the 'Holds' field in Cosmos DB
        public async Task UpdateContainerInCosmosDb(string containerId)
        {
            try
            {
                // Log containerId before querying to ensure it's valid
                if (string.IsNullOrEmpty(containerId))
                {
                    _logger.LogError("The containerId parameter is null or empty, which is invalid.");
                    return;
                }

                _logger.LogInformation($"Attempting to update container with ID: {containerId}");

                // Build the query to search for the container by its ID (ensure partition key is correct)
                var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.ContainerId = @containerId")
                    .WithParameter("@containerId", containerId);

                // Log the exact query being executed
                _logger.LogInformation($"Executing query: SELECT * FROM c WHERE c.ContainerId = '{containerId}'");

                // Execute the query (ensure the partition key is used)
                var queryIterator = _cosmosContainer.GetItemQueryIterator<CosmosContainer>(
                    queryDefinition,
                    requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(containerId) });

                var response = await queryIterator.ReadNextAsync();

                if (response.Count == 0)
                {
                    _logger.LogWarning($"No container found with ID: {containerId}. Please verify the input.");
                    return;
                }

                // Log the document retrieved (if any)
                var cosmosItem = response.FirstOrDefault();
                if (cosmosItem != null)
                {
                    _logger.LogInformation($"Document retrieved with ID: {containerId}. Preparing to update.");

                    // Log current 'Holds' value before updating
                    _logger.LogInformation($"Current Holds value for Container ID: {containerId} is {cosmosItem.Holds}");

                    // Update the container's 'Holds' field
                    cosmosItem.Holds = true; // Update to false, based on your previous request

                    // Log the updated 'Holds' value
                    _logger.LogInformation($"Updating container with ID: {containerId}, setting Holds to true.");

                    // Replace the document with the updated data
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
