namespace Api.Models.Responses;

/// <summary>
/// Represents an email address and an error why notification was not sent to it.
/// </summary>
public class FailedEmailNotificationSummaryResponse
{
    /// <summary>
    /// The address of the subscriber to whom the notification was not sent.
    /// </summary>
    public string EmailAddress { get; set; } = null!;
    /// <summary>
    /// Error that was occured while sending the notification.
    /// </summary>
    public string? Error { get; set; }
}