using Data.Models;
using Data.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DependencyInjection
{
    public static void AddDataLayer(this IServiceCollection services, IConfiguration config)
    {
        var fileStorageConfig = new FileStorageConfiguration();
        config.Bind("FileStorageConfig", fileStorageConfig);
        services.AddSingleton(fileStorageConfig); 

        services.AddScoped<IJsonEmailsStorage, JsonEmailsStorage>();
    }
}