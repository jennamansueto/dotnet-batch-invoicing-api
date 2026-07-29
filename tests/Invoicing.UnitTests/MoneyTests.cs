using System;
using Contoso.Invoicing.Domain.Models;
using Xunit;

namespace Contoso.Invoicing.UnitTests
{
    public class MoneyTests
    {
        [Fact]
        public void Zero_ReturnsZeroAmount()
        {
            var money = Money.Zero("USD");
            Assert.Equal(0m, money.Amount);
            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void Add_SameCurrency_ReturnsSummedAmount()
        {
            var a = new Money(100.50m, "USD");
            var b = new Money(49.50m, "USD");
            var result = a.Add(b);
            Assert.Equal(150.00m, result.Amount);
        }

        [Fact]
        public void Add_DifferentCurrency_Throws()
        {
            var usd = new Money(100m, "USD");
            var eur = new Money(50m, "EUR");
            Assert.Throws<InvalidOperationException>(() => usd.Add(eur));
        }
    }
}
