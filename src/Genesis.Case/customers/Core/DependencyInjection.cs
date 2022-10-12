using System.Reflection;
using Core.Abstractions;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static void AddCore(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetAssembly(typeof(DependencyInjection)));
        services.AddScoped<ICustomerService, CustomerService>();
    }
}