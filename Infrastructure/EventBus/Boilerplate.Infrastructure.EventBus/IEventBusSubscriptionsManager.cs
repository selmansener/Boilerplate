using Boilerplate.Infrastructure.EventBus.Abstractions;
using Boilerplate.Infrastructure.EventBus.Events;
using System;
using System.Collections.Generic;

namespace Boilerplate.Infrastructure.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void AddSubscription<T, TH>()
            where T : BaseEvent
            where TH : IEventHandler<T>;

        void RemoveSubscription<T, TH>()
                where TH : IEventHandler<T>
                where T : BaseEvent;
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionsForEvent<T>() where T : BaseEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : BaseEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}