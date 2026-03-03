using System;
using System.Collections.Concurrent;
using Contoso.Invoicing.Domain.Models;
using Contoso.Invoicing.Domain.Repositories;

namespace Contoso.Invoicing.Infrastructure.Repositories
{
    public class InMemoryInvoiceBatchRepository : IInvoiceBatchRepository
    {
        private static readonly ConcurrentDictionary<Guid, InvoiceBatch> Store =
            new ConcurrentDictionary<Guid, InvoiceBatch>();

        public void Add(InvoiceBatch batch)
        {
            if (batch == null) throw new ArgumentNullException(nameof(batch));
            Store[batch.BatchId] = batch;
        }

        public InvoiceBatch GetById(Guid batchId)
        {
            Store.TryGetValue(batchId, out var batch);
            return batch;
        }

        public void Update(InvoiceBatch batch)
        {
            if (batch == null) throw new ArgumentNullException(nameof(batch));
            Store[batch.BatchId] = batch;
        }
    }
}
