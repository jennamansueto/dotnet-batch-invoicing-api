using System;
using Contoso.Invoicing.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.Invoicing.UnitTests
{
    [TestClass]
    public class MoneyTests
    {
        [TestMethod]
        public void Zero_ReturnsZeroAmount()
        {
            var money = Money.Zero("USD");
            Assert.AreEqual(0m, money.Amount);
            Assert.AreEqual("USD", money.Currency);
        }

        [TestMethod]
        public void Add_SameCurrency_ReturnsSummedAmount()
        {
            var a = new Money(100.50m, "USD");
            var b = new Money(49.50m, "USD");
            var result = a.Add(b);
            Assert.AreEqual(150.00m, result.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Add_DifferentCurrency_Throws()
        {
            var usd = new Money(100m, "USD");
            var eur = new Money(50m, "EUR");
            usd.Add(eur);
        }
    }
}
