using Core.Contracts.Models;
using Core.Models.Notifications;

namespace Core.Abstractions;

public interface IEmailService
{
    Task<SendEmailNotificationsResponse> SendEmailsAsync(IEnumerable<EmailNotification> notificationDtos);
}
