using MailKit.Net.Smtp;

namespace Integrations.Notifications.Emails;

public interface ISmtpClientFactory
{
    /// <summary>
    /// Get or create an instance of <see cref="ISmtpClient"/> 
    /// </summary>
    /// <remarks> This factory is build over an IoC container and .Dispose method shouldn't be called for client instances</remarks>
    /// <returns><see cref="ISmtpClient"/></returns>
    ISmtpClient GetSmtpClient();
}