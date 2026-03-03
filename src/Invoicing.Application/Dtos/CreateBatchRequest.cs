using System;
using System.Collections.Generic;

namespace Contoso.Invoicing.Application.Dtos
{
    public class CreateBatchRequest
    {
        public List<string> CustomerIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string RequestedBy { get; set; }
    }
}
