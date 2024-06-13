using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace InvoiceFetcher.Infrastructure.EventSourcing
{
    public interface IEventSourceContext
    {
        Task InsertEvents(EventCollection events);
    }

    internal class EventSourceContext : IEventSourceContext
    {
        // TODO: consider moving elastic client to InvoiceFetcher.Infrastructure.EventSourcing.Elasticsearch
        private readonly ElasticsearchClient _elasticsearchClient;

        public EventSourceContext(EventSourcingOptions eventSourcingOptions)
        {
            var settings = new ElasticsearchClientSettings(new Uri(eventSourcingOptions.ElasticHostUrl))
                .CertificateFingerprint(eventSourcingOptions.CertificateFingerprint)
                // TODO: add api key option
                .Authentication(new BasicAuthentication(eventSourcingOptions.Username, eventSourcingOptions.Password))
                // TODO: add index name option
                // TODO: consider time format for index name
                .DefaultMappingFor<EventBase>(i => i.IndexName("event-sourcing"));

            // TODO: consider scoped or singleton elasticsearch client?
            _elasticsearchClient = new ElasticsearchClient(settings);
        }

        public async Task InsertEvents(EventCollection events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            await Task.Factory.StartNew((eventsObj) =>
            {
                EventCollection _events = (EventCollection)eventsObj;

                while (_events.TryPeek(out _))
                {
                    var @event = _events.Dequeue();

                    if (@event == null)
                    {
                        break;
                    }
                    _elasticsearchClient.Index(@event);
                }

            }, events);
        }
    }
}
