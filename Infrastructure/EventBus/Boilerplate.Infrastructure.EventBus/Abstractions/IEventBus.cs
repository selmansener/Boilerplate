using Boilerplate.Infrastructure.EventBus.Events;

namespace Boilerplate.Infrastructure.EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(BaseEvent @event);

        Task PublishAsync(BaseEvent @event, CancellationToken cancellationToken = default);

        void Subscribe<T, TH>()
            where T : BaseEvent
            where TH : IEventHandler<T>;

        void Unsubscribe<T, TH>()
            where TH : IEventHandler<T>
            where T : BaseEvent;
    }
}