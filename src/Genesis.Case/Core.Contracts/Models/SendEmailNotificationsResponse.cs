namespace Core.Contracts.Models;

public class SendEmailNotificationsResponse
{
    public int TotalSubscribers { get; set; }
    public int SuccessfullyNotified { get; set; }
    public List<FailedEmailNotificationSummary>? Failed { get; set; }
}