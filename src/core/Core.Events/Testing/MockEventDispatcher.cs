using Core.Events.Abstractions;
using Core.Events.Dispatching;
using System.Collections.Concurrent;

namespace Core.Events.Testing;

public class MockEventDispatcher : IEventDispatcher
{
    public ConcurrentBag<IDomainEvent> DispatchedEvents { get; } = new();

    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        DispatchedEvents.Add(domainEvent);
        return Task.CompletedTask;
    }

    public bool WasDispatched<TEvent>() where TEvent : IDomainEvent
    {
        return DispatchedEvents.OfType<TEvent>().Any();
    }
}
