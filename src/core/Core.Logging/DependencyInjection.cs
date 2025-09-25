using Core.Logging.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Core.Logging;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<Abstractions.ILogger, Logger>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationIdHeader() // Requires Serilog.Enrichers.CorrelationId
            .WriteTo.Console()
            .WriteTo.ApplicationInsights(configuration["ApplicationInsights:InstrumentationKey"], TelemetryConverter.Traces)
            .CreateLogger();

        return services;
    }

}

/*
 // Register Core.Logging
builder.Services.AddCoreLogging(builder.Configuration);

// Use request/response logging middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

{
  "ApplicationInsights": {
    "InstrumentationKey": "<your-key>"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}


 */