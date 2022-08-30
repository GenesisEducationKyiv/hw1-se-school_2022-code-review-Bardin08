using Core.Models;

namespace Core.Abstractions;

public interface ISubscriptionService
{
    Task<bool> SubscribeAsync(string email);
    Task<SubscriptionNotifyResult> NotifyAsync();
}