using Catalog.Domain.Entities;
using Core.Events.Abstractions;

namespace Catalog.Domain.Events;

public class ProductPriceChangedEvent : IDomainEvent
{
    public Product Product { get; }

    public DateTime OccurredOn { get; }

    public ProductPriceChangedEvent(Product product)
    {
        Product = product;
        OccurredOn = DateTime.UtcNow;
    }
}
