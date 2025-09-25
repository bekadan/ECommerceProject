using Azure;
using Azure.Messaging.EventGrid;
using Core.Events.Abstractions;
using Polly;
using Polly.Retry;
using System.Text.Json;

namespace Core.Events.Dispatching;

public class AzureEventGridDispatcher : IIntegrationEventDispatcher
{
    private readonly IEventGridClient _client;
    private readonly AsyncRetryPolicy _retryPolicy;

    // Production constructor
    public AzureEventGridDispatcher(string endpoint, AzureKeyCredential credential)
    {
        var client = new EventGridPublisherClient(new Uri(endpoint), credential);
        _client = new EventGridClientWrapper(client);

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }

    // Testable constructor
    public AzureEventGridDispatcher(IEventGridClient client)
    {
        _client = client;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }

    public async Task DispatchAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var eventGridEvent = new EventGridEvent(
            subject: integrationEvent.GetType().Name,
            eventType: integrationEvent.GetType().Name,
            dataVersion: "1.0",
            data: JsonSerializer.Serialize(integrationEvent)
        );

        await _retryPolicy.ExecuteAsync(async () =>
        {
            await _client.SendEventAsync(eventGridEvent, cancellationToken);
        });
    }
}
