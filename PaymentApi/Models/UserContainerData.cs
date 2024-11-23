public class UserContainerData
{
    public int Id { get; set; }  // This will be the primary key
    public string UserId { get; set; }
    public string Email { get; set; }
    public string ContainerId { get; set; }
    public bool IsPaid { get; set; }
}