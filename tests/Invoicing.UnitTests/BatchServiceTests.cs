using System;
using System.Collections.Generic;
using System.Linq;
using Contoso.Invoicing.Application.Dtos;
using Contoso.Invoicing.Application.Services;
using Contoso.Invoicing.Domain.Models;
using Contoso.Invoicing.Infrastructure.Events;
using Contoso.Invoicing.Infrastructure.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.Invoicing.UnitTests
{
    [TestClass]
    public class BatchServiceTests
    {
        private BatchService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new BatchService(
                new InMemoryInvoiceBatchRepository(),
                new InMemoryInvoiceRepository(),
                new ConsoleEventPublisher());
        }

        [TestMethod]
        public void CreateBatch_ReturnsPendingBatch()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var result = _service.CreateBatch(request);

            Assert.AreEqual("Pending", result.Status);
            Assert.AreEqual("admin@contoso.com", result.RequestedBy);
            Assert.AreNotEqual(Guid.Empty, result.BatchId);
        }

        [TestMethod]
        public void RunBatch_CompletesAndGeneratesInvoices()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002", "CUST-003" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var batch = _service.CreateBatch(request);
            var result = _service.RunBatch(batch.BatchId);

            Assert.AreEqual("Completed", result.Status);
            Assert.AreEqual(3, result.InvoiceCount);
            Assert.IsTrue(result.GrandTotalAmount > 0);
        }

        [TestMethod]
        public void ListInvoices_AfterRun_ReturnsCorrectCount()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002" },
                FromDate = new DateTime(2023, 6, 1),
                ToDate = new DateTime(2023, 6, 30),
                RequestedBy = "finance@contoso.com"
            };

            var batch = _service.CreateBatch(request);
            _service.RunBatch(batch.BatchId);
            var invoices = _service.ListInvoices(batch.BatchId);

            Assert.AreEqual(2, invoices.Count);
            Assert.IsTrue(invoices.All(i => i.Currency == "USD"));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetBatch_NonExistent_Throws()
        {
            _service.GetBatch(Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RunBatch_AlreadyCompleted_Throws()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var batch = _service.CreateBatch(request);
            _service.RunBatch(batch.BatchId);
            _service.RunBatch(batch.BatchId);
        }
    }
}
