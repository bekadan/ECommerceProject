using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Core.Exceptions.Types;
using Shared.Core.Primitives;

namespace Catalog.Domain.Aggregates;

public class ProductAggregate : AggregateRoot
{
    public Product Product { get; private set; }
    public Category? Category { get; private set; } // optional, can load lazily

    private ProductAggregate() { } // For ORM or serialization

    public static ProductAggregate FromEntity(Product entity)
    {
        return new ProductAggregate(entity);
    }

    public static ProductAggregate CreateNew(string name, decimal price, string currency, Guid categoryId, int stock)
    {
        var product = new Product(Guid.NewGuid(), new ProductName(name), new Money(price, currency), categoryId, stock);

        return new ProductAggregate(product);
    }

    public ProductAggregate(Product product, Category? category = null)
    {
        Product = product ?? throw new DomainException("Product cannot be null.");
        Category = category;

        AddDomainEvent(new Events.ProductCreatedEvent(Product));
    }

    // Example aggregate method: change price with domain rules
    public void ChangeProductPrice(Money newPrice)
    {
        if (newPrice == null)
            throw new DomainException("New price cannot be null.");

        Product.ChangePrice(newPrice);
        AddDomainEvent(new Events.ProductPriceChangedEvent(Product));
    }

    // Example: adjust stock
    public void AdjustStock(int amount)
    {
        Product.AdjustStock(amount);
    }

    // Optional: assign or change category
    public void AssignCategory(Category category)
    {
        if (category is null)
            throw new DomainException("Category cannot be null.");

        Category = category;
    }
}
