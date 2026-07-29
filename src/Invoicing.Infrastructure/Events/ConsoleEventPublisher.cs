using System;
using System.Threading.Tasks;
using Contoso.Invoicing.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Contoso.Invoicing.Infrastructure.Events
{
    public class ConsoleEventPublisher : IEventPublisher
    {
        private readonly ILogger<ConsoleEventPublisher> _logger;

        public ConsoleEventPublisher(ILogger<ConsoleEventPublisher> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task PublishAsync<T>(T @event) where T : class
        {
            _logger.LogInformation(
                "[EVENT] {PublishedAt:O} | {EventName} | {Event}",
                DateTime.UtcNow,
                typeof(T).Name,
                @event);
            return Task.CompletedTask;
        }
    }
}
