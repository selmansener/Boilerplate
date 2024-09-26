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
            CurrentData = new Dictionary<string, object?>();
            OrijinalData = new Dictionary<string, object?>();
        }

        public Guid EventId { get; private set; }

        public string EventSource { get; protected set; }

        public string EventName { get; protected set; }

        public Dictionary<string, object?> OrijinalData { get; private set; }

        public Dictionary<string, object?> CurrentData { get; private set; }

        [JsonPropertyName("@timestamp")]
        public DateTime Timestamp { get; private set; }

        public void AddField(string key, object? currentValue)
        {
            AddField(key, currentValue, null);
        }

        public void AddField(string key,  object? currentValue, object? originalValue)
        {
            if (CurrentData.ContainsKey(key) || OrijinalData.ContainsKey(key))
            {
                // TODO: should we throw?
                return;
            }

            CurrentData.Add(key, currentValue);
            if (originalValue != null)
            {
                OrijinalData.Add(key, originalValue);
            }
        }
    }
}
