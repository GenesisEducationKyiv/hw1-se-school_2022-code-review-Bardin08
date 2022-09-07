namespace Api.Models.Responses;

/// <summary>
/// An /sendEmails endpoint API response model
/// </summary>
public class SendEmailsResponse
{
    /// <summary>
    /// Total amount of emails in the storage
    /// </summary>
    public int TotalSubscribers { get; set; }
    
    /// <summary>
    /// The amount of successfully sent emails
    /// </summary>
    public int SuccessfullyNotified { get; set; }
    
    /// <summary>
    /// Emails that were not sent
    /// </summary>
    public List<string>? Failed { get; set; }
}