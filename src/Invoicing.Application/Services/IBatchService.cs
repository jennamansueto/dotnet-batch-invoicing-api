using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Invoicing.Application.Dtos;

namespace Contoso.Invoicing.Application.Services
{
    public interface IBatchService
    {
        Task<BatchSummaryDto> CreateBatchAsync(CreateBatchRequest request);
        Task<BatchSummaryDto> RunBatchAsync(Guid batchId);
        Task<BatchSummaryDto> GetBatchAsync(Guid batchId);
        Task<IReadOnlyList<InvoiceDto>> ListInvoicesAsync(Guid batchId);
    }
}
