using System.Web.Http;
using Contoso.Invoicing.Api.App_Start;
using Contoso.Invoicing.Api.Middleware;
using Contoso.Invoicing.Application.Services;
using Contoso.Invoicing.Domain.Events;
using Contoso.Invoicing.Domain.Repositories;
using Contoso.Invoicing.Infrastructure.Events;
using Contoso.Invoicing.Infrastructure.Repositories;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Contoso.Invoicing.Api.Startup))]

namespace Contoso.Invoicing.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);

            WireUpDependencies(config);

            app.Use<ApiKeyMiddleware>();

            app.UseWebApi(config);
        }

        private static void WireUpDependencies(HttpConfiguration config)
        {
            var batchRepo = new InMemoryInvoiceBatchRepository();
            var invoiceRepo = new InMemoryInvoiceRepository();
            var eventPublisher = new ConsoleEventPublisher();
            var batchService = new BatchService(batchRepo, invoiceRepo, eventPublisher);

            config.Properties["IBatchService"] = batchService;
            config.Properties["IInvoiceBatchRepository"] = batchRepo;
            config.Properties["IInvoiceRepository"] = invoiceRepo;
            config.Properties["IEventPublisher"] = eventPublisher;
        }
    }
}
