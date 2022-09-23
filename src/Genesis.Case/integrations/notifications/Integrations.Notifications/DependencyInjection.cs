using Integrations.Notifications.Contracts.Abstractions;
using Integrations.Notifications.Contracts.Abstractions.Emails;
using Integrations.Notifications.Emails;
using Integrations.Notifications.Emails.Models;
using Integrations.Notifications.Emails.Providers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integrations.Notifications;

public static class DependencyInjection
{
    public static void AddNotificationsIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        var gmailServiceConfiguration = new EmailServiceConfiguration();
        configuration.Bind("Email", gmailServiceConfiguration);
        services.AddSingleton(gmailServiceConfiguration); 

        services.AddTransient<ISmtpClient, SmtpClient>();
        services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
        services.AddScoped<IGmailProvider, GmailEmailProvider>();
    }
}