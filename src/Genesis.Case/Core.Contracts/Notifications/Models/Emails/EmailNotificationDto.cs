namespace Core.Contracts.Notifications.Models.Emails;

public class EmailNotificationDto
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}