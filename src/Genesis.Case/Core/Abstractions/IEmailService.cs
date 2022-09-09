using Core.Models;

namespace Core.Abstractions;

public interface IEmailService
{
    Task<List<SendEmailResult>> SendEmailsAsync(IEnumerable<EmailNotification> notifications);
}