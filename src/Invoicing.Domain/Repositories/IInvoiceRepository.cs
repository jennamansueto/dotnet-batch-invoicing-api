using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Invoicing.Domain.Models;

namespace Contoso.Invoicing.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);
        Task<IReadOnlyList<Invoice>> GetByBatchIdAsync(Guid batchId);
    }
}
