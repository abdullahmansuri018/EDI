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
        private async Task UpdateContainerInCosmosDb(string containerId)
        {
            try
            {
                _logger.LogInformation($"Attempting to find container in Cosmos DB with ContainerId: {containerId}");

                var cosmosItem = await _cosmosContainer
                    .GetItemLinqQueryable<CosmosContainer>()
                    .Where(item => item.ContainerId == containerId)
                    .FirstOrDefaultAsync();

                if (cosmosItem != null)
                {
                    _logger.LogInformation($"Found container with ID: {containerId}. Updating 'Holds' field.");

                    cosmosItem.Holds = true;
                    string partitionKey = containerId;

                    // Replace the item in Cosmos DB
                    await _cosmosContainer.ReplaceItemAsync(cosmosItem, cosmosItem.Id.ToString(), new PartitionKey(partitionKey));
                    _logger.LogInformation($"Container ID {containerId} updated with Holds = true.");
                }
                else
                {
                    _logger.LogWarning($"Container with ID {containerId} not found in Cosmos DB.");
                }
            }
            catch (CosmosException cosmosEx)
            {
                _logger.LogError($"Cosmos DB error: {cosmosEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Cosmos DB: {ex.Message}");
            }
        }
    }
}
