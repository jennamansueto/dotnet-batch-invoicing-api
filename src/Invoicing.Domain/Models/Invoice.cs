using System;

namespace Contoso.Invoicing.Domain.Models
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid BatchId { get; set; }
        public string CustomerId { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public Money Total { get; set; }
        public DateTime GeneratedAt { get; set; }

        public Invoice()
        {
            InvoiceId = Guid.NewGuid();
            GeneratedAt = DateTime.UtcNow;
        }
    }
}
