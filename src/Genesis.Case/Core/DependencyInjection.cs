using Core.Abstractions;
using Core.APIs;
using Core.Notifications.Emails;
using Core.Notifications.Emails.Models;
using Core.Notifications.Emails.Providers;
using Core.Notifications.Emails.Providers.Abstractions;
using Core.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static void AddCoreLogic(this IServiceCollection services, IConfiguration configuration)
    {
        var gmailServiceConfiguration = new EmailServiceConfiguration();
        configuration.Bind("Email", gmailServiceConfiguration);
        services.AddSingleton(gmailServiceConfiguration); 

        services.AddTransient<ICoinBaseApi, CoinBaseApi>();

        services.AddTransient<IExchangeRateService, ExchangeRateService>();
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddTransient<IEmailService, EmailService>();

        services.AddHttpClient<ICoinBaseApi, CoinBaseApi>(client =>
        {
            client.BaseAddress = new Uri("https://api.coinbase.com/v2/");
        }).AddPolicyHandler(HttpRetryPolicies.GetRetryPolicy());

        services.AddTransient<ISmtpClient, SmtpClient>();
        services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
        services.AddScoped<IGmailProvider, GmailEmailProvider>();
    }
}