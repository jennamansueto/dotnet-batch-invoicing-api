using Contoso.Invoicing.Application.Services;
using Contoso.Invoicing.Domain.Events;
using Contoso.Invoicing.Domain.Repositories;
using Contoso.Invoicing.Infrastructure.Events;
using Contoso.Invoicing.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Contoso.Invoicing.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInvoicingServices(this IServiceCollection services)
        {
            services.AddSingleton<IInvoiceBatchRepository, InMemoryInvoiceBatchRepository>();
            services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
            services.AddSingleton<IEventPublisher, ConsoleEventPublisher>();
            services.AddSingleton<IBatchService, BatchService>();
            return services;
        }
    }
}
