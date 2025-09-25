using Core.Events.Handlers;
using Core.Events.Tests.Events;

namespace Core.Events.Tests.Handlers;

public class ProductCreatedHandler : IEventHandler<ProductCreatedEvent>
{
    public int CallCount { get; private set; } = 0;

    public Task HandleAsync(ProductCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.CompletedTask;
    }
}
