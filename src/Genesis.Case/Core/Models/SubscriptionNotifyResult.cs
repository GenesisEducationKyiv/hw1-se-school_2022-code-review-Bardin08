namespace Core.Models;

public class SubscriptionNotifyResult
{
    public int TotalSubscribers { get; set; }
    public int SuccessfullyNotified { get; set; }
    public List<string>? Failed { get; set; }
}