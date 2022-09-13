using Core.Abstractions;
using Core.Notifications.Emails.Models;
using Core.Notifications.Emails.Providers.Abstractions;

namespace Core.Services;

public class EmailService : IEmailService
{
    private readonly IGmailProvider _gmailProvider;
        
    public EmailService(IGmailProvider gmailProvider)
    {
        _gmailProvider = gmailProvider;
    }

    public async Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications)
    {
        return await _gmailProvider.SendEmailsAsync(notifications);
    }
}