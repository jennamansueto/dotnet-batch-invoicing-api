using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Contoso.Invoicing.Domain.Models;
using Contoso.Invoicing.Domain.Repositories;

namespace Contoso.Invoicing.Infrastructure.Repositories
{
    public class InMemoryInvoiceBatchRepository : IInvoiceBatchRepository
    {
        private static readonly ConcurrentDictionary<Guid, InvoiceBatch> Store =
            new ConcurrentDictionary<Guid, InvoiceBatch>();

        public Task AddAsync(InvoiceBatch batch)
        {
            if (batch == null) throw new ArgumentNullException(nameof(batch));
            Store[batch.BatchId] = batch;
            return Task.CompletedTask;
        }

        public Task<InvoiceBatch> GetByIdAsync(Guid batchId)
        {
            Store.TryGetValue(batchId, out var batch);
            return Task.FromResult(batch);
        }

        public Task UpdateAsync(InvoiceBatch batch)
        {
            if (batch == null) throw new ArgumentNullException(nameof(batch));
            Store[batch.BatchId] = batch;
            return Task.CompletedTask;
        }
    }
}
