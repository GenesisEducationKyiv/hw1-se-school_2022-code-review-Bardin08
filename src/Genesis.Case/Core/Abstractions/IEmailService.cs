using Core.Notifications.Emails.Models;

namespace Core.Abstractions;

public interface IEmailService
{
    Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications);
}