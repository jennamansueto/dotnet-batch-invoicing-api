namespace Contoso.Invoicing.Api.Configuration
{
    public class InvoicingOptions
    {
        public const string SectionName = "Invoicing";

        public string ApiKey { get; set; }
        public string ApplicationName { get; set; }
        public string Environment { get; set; }
    }
}
