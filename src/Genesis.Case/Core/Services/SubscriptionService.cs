using Core.Abstractions;
using Core.Contracts.Abstractions;
using Core.Contracts.Models;
using Core.Models.Notifications;
using Data.Providers;

namespace Core.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IEmailService _emailService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly IJsonEmailsStorage _emailsStorage;

    public SubscriptionService(IEmailService emailService, IExchangeRateService exchangeRateService,
        IJsonEmailsStorage emailsStorage)
    {
        _emailService = emailService;
        _exchangeRateService = exchangeRateService;
        _emailsStorage = emailsStorage;
    }

    public async Task<bool> SubscribeAsync(string email)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var isAlreadyExists = (await _emailsStorage.ReadAsync(email)) != null;

        if (isAlreadyExists)
        {
            return false;
        }

        await _emailsStorage.CreateAsync(email);
        return true;
    }

    public async Task<SendEmailNotificationsResponse> NotifyAsync()
    {
        var emailAddresses = (await _emailsStorage.ReadAllAsync()).ToList();
        var currentExchangeRate = await _exchangeRateService.GetBtcToUahExchangeRateAsync();

        var notifications = emailAddresses.Select(email => new EmailNotification()
        {
            To = email,
            Subject = "BTC to UAH exchange rate",
            Message = $"Hello, {email}!\n\nWe have a new BTC to UAH exchange rate for you! It is {currentExchangeRate} now!"
        }).ToList();

        return await _emailService.SendEmailsAsync(notifications);
    }
}