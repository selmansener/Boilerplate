using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure.EventSourcing
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSourcing(this IServiceCollection services, EventSourcingOptions eventSourcingOptions)
        {
            services.AddScoped<EventCollection>();
            services.AddScoped<IEventSourceContext, EventSourceContext>(sp => new EventSourceContext(eventSourcingOptions));

            return services;
        }
    }
}
