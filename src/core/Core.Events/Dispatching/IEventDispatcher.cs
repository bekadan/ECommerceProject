using Core.Events.Abstractions;

namespace Core.Events.Dispatching;

public interface IEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
