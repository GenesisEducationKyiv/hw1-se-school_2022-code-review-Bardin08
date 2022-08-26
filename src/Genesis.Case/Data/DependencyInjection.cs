using Data.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DependencyInjection
{
    public static void AddDataLayer(this IServiceCollection services)
    {
        services.AddScoped<IJsonEmailsStorage, JsonEmailsStorage>();
    }
}