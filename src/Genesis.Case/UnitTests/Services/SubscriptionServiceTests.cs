using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Contracts.Abstractions;
using Core.Contracts.Models;
using Core.Models.Notifications;
using Core.Services;
using Data.Providers;
using Integrations.RabbitMq.Abstractions;
using Integrations.RabbitMq.Models;
using Moq;
using Xunit;

namespace UnitTests.Services;

public class SubscriptionServiceTests
{
    private readonly SubscriptionService _sut;
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IExchangeRateService> _exchangeRateServiceMock = new();
    private readonly Mock<IJsonEmailsStorage> _jsonEmailsStorageMock = new();
    private readonly Mock<IAmqpProducer> _amqpProducerMock = new();

    public SubscriptionServiceTests()
    {
        _sut = new SubscriptionService(
            _emailServiceMock.Object,
            _exchangeRateServiceMock.Object,
            _jsonEmailsStorageMock.Object,
            _amqpProducerMock.Object);
    }

    [Fact]
    public async Task Subscribe_NewEmail_ReturnsTrue()
    {
        // Arrange
        const string email = "mail@gmail.com";
        _jsonEmailsStorageMock.Setup(x => x.ReadAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Array.Empty<string>());
        _jsonEmailsStorageMock.Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(email);
        _amqpProducerMock.SetupSequence(x => x.CreateQueue(It.IsAny<QueueModel>())).Pass();
        _amqpProducerMock.SetupSequence(x => x.SendMessages(It.IsAny<IEnumerable<MessageModel>>())).Pass();

        // Act
        var result = await _sut.SubscribeAsync(email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Subscribe_ExistingEmail_ReturnsFalse()
    {
        // Arrange
        const string email = "mail@gmail.com";
        _jsonEmailsStorageMock.Setup(x => x.ReadAsync(email))
            .ReturnsAsync(email);
        _jsonEmailsStorageMock.Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(email);

        // Act
        var result = await _sut.SubscribeAsync(email);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData((string?)null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Subscribe_EmailIsNullOrEmpty_ReturnsFalse(string email)
    {
        // Arrange & Act
        var result = await _sut.SubscribeAsync(email);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [MemberData(nameof(GetSubscribedEmails))]
    public async Task Notify_WhenAllNotificationsSent_ReturnsSubscriptionNotifyResult(string[] subscribedEmails)
    {
        // Arrange
        _jsonEmailsStorageMock.Setup(x => x.ReadAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(subscribedEmails);
        _exchangeRateServiceMock.Setup(x => x.GetBtcToUahExchangeRateAsync()).ReturnsAsync(1);
        _emailServiceMock.Setup(x => x.SendEmailsAsync(It.IsAny<IEnumerable<EmailNotification>>()))
            .ReturnsAsync(new SendEmailNotificationsResponse
            {
                TotalSubscribers = subscribedEmails.Length,
                SuccessfullyNotified = subscribedEmails.Length,
                Failed = new List<FailedEmailNotificationSummary>()
            });

        // Act
        var result = await _sut.NotifyAsync();

        // Assert
        Assert.Equal(result.TotalSubscribers, subscribedEmails.Length);
        Assert.Equal(result.SuccessfullyNotified, subscribedEmails.Length);
        Assert.NotNull(result.Failed);
        Assert.Empty(result.Failed!);
    }

    [Theory]
    [MemberData(nameof(GetSubscribedEmails))]
    public async Task Notify_WhenAllNotificationsFailed_ReturnsSubscriptionNotifyResult(string[] subscribedEmails)
    {
        // Arrange
        _jsonEmailsStorageMock.Setup(x => x.ReadAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(subscribedEmails);
        _exchangeRateServiceMock.Setup(x => x.GetBtcToUahExchangeRateAsync()).ReturnsAsync(1);
        _emailServiceMock.Setup(x => x.SendEmailsAsync(It.IsAny<IEnumerable<EmailNotification>>()))
            .ReturnsAsync(new SendEmailNotificationsResponse
            {
                TotalSubscribers = subscribedEmails.Length,
                SuccessfullyNotified = 0,
                Failed = subscribedEmails
                    .Select(x => new FailedEmailNotificationSummary
                    {
                        Error = "Unit test | Email notification failed!", EmailAddress = x
                    }).ToList()
            });

        // Act
        var result = await _sut.NotifyAsync();

        // Assert
        Assert.Equal(result.TotalSubscribers, subscribedEmails.Length);
        Assert.NotNull(result.Failed);
        Assert.Equal(subscribedEmails.Length, result.Failed!.Count);
    }

    public static IEnumerable<object[]> GetSubscribedEmails()
    {
        yield return new object[] {Array.Empty<string>()};
        yield return new object[] {new[] {"mail@email.com"}};
    }
}