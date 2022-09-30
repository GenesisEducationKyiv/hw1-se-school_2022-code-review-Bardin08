using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Integrations.Notifications.Emails;

public class SmtpClientFactory : ISmtpClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SmtpClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISmtpClient GetSmtpClient()
    {
        return _serviceProvider.GetService<ISmtpClient>()!;
    }
}