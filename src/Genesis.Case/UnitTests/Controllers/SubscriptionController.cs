using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Mappings;
using AutoMapper;
using Core.Abstractions;
using Core.Notifications.Emails.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests.Controllers;

public class SubscriptionControllerTests
{
    private readonly SubscriptionController _sut;
    private readonly Mock<ISubscriptionService> _subscriptionServiceMock = new();
    private readonly IMapper _mapper;

    public SubscriptionControllerTests()
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile<NotificationsMappingProfiles>();
        });
        _mapper = mapperConfig.CreateMapper();

        _sut = new SubscriptionController(_subscriptionServiceMock.Object, _mapper)
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
        };
    }

    [Theory]
    [InlineData("valid@email.com")]
    [InlineData("also.valid@email.com")]
    [InlineData("one.more_valid@email.com")]
    public async Task Subscribe_WhenEmailValid_ReturnsStatus200OK(string email)
    {
        // Arrange
        _subscriptionServiceMock.Setup(x => x.SubscribeAsync(email))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Subscribe(email);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Theory]
    [InlineData("invalid_email")]
    [InlineData("invalid2135email")]
    [InlineData("invalid2135.email.com")]
    [InlineData("2..invalid@email.com")]
    public async Task Subscribe_WhenEmailInvalid_ReturnsStatus400BadRequest(string email)
    {
        // Arrange
        _subscriptionServiceMock.Setup(x => x.SubscribeAsync(email))
            .ReturnsAsync(false);
        
        // Act
        var result = await _sut.Subscribe(email);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Subscribe_WhenEmailAlreadySubscribed_ReturnsStatus409Conflict()
    {
        // Arrange
        const string email = "already_subscribed@email.com";
        _subscriptionServiceMock.Setup(x => x.SubscribeAsync(email)).ReturnsAsync(false);
        
        // Act
        var result = await _sut.Subscribe(email);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }
    
    [Fact]
    public async Task Notify_WhenAllNotificationsSent_ReturnsStatus200OK()
    {
        // Arrange
        _subscriptionServiceMock.Setup(x => x.NotifyAsync()).ReturnsAsync(new SendEmailNotificationsResult
        {
            TotalSubscribers = 1,
            SuccessfullyNotified = 1,
            Failed = Enumerable.Empty<FailedEmailNotificationSummary>().ToList()
        });
        
        // Act
        var result = await _sut.Notify();
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}