using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Api.Models.Responses;
using Core.Notifications.Emails;
using Core.Notifications.Emails.Models;
using Core.Notifications.Emails.Providers.Abstractions;
using Data.Providers;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Subscription;

public class SendEmailsFailsTests
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly IJsonEmailsStorage _emailsStorage;


    public SendEmailsFailsTests(CustomWebApplicationFactory<Program> factory)
    {
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
                    .ReturnsAsync(string.Empty);

                smtpClientMock.Setup(x => x.DisconnectAsync(
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                var smtpClientFactoryMock = new Mock<ISmtpClientFactory>();
                smtpClientFactoryMock.Setup(x => x.GetSmtpClient())
                    .Returns(smtpClientMock.Object);

                services.AddScoped(_ => smtpClientFactoryMock.Object);

                var gmailProviderMock = new Mock<IGmailProvider>();

                var expectedResponse = new List<SendEmailResult>
                {
                    new()
                    {
                        Email = "integration-tests@gmail.com",
                        Errors = new[]
                        {
                            "A letter to integration-tests@gmail.com wasn't sent. SMTP server not respond"
                        },
                        IsSuccessful = false,
                        Timestamp = DateTimeOffset.UtcNow
                    }
                };

                gmailProviderMock.Setup(x => x.SendEmailsAsync(
                        It.IsAny<List<EmailNotification>>()))
                    .ReturnsAsync(expectedResponse);

                services.AddScoped(_ => gmailProviderMock.Object);
            });
        }).CreateClient();

        var scope = factory.Services.CreateScope();
        _emailsStorage = scope.ServiceProvider.GetService<IJsonEmailsStorage>()!;
    }

    [Fact]
    public async Task SendEmails_SmtpServerNotResponse_ReturnsOk()
    {
        const string subscriber = "integration-tests@gmail.com";

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