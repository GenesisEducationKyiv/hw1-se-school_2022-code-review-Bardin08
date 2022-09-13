using Core.Notifications.Emails.Models;

namespace Core.Notifications.Emails.Providers.Abstractions;

public interface IBaseEmailProvider
{
    Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications);

}