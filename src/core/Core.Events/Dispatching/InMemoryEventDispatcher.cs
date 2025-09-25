using Core.Events.Abstractions;
using Core.Events.Extensions;
using Core.Events.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Events.Dispatching;

public class InMemoryEventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType)
                                       .Cast<IEventHandler<IDomainEvent>>();

        foreach (var handler in handlers)
        {
            await handler.InvokeHandlerAsync(domainEvent, cancellationToken);
        }
    }
}
