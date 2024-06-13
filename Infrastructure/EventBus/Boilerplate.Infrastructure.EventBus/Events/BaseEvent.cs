using System;
using System.Text.Json.Serialization;

namespace InvoiceFetcher.Infrastructure.EventBus.Events
{
    public abstract class BaseEvent
    {
        public BaseEvent()
        {
            EventId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public BaseEvent(Guid id, DateTime createDate)
        {
            EventId = id;
            CreationDate = createDate;
        }

        [JsonInclude]
        public Guid EventId { get; private init; }

        [JsonInclude]
        public DateTime CreationDate { get; private init; }
    }
}