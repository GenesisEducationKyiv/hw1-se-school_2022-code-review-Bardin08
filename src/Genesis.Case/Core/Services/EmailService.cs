using Core.Abstractions;
using Core.Models.Notifications.Emails;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Core.Services;

public class EmailService : IEmailService
{
    private readonly EmailServiceConfiguration _configuration;

    public EmailService(EmailServiceConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications)
    {
        var tasks = notifications.Select(SendEmailAsync);
        var notificationResults = await Task.WhenAll(tasks);
        return notificationResults.ToList();
    }

    private async Task<SendEmailResult> SendEmailAsync(EmailNotification notification)
    {
        var smtpClient = new SmtpClient();

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

    private MimeMessage GetMessage(EmailNotification notification)
    {
        return new MimeMessage
        {
            From = {new MailboxAddress(_configuration.SenderName, _configuration.Login)},
            To = {new MailboxAddress(notification.To, notification.To)},
            Subject = notification.Subject ?? String.Empty,
            Body = new TextPart(TextFormat.Plain) {Text = notification.Message ?? String.Empty}
        };
    }
}