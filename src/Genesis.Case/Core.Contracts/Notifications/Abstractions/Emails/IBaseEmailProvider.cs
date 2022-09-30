using Core.Contracts.Notifications.Models.Emails;

namespace Core.Contracts.Notifications.Abstractions.Emails;

public interface IBaseEmailProvider
{
    Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotificationDto> notifications);
}