using Core.Models;

namespace Core.Abstractions;

public interface IEmailService
{
    Task<SendEmailResult> SendEmailAsync(string email, string subject, string message);
}