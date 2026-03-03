using System;

namespace Contoso.Invoicing.Application.Dtos
{
    public class InvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public string CustomerId { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
