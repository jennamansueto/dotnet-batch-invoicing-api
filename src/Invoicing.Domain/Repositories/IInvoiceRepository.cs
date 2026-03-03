using System;
using System.Collections.Generic;
using Contoso.Invoicing.Domain.Models;

namespace Contoso.Invoicing.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        void Add(Invoice invoice);
        IReadOnlyList<Invoice> GetByBatchId(Guid batchId);
    }
}
