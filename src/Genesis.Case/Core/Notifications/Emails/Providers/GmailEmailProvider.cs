using Core.Notifications.Emails.Models;
using Core.Notifications.Emails.Providers.Abstractions;
using MailKit.Security;

namespace Core.Notifications.Emails.Providers;

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

    public override async Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications)
    {
        var tasks = notifications.Select(SendEmailAsync);
        var notificationResults = await Task.WhenAll(tasks);
        return notificationResults.ToList();
    }

    private async Task<SendEmailResult> SendEmailAsync(EmailNotification notification)
    {
        var smtpClient = _smtpClientFactory.GetSmtpClient();

        try
        {
            var email = GetMessage(notification);

            await smtpClient.ConnectAsync(_configuration.SmtpHost, _configuration.Port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_configuration.Login, _configuration.Password);
            await smtpClient.SendAsync(email);

            return new SendEmailResult
            {
                IsSuccessful = true, Email = notification.To, Timestamp = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new SendEmailResult
            {
                IsSuccessful = false,
                Email = notification.To,
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