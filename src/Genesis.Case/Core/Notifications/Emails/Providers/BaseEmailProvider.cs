using Core.Notifications.Emails.Models;
using Core.Notifications.Emails.Providers.Abstractions;
using MimeKit;
using MimeKit.Text;

namespace Core.Notifications.Emails.Providers;

public abstract class BaseEmailProvider : IBaseEmailProvider
{
    private readonly EmailServiceConfiguration _configuration;

    protected BaseEmailProvider(EmailServiceConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected MimeMessage GetMessage(EmailNotification notification)
    {
        return new MimeMessage
        {
            From = {new MailboxAddress(_configuration.SenderName, _configuration.Login)},
            To = {new MailboxAddress(notification.To, notification.To)},
            Subject = notification.Subject ?? String.Empty,
            Body = new TextPart(TextFormat.Plain) {Text = notification.Message ?? String.Empty}
        };
    }

    public abstract Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications);
}