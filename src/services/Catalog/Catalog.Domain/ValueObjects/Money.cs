using Shared.Core.Primitives;
using Shared.Core.Utility;

namespace Catalog.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "USD")
        {
            Ensure.NotLessThan(amount, 0, "Money amount cannot be negative.", nameof(amount));

            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
