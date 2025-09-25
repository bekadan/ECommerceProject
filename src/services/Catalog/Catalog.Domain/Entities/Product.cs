using Catalog.Domain.Aggregates;
using Catalog.Domain.ValueObjects;
using Shared.Core.Primitives;
using Shared.Core.Utility;

namespace Catalog.Domain.Entities;

public class Product : AggregateRoot
{
    public ProductName Name { get; private set; }
    public Money Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public int Stock { get; private set; }

    // Add navigation property
    public Category? Category { get; private set; }

    private Product() { } // For ORM or serialization

    public Product(Guid guid, ProductName name, Money price, Guid categoryId, int stock)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        CategoryId = categoryId;
        Stock = stock;

        AddDomainEvent(new Events.ProductCreatedEvent(this));
    }

    

    public void ChangePrice(Money newPrice)
    {
        Ensure.NotEmpty(newPrice.Currency, "Currency cannot be empty", nameof(newPrice.Currency));
        Ensure.NotLessThan(newPrice.Amount, 0,"Price Amount cannot be less than zero", nameof(newPrice.Amount));
        if (Price.Equals(newPrice)) return;

        Price = newPrice;
        AddDomainEvent(new Events.ProductPriceChangedEvent(this));
    }

    public void AdjustStock(int amount)
    {
        Ensure.NotLessThan(Stock + amount, 0, "Stock cannot be negative.", nameof(amount));

        Stock += amount;
    }
}
