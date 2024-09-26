using System.Reflection;

using DynamicQueryBuilder.Models;

using Boilerplate.Business.CQRS.InvoiceDomain.Commands;
using Boilerplate.Business.PipelineBehaviors;

using Mapster;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddSingleton(new DynamicQueryBuilderSettings
            {
                UsesCaseInsensitiveSource = true
            });
            // TODO: Denenicek
            services.AddTransient<IPipelineBehavior<CreateInvoiceCommand, Unit>, TransientHttpErrorsResilienceBehavior<CreateInvoiceCommand, Unit>>();

            TypeAdapterConfig.GlobalSettings.Scan(typeof(ServiceCollectionExtensions).Assembly);
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

            return services;
        }
    }
}
