using Core.Abstractions;
using Core.Models;
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

    public async Task<SendEmailNotificationsResult> NotifyAsync()
    {
        var result = new SendEmailNotificationsResult();
        var emailAddresses = (await _emailsStorage.ReadAllAsync()).ToList();
        result.TotalSubscribers = emailAddresses.Count;

        var currentExchangeRate = await _exchangeRateService.GetCurrentBtcToUahExchangeRateAsync();

        var tasks = emailAddresses
            .Select(address => _emailService.SendEmailAsync(address, "Current Exchange Rate",
                $"Hello, {address}!\n\nWe have a new BTC to UAH exchange rate for you! It is {currentExchangeRate} now!"))
            .ToList();

        var results = await Task.WhenAll(tasks);
        result.SuccessfullyNotified = results.Count(x => x.IsSuccessful);
        result.Failed = results.Where(x => !x.IsSuccessful)
            .Select(x => new FailedEmailNotificationSummary()
            {
                EmailAddress = x.Email!, Error = string.Join(", ", x.Errors ?? Array.Empty<string>())
            }).ToList();

        return result;
    }
}