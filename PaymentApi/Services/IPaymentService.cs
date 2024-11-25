namespace PaymentApi.Services
{
    public interface IPaymentService
    {
        Task PushDataToServiceBusAsync(string containerId);  // Push data to Service Bus
        Task ProcessMessageFromServiceBusAsync(int userId);  // Process message from Service Bus and update SQL & Cosmos DB
    }
}
