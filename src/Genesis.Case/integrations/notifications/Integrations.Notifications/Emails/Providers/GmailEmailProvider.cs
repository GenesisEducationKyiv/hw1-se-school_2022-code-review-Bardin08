using Core.Contracts.Notifications.Abstractions.Emails;
using Core.Contracts.Notifications.Models.Emails;
using Integrations.Notifications.Emails.Models;
using MailKit.Security;

namespace Integrations.Notifications.Emails.Providers;

public class GmailEmailProvider : BaseEmailProvider, IGmailProvider
{
    private readonly ISmtpClientFactory _smtpClientFactory;
    private readonly EmailServiceConfiguration _configuration;

    public GmailEmailProvider(
        ISmtpClientFactory smtpClientFactory,
        EmailServiceConfiguration configuration) : base(configuration)
    {
        _smtpClientFactory = smtpClientFactory;
        _configuration = configuration;
    }

    public override async Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotificationDto> notifications)
    {
        var tasks = notifications.Select(SendEmailAsync);
        var notificationResults = await Task.WhenAll(tasks);
        return notificationResults.ToList();
    }

    private async Task<SendEmailResult> SendEmailAsync(EmailNotificationDto notificationDto)
    {
        var smtpClient = _smtpClientFactory.GetSmtpClient();

        try
        {
            var email = GetMessage(notificationDto);

            await smtpClient.ConnectAsync(_configuration.SmtpHost, _configuration.Port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_configuration.Login, _configuration.Password);
            await smtpClient.SendAsync(email);

            return new SendEmailResult
            {
                IsSuccessful = true, Email = notificationDto.To, Timestamp = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new SendEmailResult
            {
                IsSuccessful = false,
                Email = notificationDto.To,
                Timestamp = DateTimeOffset.UtcNow,
                Errors = new[] {ex.Message}
            };
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }
}