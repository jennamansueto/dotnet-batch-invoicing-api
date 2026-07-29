using System.Threading.Tasks;

namespace Contoso.Invoicing.Domain.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
