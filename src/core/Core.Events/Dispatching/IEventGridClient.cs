using Azure.Messaging.EventGrid;

namespace Core.Events.Dispatching;

public interface IEventGridClient
{
    Task SendEventAsync(EventGridEvent evt, CancellationToken cancellationToken = default);
}
