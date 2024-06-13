using InvoiceFetcher.Infrastructure.EventBus.Events;
using System.Threading.Tasks;

namespace InvoiceFetcher.Infrastructure.EventBus.Abstractions
{
    public interface IEventHandler<in TIntegrationEvent> : IEventHandler
        where TIntegrationEvent : BaseEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IEventHandler
    {
    }
}