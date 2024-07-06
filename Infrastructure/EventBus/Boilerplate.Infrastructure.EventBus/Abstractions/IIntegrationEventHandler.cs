using Boilerplate.Infrastructure.EventBus.Events;
using System.Threading.Tasks;

namespace Boilerplate.Infrastructure.EventBus.Abstractions
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