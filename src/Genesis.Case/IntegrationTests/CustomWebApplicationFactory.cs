using System.Threading.Tasks;
using Data.Providers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
{
    public override async ValueTask DisposeAsync()
    {
        var scope = Services.CreateScope();
        var emailsStorage = scope.ServiceProvider.GetRequiredService<IJsonEmailsStorage>();

        var allEmails = await emailsStorage.ReadAllAsync(0, 0);

        foreach (var email in allEmails)
        {
            await emailsStorage.DeleteAsync(email);
        }

        await base.DisposeAsync();
    }
}