using System;

namespace Contoso.Invoicing.Domain.Models
{
    public class Money
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public Money() { }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public static Money Zero(string currency = "USD")
        {
            return new Money(0m, currency);
        }

        public Money Add(Money other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot add money with different currencies.");
            return new Money(Amount + other.Amount, Currency);
        }
    }
}
