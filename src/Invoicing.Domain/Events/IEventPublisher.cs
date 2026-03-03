namespace Contoso.Invoicing.Domain.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : class;
    }
}
