namespace Core.Contracts.Models;

public class FailedEmailNotificationSummary
{
    public string EmailAddress { get; set; } = null!;
    public string? Error { get; set; }
}