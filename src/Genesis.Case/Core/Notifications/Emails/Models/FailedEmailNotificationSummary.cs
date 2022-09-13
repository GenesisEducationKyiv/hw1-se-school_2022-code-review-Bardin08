namespace Core.Notifications.Emails.Models;

public class FailedEmailNotificationSummary
{
    public string EmailAddress { get; set; } = null!;
    public string? Error { get; set; }
}