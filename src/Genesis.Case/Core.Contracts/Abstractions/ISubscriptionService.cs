using Core.Contracts.Models;

namespace Core.Contracts.Abstractions;

public interface ISubscriptionService
{
    Task<bool> SubscribeAsync(string email);
    Task<SendEmailNotificationsResponse> NotifyAsync();
}