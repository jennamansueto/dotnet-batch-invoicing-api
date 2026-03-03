using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Contoso.Invoicing.Domain.Models;
using Contoso.Invoicing.Domain.Repositories;

namespace Contoso.Invoicing.Infrastructure.Repositories
{
    public class InMemoryInvoiceRepository : IInvoiceRepository
    {
        private static readonly ConcurrentBag<Invoice> Store = new ConcurrentBag<Invoice>();

        public void Add(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            Store.Add(invoice);
        }

        public IReadOnlyList<Invoice> GetByBatchId(Guid batchId)
        {
            return Store.Where(i => i.BatchId == batchId).ToList().AsReadOnly();
        }
    }
}
