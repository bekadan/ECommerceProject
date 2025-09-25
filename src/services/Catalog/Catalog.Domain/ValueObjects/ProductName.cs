using Shared.Core.Primitives;
using Shared.Core.Utility;

namespace Catalog.Domain.ValueObjects;

public class ProductName : ValueObject
{
    public string Value { get; }

    public ProductName(string value)
    {
        Ensure.NotEmpty(value, "Product Name cannot be empty", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
