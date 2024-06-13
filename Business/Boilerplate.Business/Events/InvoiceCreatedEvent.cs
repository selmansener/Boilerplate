using InvoiceFetcher.Infrastructure.EventBus.Abstractions;
using InvoiceFetcher.Infrastructure.EventBus.Events;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace InvoiceFetcher.Business.Events
{
    public class InvoiceCreatedEvent : BaseEvent
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string CompanyIdentifier { get; set; }

        public DateTime ExternalCreatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // TODO: split handlers to their own file
    public class InvoiceCreatedEventHandler : IEventHandler<InvoiceCreatedEvent>
    {
        private readonly ILogger<InvoiceCreatedEventHandler> _logger;

        public InvoiceCreatedEventHandler(ILogger<InvoiceCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(InvoiceCreatedEvent @event)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(@event));

            await Task.CompletedTask;
        }
    }

    // TODO: split handlers to their own file
    public class InvoiceCreatedAnotherEventHandler : IEventHandler<InvoiceCreatedEvent>
    {
        private readonly ILogger<InvoiceCreatedAnotherEventHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public InvoiceCreatedAnotherEventHandler(ILogger<InvoiceCreatedAnotherEventHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Handle(InvoiceCreatedEvent @event)
        {
            _logger.LogWarning(JsonConvert.SerializeObject(@event));

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                await httpClient.GetAsync("https://google.com");
            }

            await Task.CompletedTask;
        }
    }

    // TODO: split handlers to their own file
    public class InvoiceCreatedOneMoreHandler : IEventHandler<InvoiceCreatedEvent>
    {
        public Task Handle(InvoiceCreatedEvent @event)
        {
            // To check if handlers exceptions handled in consumer
            throw new NotImplementedException();
        }
    }
}
