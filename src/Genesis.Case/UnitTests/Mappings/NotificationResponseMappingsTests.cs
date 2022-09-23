using System.Collections.Generic;
using System.Linq;
using Api.Mappings;
using Api.Models.Responses;
using AutoMapper;
using Core.Contracts.Models;
using Xunit;

namespace UnitTests.Mappings;

public class NotificationResponseMappingsTests
{
    const string TestEmail = "test.email@gmail.com";
    const string EmailSendingError = "test message";

    private readonly IMapper _sut;

    public NotificationResponseMappingsTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<NotificationsMappingProfiles>());
        _sut = mapperConfig.CreateMapper();
    }

    [Fact]
    public void AutoMapper_MapFromFailedEmailNotificationSummary_ReturnsFailedEmailNotificationSummaryResponse()
    {
        // Arrange
        const string testEmail = "test.email@gmail.com";
        const string emailSendingError = "test message";

        var summary = new FailedEmailNotificationSummary {EmailAddress = testEmail, Error = emailSendingError};

        var expected = new FailedEmailNotificationSummaryResponse {EmailAddress = testEmail, Error = emailSendingError};

        // Act
        var result = _sut.Map<FailedEmailNotificationSummaryResponse>(summary);

        // Assert
        Assert.Equal(expected.EmailAddress, result.EmailAddress);
        Assert.Equal(expected.Error, result.Error);
    }

    [Theory]
    [MemberData(nameof(SendEmailNotificationsResultData))]
    public void AutoMapper_MapFromSendEmailNotificationsResult_ReturnsSendEmailsResponse(
        SendEmailNotificationsResponse core, SendEmailsResponse api)
    {
        // Arrange

        // Act
        var result = _sut.Map<SendEmailsResponse>(core);

        // Assert
        Assert.Equal(api.TotalSubscribers, result.TotalSubscribers);
        Assert.Equal(api.SuccessfullyNotified, result.SuccessfullyNotified);
        Assert.Equal(api.Failed!.Count, result.Failed!.Count);

        if (api.Failed.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < api.Failed.Count; i++)
        {
            var expected = api.Failed[i];
            var mapped = result.Failed[i];

            Assert.Equal(expected.EmailAddress, mapped.EmailAddress);
            Assert.Equal(expected.Error, mapped.Error);
        }
    }

    public static TheoryData<SendEmailNotificationsResponse, SendEmailsResponse> SendEmailNotificationsResultData =>
        new()
        {
            {
                new SendEmailNotificationsResponse
                {
                    TotalSubscribers = 1,
                    SuccessfullyNotified = 1,
                    Failed = Enumerable.Empty<FailedEmailNotificationSummary>().ToList()
                },
                new SendEmailsResponse
                {
                    TotalSubscribers = 1,
                    SuccessfullyNotified = 1,
                    Failed = Enumerable.Empty<FailedEmailNotificationSummaryResponse>().ToList()
                }
            },
            {
                new SendEmailNotificationsResponse
                {
                    TotalSubscribers = 1,
                    SuccessfullyNotified = 0,
                    Failed = new List<FailedEmailNotificationSummary>()
                    {
                        new()
                        {
                            EmailAddress = TestEmail,
                            Error = EmailSendingError
                        }
                    }
                },
                new SendEmailsResponse
                {
                    TotalSubscribers = 1,
                    SuccessfullyNotified = 0,
                    Failed = new List<FailedEmailNotificationSummaryResponse>
                    {
                        new()
                        {
                            EmailAddress = TestEmail,
                            Error = EmailSendingError
                        }
                    }
                }
            },
        };
}