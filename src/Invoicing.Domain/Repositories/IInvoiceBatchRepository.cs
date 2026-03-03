using System;
using Contoso.Invoicing.Domain.Models;

namespace Contoso.Invoicing.Domain.Repositories
{
    public interface IInvoiceBatchRepository
    {
        void Add(InvoiceBatch batch);
        InvoiceBatch GetById(Guid batchId);
        void Update(InvoiceBatch batch);
    }
}
