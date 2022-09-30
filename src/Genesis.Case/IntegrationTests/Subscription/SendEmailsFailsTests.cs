using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Api.Models.Responses;
using Core.Contracts.Notifications.Models.Emails;
using Data.Providers;
using Integrations.Notifications.Emails;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Subscription;

[Collection("Subscription")]
public class SendEmailsFailsTests
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly IJsonEmailsStorage _emailsStorage;
    private readonly Guid _testUId;

    public SendEmailsFailsTests(CustomWebApplicationFactory<Program> factory)
    {
        _testUId = factory.TestUId;
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var smtpClientMock = new Mock<ISmtpClient>();

                smtpClientMock.Setup(x => x.ConnectAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<SecureSocketOptions>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                smtpClientMock.Setup(x => x.AuthenticateAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                smtpClientMock.Setup(x => x.SendAsync(
                        It.IsAny<MimeMessage>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<ITransferProgress>()))
                    .Throws(new SmtpException("Integration test exception"));

                smtpClientMock.Setup(x => x.DisconnectAsync(
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                var smtpClientFactoryMock = new Mock<ISmtpClientFactory>();
                smtpClientFactoryMock.Setup(x => x.GetSmtpClient())
                    .Returns(smtpClientMock.Object);

                services.AddScoped(_ => smtpClientFactoryMock.Object);

                // var gmailProviderMock = new Mock<IGmailProvider>();

                var subscriber = string.Format("integration-tests_{0}@gmail.com", _testUId);
                var expectedResponse = new List<SendEmailResult>
                {
                    new()
                    {
                        Email = subscriber,
                        Errors = new[]
                        {
                            $"A letter to {subscriber} wasn't sent. SMTP server not respond"
                        },
                        IsSuccessful = false,
                        Timestamp = DateTimeOffset.UtcNow
                    }
                };

                // gmailProviderMock.Setup(x => x.SendEmailsAsync(
                //         It.IsAny<IEnumerable<EmailNotificationDto>>()))
                //     .ReturnsAsync(expectedResponse);
// 
                // services.AddScoped(_ => gmailProviderMock.Object);
            });
        }).CreateClient();

        var scope = factory.Services.CreateScope();
        _emailsStorage = scope.ServiceProvider.GetService<IJsonEmailsStorage>()!;
    }

    [Fact]
    public async Task SendEmails_SmtpServerNotResponse_ReturnsOk()
    {
        const string email = "integration-tests_{0}@gmail.com";
        var subscriber = string.Format(email, _testUId);
        
        var expectedResponse = new SendEmailsResponse
        {
            TotalSubscribers = 1,
            SuccessfullyNotified = 0,
            Failed = new List<FailedEmailNotificationSummaryResponse>()
            {
                new()
                {
                    EmailAddress = subscriber,
                    Error = $"A letter to {subscriber} wasn't sent. SMTP server not respond"
                }
            }
        };

        await _emailsStorage.CreateAsync(subscriber);

        var response = await _httpClient.PostAsync("/sendEmails", new StringContent(""));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<SendEmailsResponse>(responseString)!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse.TotalSubscribers, responseModel.TotalSubscribers);
        Assert.Equal(expectedResponse.SuccessfullyNotified, responseModel.SuccessfullyNotified);

        Assert.Collection(responseModel.Failed,
            x => x.EmailAddress = $"A letter to {subscriber} wasn't sent. SMTP server not respond");
    }
}