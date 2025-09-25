using Catalog.Domain.Aggregates;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Core.Exceptions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Factories;

public static class ProductAggregateFactory
{
    /// <summary>
    /// Creates a new ProductAggregate with all domain objects properly initialized.
    /// </summary>
    /// <param name="productName">Name of the product</param>
    /// <param name="priceAmount">Price amount</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <param name="categoryId">Category Id</param>
    /// <param name="stock">Initial stock quantity</param>
    /// <param name="categoryName">Optional category name if creating a new category</param>
    /// <returns>ProductAggregate instance</returns>
    public static ProductAggregate CreateNew(
        string productName,
        decimal priceAmount,
        string currency,
        Guid categoryId,
        int stock,
        string? categoryName = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name cannot be empty.");

        if (priceAmount < 0)
            throw new DomainException("Price cannot be negative.");

        if (stock < 0)
            throw new DomainException("Stock cannot be negative.");

        var product = new Product(
            new ProductName(productName),
            new Money(priceAmount, currency),
            categoryId,
            stock
        );

        Category? category = null;
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            category = new Category(categoryName);
        }

        var aggregate = new ProductAggregate(product, category);

        return aggregate;
    }
}
