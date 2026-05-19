using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contoso.Invoicing.Domain.Models;
using Contoso.Invoicing.Domain.Repositories;

namespace Contoso.Invoicing.Infrastructure.Repositories
{
    public class InMemoryInvoiceRepository : IInvoiceRepository
    {
        private static readonly ConcurrentBag<Invoice> Store = new ConcurrentBag<Invoice>();

        public Task AddAsync(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            Store.Add(invoice);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Invoice>> GetByBatchIdAsync(Guid batchId)
        {
            IReadOnlyList<Invoice> result = Store.Where(i => i.BatchId == batchId).ToList().AsReadOnly();
            return Task.FromResult(result);
        }
    }
}
