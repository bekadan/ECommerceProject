using Core.Utilities.Abstractions;
using Core.Utilities.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Utilities;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreUtilities(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IGuidProvider, GuidProvider>();
        return services;
    }
}
