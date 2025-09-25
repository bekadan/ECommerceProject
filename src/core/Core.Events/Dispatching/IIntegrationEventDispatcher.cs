using Core.Events.Abstractions;

namespace Core.Events.Dispatching;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
