namespace PaymentApi.Services
{
    public interface IPaymentService
    {
        Task<bool> MarkAsPaidAndNotifyAsync(int userId, string containerId);
        Task ReceiveAndProcessMessageAsync();
        Task UpdateContainerInCosmosDb(string containerId);  // Add this method
    }

}