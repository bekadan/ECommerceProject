using Core.Exceptions.Types;
using Shared.Core.Primitives;

namespace Catalog.Domain.Entities;

public class Category : AggregateRoot
{
    public string Name { get; private set; }

    private Category() { }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new DomainException("Category name cannot be null.");
    }
}
