using Core.Abstractions;
using Core.APIs;
using Core.Models.Notifications.Emails;
using Core.Services;
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
    }
}