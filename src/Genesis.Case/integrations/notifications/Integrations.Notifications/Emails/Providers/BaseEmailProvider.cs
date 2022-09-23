using Integrations.Notifications.Contracts.Abstractions;
using Integrations.Notifications.Contracts.Abstractions.Emails;
using Integrations.Notifications.Contracts.Models.Emails;
using Integrations.Notifications.Emails.Models;
using MimeKit;
using MimeKit.Text;

namespace Integrations.Notifications.Emails.Providers;

public abstract class BaseEmailProvider : IBaseEmailProvider
{
    private readonly EmailServiceConfiguration _configuration;

    protected BaseEmailProvider(EmailServiceConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected MimeMessage GetMessage(EmailNotificationDto notificationDto)
    {
        return new MimeMessage
        {
            From = {new MailboxAddress(_configuration.SenderName, _configuration.Login)},
            To = {new MailboxAddress(notificationDto.To, notificationDto.To)},
            Subject = notificationDto.Subject ?? String.Empty,
            Body = new TextPart(TextFormat.Plain) {Text = notificationDto.Message ?? String.Empty}
        };
    }

    public abstract Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotificationDto> notifications);
}