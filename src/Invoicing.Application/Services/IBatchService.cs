using System;
using System.Collections.Generic;
using Contoso.Invoicing.Application.Dtos;

namespace Contoso.Invoicing.Application.Services
{
    public interface IBatchService
    {
        BatchSummaryDto CreateBatch(CreateBatchRequest request);
        BatchSummaryDto RunBatch(Guid batchId);
        BatchSummaryDto GetBatch(Guid batchId);
        IReadOnlyList<InvoiceDto> ListInvoices(Guid batchId);
    }
}
