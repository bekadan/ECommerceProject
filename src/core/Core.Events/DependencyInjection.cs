using Azure;
using Core.Events.Dispatching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Events;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreEvents(this IServiceCollection services, IConfiguration configuration)
    {
        var useEventGrid = configuration.GetValue<bool>("Events:UseEventGrid");

        if (useEventGrid)
        {
            var endpoint = configuration["Events:EventGridEndpoint"]
                           ?? throw new InvalidOperationException("Events:EventGridEndpoint not set");
            var key = configuration["Events:EventGridKey"]
                      ?? throw new InvalidOperationException("Events:EventGridKey not set");
            var credential = new AzureKeyCredential(key);

            services.AddSingleton<IIntegrationEventDispatcher>(sp =>
                new AzureEventGridDispatcher(endpoint, credential));
        }
        else
        {
            services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();
        }

        return services;
    }

}

/*
 {
  "Events": {
    "UseEventGrid": true,
    "EventGridEndpoint": "https://<your-topic-name>.<region>-1.eventgrid.azure.net/api/events",
    "EventGridKey": "<your-access-key>"
  }
}

// Add Core Events package
builder.Services.AddCoreEvents(builder.Configuration);
 
 */