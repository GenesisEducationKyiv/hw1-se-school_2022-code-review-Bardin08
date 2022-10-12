using System.Text;
using Core.Abstractions;
using Core.Contracts.Abstractions;
using Core.Contracts.Models;
using Core.Models.Customers;
using Core.Models.Notifications;
using Data.Providers;
using Integrations.RabbitMq.Abstractions;
using Integrations.RabbitMq.Models;
using Newtonsoft.Json;

namespace Core.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IEmailService _emailService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly IJsonEmailsStorage _emailsStorage;
    private readonly IAmqpProducer _amqpProducer;

    public SubscriptionService(
        IEmailService emailService,
        IExchangeRateService exchangeRateService,
        IJsonEmailsStorage emailsStorage,
        IAmqpProducer amqpProducer)
    {
        _emailService = emailService;
        _exchangeRateService = exchangeRateService;
        _emailsStorage = emailsStorage;
        _amqpProducer = amqpProducer;

        _amqpProducer.CreateQueue(new QueueModel
        {
            QueueName = "customer_create_requests",
            IsAutoDelete = false
        });
    }

    public async Task<bool> SubscribeAsync(string email)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var isAlreadyExists = await _emailsStorage.ReadAsync(email) != null;

        if (isAlreadyExists)
        {
            return false;
        }

        var createCustomerRequest = new CreateCustomerRequest {Email = email};
        _amqpProducer.SendMessages(new MessageModel[]
        {
            new()
            {
                RoutingKey = "customer_create_requests",
                Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(createCustomerRequest))
            }
        });

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
            Message =
                $"Hello, {email}!\n\nWe have a new BTC to UAH exchange rate for you! It is {currentExchangeRate} now!"
        }).ToList();

        return await _emailService.SendEmailsAsync(notifications);
    }
}