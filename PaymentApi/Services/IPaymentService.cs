using System.Threading.Tasks;

namespace PaymentApi.Services
{
    public interface IPaymentService
    {
        // Push data to Service Bus with containerId
        Task PushDataToServiceBusAsync(string containerId);

        // Process message from Service Bus, update SQL & Cosmos DB, and complete the message
        Task ProcessMessageFromServiceBusAsync(int userId);

        // Mark the container as paid, notify via Service Bus, and save the payment to the database
        Task<bool> CreatePaymentTableSql(int userId, string containerId);
    }
}
