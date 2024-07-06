using System.Text.Json.Serialization;

namespace Boilerplate.Infrastructure.EventSourcing
{
    public class EventBase
    {
        public EventBase(string eventSource, string eventName)
        {
            EventId = Guid.NewGuid();
            EventSource = eventSource;
            EventName = eventName;
            Timestamp = DateTime.UtcNow;
            Data = new Dictionary<string, object?>();
        }

        public Guid EventId { get; private set; }

        public string EventSource { get; protected set; }

        public string EventName { get; protected set; }

        public Dictionary<string, object?> Data { get; private set; }

        [JsonPropertyName("@timestamp")]
        public DateTime Timestamp { get; private set; }

        public void AddField(string key,  object? value)
        {
            if (Data.ContainsKey(key))
            {
                // TODO: should we throw?
                return;
            }

            Data.Add(key, value);
        }
    }
}
