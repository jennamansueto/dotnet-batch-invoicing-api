using System;

namespace Contoso.Invoicing.Application.Dtos
{
    public class BatchSummaryDto
    {
        public Guid BatchId { get; set; }
        public string Status { get; set; }
        public int InvoiceCount { get; set; }
        public decimal GrandTotalAmount { get; set; }
        public string Currency { get; set; }
        public string RequestedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string FailureReason { get; set; }
    }
}
