using Core.Models.Notifications.Emails;

namespace Core.Abstractions;

public interface ISubscriptionService
{
    Task<bool> SubscribeAsync(string email);
    Task<SendEmailNotificationsResult> NotifyAsync();
}