using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Models;
using Core.Services;
using Data.Providers;
using Moq;
using Xunit;

namespace UnitTests.Services;

public class SubscriptionServiceTests
{
    private readonly SubscriptionService _sut;
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IExchangeRateService> _exchangeRateServiceMock = new();
    private readonly Mock<IJsonEmailsStorage> _jsonEmailsStorageMock = new();

    public SubscriptionServiceTests()
    {
        _sut = new SubscriptionService(
            _emailServiceMock.Object,
            _exchangeRateServiceMock.Object,
            _jsonEmailsStorageMock.Object);
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
        _exchangeRateServiceMock.Setup(x => x.GetCurrentBtcToUahExchangeRateAsync()).ReturnsAsync(1);
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string e, string _, string _) =>
                new SendEmailResult {Email = e, IsSuccessful = true, Timestamp = DateTimeOffset.UtcNow});

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
        _exchangeRateServiceMock.Setup(x => x.GetCurrentBtcToUahExchangeRateAsync()).ReturnsAsync(1);
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string e, string _, string _) =>
                new SendEmailResult
                {
                    Email = e,
                    IsSuccessful = false,
                    Timestamp = DateTimeOffset.UtcNow,
                    Errors = new[] {"Notification sending failed"}
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