using Core.Abstractions;
using Core.APIs;
using Core.Models;
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
    }
}