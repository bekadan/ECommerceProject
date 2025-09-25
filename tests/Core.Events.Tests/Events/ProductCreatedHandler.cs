using Core.Events.Handlers;

namespace Core.Events.Tests.Events;

public class ProductCreatedHandler : IEventHandler<ProductCreatedEvent>
{
    public int CallCount { get; private set; } = 0;

    public Task HandleAsync(ProductCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.CompletedTask;
    }
}
