namespace PaymentApi.Models;
public class Payment
{
    public int Id{ get; set; }
    public string UserId { get; set; }
    public string ContainerId { get; set; }
    public string TransactionId{get; set; }
    public DateTime dateTime { get; set; }
}