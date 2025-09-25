using Azure.Messaging.EventGrid;

namespace Core.Events.Dispatching;

public class EventGridClientWrapper : IEventGridClient
{
    private readonly EventGridPublisherClient _client;

    public EventGridClientWrapper(EventGridPublisherClient client)
    {
        _client = client;
    }

    public Task SendEventAsync(EventGridEvent evt, CancellationToken cancellationToken = default)
    {
        return _client.SendEventAsync(evt, cancellationToken);
    }
}
