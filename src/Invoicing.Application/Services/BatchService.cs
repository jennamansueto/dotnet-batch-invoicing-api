using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contoso.Invoicing.Application.Dtos;
using Contoso.Invoicing.Domain.Events;
using Contoso.Invoicing.Domain.Models;
using Contoso.Invoicing.Domain.Repositories;

namespace Contoso.Invoicing.Application.Services
{
    public class BatchService : IBatchService
    {
        private readonly IInvoiceBatchRepository _batchRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IEventPublisher _eventPublisher;
        private static readonly Random Rng = new Random();

        public BatchService(
            IInvoiceBatchRepository batchRepo,
            IInvoiceRepository invoiceRepo,
            IEventPublisher eventPublisher)
        {
            _batchRepo = batchRepo ?? throw new ArgumentNullException(nameof(batchRepo));
            _invoiceRepo = invoiceRepo ?? throw new ArgumentNullException(nameof(invoiceRepo));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<BatchSummaryDto> CreateBatchAsync(CreateBatchRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.CustomerIds == null || !request.CustomerIds.Any())
                throw new ArgumentException("At least one customer ID is required.");

            var batch = new InvoiceBatch
            {
                CustomerIds = request.CustomerIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                RequestedBy = request.RequestedBy
            };

            await _batchRepo.AddAsync(batch).ConfigureAwait(false);
            return MapToSummary(batch);
        }

        public async Task<BatchSummaryDto> RunBatchAsync(Guid batchId)
        {
            var batch = await _batchRepo.GetByIdAsync(batchId).ConfigureAwait(false);
            if (batch == null)
                throw new KeyNotFoundException($"Batch {batchId} not found.");

            if (batch.Status != BatchStatus.Pending)
                throw new InvalidOperationException($"Batch is in '{batch.Status}' state and cannot be run.");

            batch.Status = BatchStatus.Running;
            await _batchRepo.UpdateAsync(batch).ConfigureAwait(false);

            try
            {
                var grandTotal = Money.Zero("USD");

                foreach (var customerId in batch.CustomerIds)
                {
                    var lineTotal = new Money(Math.Round((decimal)(Rng.NextDouble() * 5000 + 100), 2), "USD");
                    var invoice = new Invoice
                    {
                        BatchId = batch.BatchId,
                        CustomerId = customerId,
                        PeriodFrom = batch.FromDate,
                        PeriodTo = batch.ToDate,
                        Total = lineTotal
                    };

                    await _invoiceRepo.AddAsync(invoice).ConfigureAwait(false);
                    grandTotal = grandTotal.Add(lineTotal);
                }

                batch.InvoiceCount = batch.CustomerIds.Count;
                batch.GrandTotal = grandTotal;
                batch.Status = BatchStatus.Completed;
                batch.CompletedAt = DateTime.UtcNow;
                await _batchRepo.UpdateAsync(batch).ConfigureAwait(false);

                await _eventPublisher
                    .PublishAsync(new { EventType = "InvoiceBatchCompleted", batch.BatchId, batch.InvoiceCount })
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                batch.Status = BatchStatus.Failed;
                batch.FailureReason = ex.Message;
                await _batchRepo.UpdateAsync(batch).ConfigureAwait(false);
            }

            return MapToSummary(batch);
        }

        public async Task<BatchSummaryDto> GetBatchAsync(Guid batchId)
        {
            var batch = await _batchRepo.GetByIdAsync(batchId).ConfigureAwait(false);
            if (batch == null)
                throw new KeyNotFoundException($"Batch {batchId} not found.");
            return MapToSummary(batch);
        }

        public async Task<IReadOnlyList<InvoiceDto>> ListInvoicesAsync(Guid batchId)
        {
            var batch = await _batchRepo.GetByIdAsync(batchId).ConfigureAwait(false);
            if (batch == null)
                throw new KeyNotFoundException($"Batch {batchId} not found.");

            var invoices = await _invoiceRepo.GetByBatchIdAsync(batchId).ConfigureAwait(false);

            return invoices
                .Select(inv => new InvoiceDto
                {
                    InvoiceId = inv.InvoiceId,
                    CustomerId = inv.CustomerId,
                    PeriodFrom = inv.PeriodFrom,
                    PeriodTo = inv.PeriodTo,
                    TotalAmount = inv.Total.Amount,
                    Currency = inv.Total.Currency,
                    GeneratedAt = inv.GeneratedAt
                })
                .ToList()
                .AsReadOnly();
        }

        private static BatchSummaryDto MapToSummary(InvoiceBatch batch)
        {
            return new BatchSummaryDto
            {
                BatchId = batch.BatchId,
                Status = batch.Status.ToString(),
                InvoiceCount = batch.InvoiceCount,
                GrandTotalAmount = batch.GrandTotal?.Amount ?? 0m,
                Currency = batch.GrandTotal?.Currency ?? "USD",
                RequestedBy = batch.RequestedBy,
                CreatedAt = batch.CreatedAt,
                CompletedAt = batch.CompletedAt,
                FailureReason = batch.FailureReason
            };
        }
    }
}
