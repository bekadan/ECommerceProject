using Core.Events.Abstractions;

namespace Core.Events.Tests.Events;

public class ProductCreatedEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public ProductCreatedEvent(Guid productId)
    {
        ProductId = productId;
    }
}
