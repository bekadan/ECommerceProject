using Core.Events.Abstractions;
using Core.Events.Handlers;

namespace Core.Events.Extensions;

public static class EventHandlerExtensions
{
    public static Task InvokeHandlerAsync(this IEventHandler<IDomainEvent> handler, IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        dynamic dynHandler = handler;
        dynamic dynEvent = domainEvent;
        return dynHandler.HandleAsync(dynEvent, cancellationToken);
    }
}
