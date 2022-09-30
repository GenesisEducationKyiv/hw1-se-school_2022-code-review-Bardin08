using Integrations.Notifications.Contracts.Models.Emails;

namespace Integrations.Notifications.Contracts.Abstractions.Emails;

public interface IBaseEmailProvider
{
    Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotificationDto> notifications);
}