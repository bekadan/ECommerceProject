using Core.Validation.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Validation;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreValidation(this IServiceCollection services)
    {
        // Automatically register all IValidator<T> implementations in the assembly
        var assembly = typeof(DependencyInjection).Assembly;

        foreach (var type in assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))))
        {
            var interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddSingleton(interfaceType, type);
        }

        return services;
    }
}

/*
 * // Register Core.Logging first
builder.Services.AddCoreLogging(builder.Configuration);
builder.Services.AddCoreValidation(); // Automatically injects ILogger into validators
builder.Services.AddCore.Exceptions(); // To throw ValidationException
 */
