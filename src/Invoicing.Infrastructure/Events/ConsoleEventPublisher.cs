using System;
using System.Diagnostics;
using Contoso.Invoicing.Domain.Events;

namespace Contoso.Invoicing.Infrastructure.Events
{
    public class ConsoleEventPublisher : IEventPublisher
    {
        public void Publish<T>(T @event) where T : class
        {
            var message = $"[EVENT] {DateTime.UtcNow:O} | {typeof(T).Name} | {@event}";
            Trace.TraceInformation(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
