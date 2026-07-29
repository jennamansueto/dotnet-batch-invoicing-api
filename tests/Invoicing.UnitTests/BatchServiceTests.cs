using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contoso.Invoicing.Application.Dtos;
using Contoso.Invoicing.Application.Services;
using Contoso.Invoicing.Infrastructure.Events;
using Contoso.Invoicing.Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Contoso.Invoicing.UnitTests
{
    public class BatchServiceTests
    {
        private readonly BatchService _service;

        public BatchServiceTests()
        {
            _service = new BatchService(
                new InMemoryInvoiceBatchRepository(),
                new InMemoryInvoiceRepository(),
                new ConsoleEventPublisher(NullLogger<ConsoleEventPublisher>.Instance));
        }

        [Fact]
        public async Task CreateBatch_ReturnsPendingBatch()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var result = await _service.CreateBatchAsync(request);

            Assert.Equal("Pending", result.Status);
            Assert.Equal("admin@contoso.com", result.RequestedBy);
            Assert.NotEqual(Guid.Empty, result.BatchId);
        }

        [Fact]
        public async Task RunBatch_CompletesAndGeneratesInvoices()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002", "CUST-003" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var batch = await _service.CreateBatchAsync(request);
            var result = await _service.RunBatchAsync(batch.BatchId);

            Assert.Equal("Completed", result.Status);
            Assert.Equal(3, result.InvoiceCount);
            Assert.True(result.GrandTotalAmount > 0);
        }

        [Fact]
        public async Task ListInvoices_AfterRun_ReturnsCorrectCount()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002" },
                FromDate = new DateTime(2023, 6, 1),
                ToDate = new DateTime(2023, 6, 30),
                RequestedBy = "finance@contoso.com"
            };

            var batch = await _service.CreateBatchAsync(request);
            await _service.RunBatchAsync(batch.BatchId);
            var invoices = await _service.ListInvoicesAsync(batch.BatchId);

            Assert.Equal(2, invoices.Count);
            Assert.True(invoices.All(i => i.Currency == "USD"));
        }

        [Fact]
        public async Task GetBatch_NonExistent_Throws()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetBatchAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task RunBatch_AlreadyCompleted_Throws()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var batch = await _service.CreateBatchAsync(request);
            await _service.RunBatchAsync(batch.BatchId);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RunBatchAsync(batch.BatchId));
        }
    }
}
