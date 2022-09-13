using Core.Notifications.Emails.Models;

namespace Core.Abstractions;

public interface ISubscriptionService
{
    Task<bool> SubscribeAsync(string email);
    Task<SendEmailNotificationsResult> NotifyAsync();
}