using System;
using System.Threading.Tasks;
using Contoso.Invoicing.Domain.Models;

namespace Contoso.Invoicing.Domain.Repositories
{
    public interface IInvoiceBatchRepository
    {
        Task AddAsync(InvoiceBatch batch);
        Task<InvoiceBatch> GetByIdAsync(Guid batchId);
        Task UpdateAsync(InvoiceBatch batch);
    }
}
