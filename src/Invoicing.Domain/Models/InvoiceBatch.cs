using System;
using System.Collections.Generic;

namespace Contoso.Invoicing.Domain.Models
{
    public class InvoiceBatch
    {
        public Guid BatchId { get; set; }
        public List<string> CustomerIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string RequestedBy { get; set; }
        public BatchStatus Status { get; set; }
        public int InvoiceCount { get; set; }
        public Money GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string FailureReason { get; set; }

        public InvoiceBatch()
        {
            BatchId = Guid.NewGuid();
            CustomerIds = new List<string>();
            Status = BatchStatus.Pending;
            GrandTotal = Money.Zero();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
