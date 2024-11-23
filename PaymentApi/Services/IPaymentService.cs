namespace PaymentApi.Services
{
    public interface IPaymentService
    {
        Task<bool> MarkAsPaidAndNotifyAsync(int userId, string containerId);

        Task ReceiveAndProcessMessageAsync();
    }
}