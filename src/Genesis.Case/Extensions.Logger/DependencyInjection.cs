using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Extensions.Logger;

public static class DependencyInjection
{
    public static void AddCustomLogger(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, FusionLogger>();
        services.AddSingleton<ILoggerFactory, FusionLoggerFactory>();
    }
}