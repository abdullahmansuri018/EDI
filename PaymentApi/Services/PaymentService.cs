using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using PaymentApi.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbContext;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _serviceBusSender;
        private readonly ServiceBusReceiver _serviceBusReceiver;
        private readonly Container _cosmosContainer;
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
            var messageBody = JsonConvert.SerializeObject(new { ContainerId = containerId });
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody)) { ContentType = "application/json" };

            try
            {
                _logger.LogInformation($"Sending message to Service Bus: {messageBody}");
                await _serviceBusSender.SendMessageAsync(message);
                _logger.LogInformation("Message sent to Service Bus.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message: {ex.Message}");
            }
        }

        // Process message from Service Bus and update SQL and Cosmos DB
        public async Task ProcessMessageFromServiceBusAsync(int userId)
        {
            try
            {
                var receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();
                System.Console.WriteLine(receivedMessage);
                _logger.LogInformation("received ");

                // Lock duration of 60 seconds
                if (receivedMessage == null)
                {
                    _logger.LogWarning("No message received from Service Bus.");
                    return;
                }
                var messageBody = Encoding.UTF8.GetString(receivedMessage.Body);
                var messageData = JsonConvert.DeserializeObject<dynamic>(messageBody);
                var containerId = messageData?.ContainerId?.ToString();

                if (string.IsNullOrEmpty(containerId))
                {
                    _logger.LogWarning("Invalid message, missing ContainerId. Abandoning message.");
                    await _serviceBusReceiver.AbandonMessageAsync(receivedMessage);  // Abandon message for retry
                    return;
                }

                _logger.LogInformation($"Processing message for ContainerId: {containerId}");

                // Process payment and notify
                await MarkAsPaidAndNotifyAsync(userId, containerId);
                await UpdateContainerInCosmosDb(containerId);
                await _serviceBusReceiver.CompleteMessageAsync(receivedMessage);
                System.Console.WriteLine(receivedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // Mark container as paid and send a notification to Service Bus
        public async Task<bool> MarkAsPaidAndNotifyAsync(int userId, string containerId)
        {
            var userContainerData = _dbContext.UserContainerData
                .FirstOrDefault(x => x.UserId == userId.ToString() && x.ContainerId == containerId);

            if (userContainerData == null)
                return false;

            var payment = new Payment
            {
                UserId = userContainerData.UserId,
                ContainerId = containerId,
                TransactionId = Guid.NewGuid().ToString(),
                dateTime = DateTime.UtcNow
            };

            _dbContext.paymentData.Add(payment);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        // Update Cosmos DB with new information
        public async Task UpdateContainerInCosmosDb(string containerId)
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c WHERE c.ContainerId = @containerId").WithParameter("@containerId", containerId);
                var response = await _cosmosContainer.GetItemQueryIterator<CosmosContainer>(query).ReadNextAsync();

                var cosmosItem = response.FirstOrDefault();
                if (cosmosItem != null)
                {
                    cosmosItem.Holds = false;
                    cosmosItem.Fees = 0;
                    await _cosmosContainer.UpsertItemAsync(cosmosItem, new PartitionKey(containerId));
                    _logger.LogInformation($"Updated container {containerId} in Cosmos DB.");
                }
                else
                {
                    _logger.LogWarning($"Container {containerId} not found in Cosmos DB.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Cosmos DB: {ex.Message}");
            }
        }
    }
}
