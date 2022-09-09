namespace Core.Models.Notifications.Emails;

public class SendEmailNotificationsResult
{
    public int TotalSubscribers { get; set; }
    public int SuccessfullyNotified { get; set; }
    public List<FailedEmailNotificationSummary>? Failed { get; set; }
}