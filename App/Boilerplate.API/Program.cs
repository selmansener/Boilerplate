
using FluentValidation;

using Boilerplate.API.Areas.Dev.Controllers;
using Boilerplate.API.Middlewares;
using Boilerplate.DataAccess.Extensions;
using Boilerplate.Business.Extensions;
using Boilerplate.Infrastructure.EventBusRabbitMQ;
using Boilerplate.Infrastructure.EventBus.Abstractions;
using Boilerplate.Business.Events;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Prometheus;
using Boilerplate.API.HealthCheck;
using Serilog.Events;
using Serilog.Enrichers.Sensitive;
using Boilerplate.API.Helpers;
using Boilerplate.API.Filters;
using Microsoft.Extensions.Options;
using Boilerplate.Shared.Interfaces;

namespace Boilerplate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var eventBusConfig = builder.Configuration.GetSection("EventBus").Get<EventBusConfig>();

            var configuration = builder.Configuration;

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithSensitiveDataMasking(opt =>
                {
                    opt.Mode = MaskingMode.Globally;

                    var sensitiveDataMasking = builder.Configuration.GetSection("SensitiveDataMasking").Get<string[]>();

                    if (sensitiveDataMasking == null)
                    {
                        // ignore rest
                        return;
                    }

                    foreach (var maskProperty in sensitiveDataMasking)
                    {
                        opt.MaskProperties.Add(maskProperty);
                    }

                    opt.MaskingOperators.Add(new JsonMaskingOperator(sensitiveDataMasking));
                })
                .WriteTo.Debug()
                    .WriteTo.Console()
                .Enrich.WithElasticApmCorrelationInfo()
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Debug, sinkConfiguration => sinkConfiguration.Elasticsearch(ConfigureElasticSink(configuration, builder.Environment.EnvironmentName, LogEventLevel.Debug)))
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Information, sinkConfiguration => sinkConfiguration.Elasticsearch(ConfigureElasticSink(configuration, builder.Environment.EnvironmentName, LogEventLevel.Information)))
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Warning, sinkConfiguration => sinkConfiguration.Elasticsearch(ConfigureElasticSink(configuration, builder.Environment.EnvironmentName, LogEventLevel.Warning)))
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Error, sinkConfiguration => sinkConfiguration.Elasticsearch(ConfigureElasticSink(configuration, builder.Environment.EnvironmentName, LogEventLevel.Error)))
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Fatal, sinkConfiguration => sinkConfiguration.Elasticsearch(ConfigureElasticSink(configuration, builder.Environment.EnvironmentName, LogEventLevel.Fatal)))
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            builder.Logging.AddSerilog(Log.Logger);

            builder.Host.UseSerilog(Log.Logger);

            // Add services to the container.
            builder.Services.AddValidatorsFromAssemblyContaining<SomeValidator>();
            builder.Services.AddProblemDetails();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddBusinessLayer();
            builder.Services.AddHttpClient();
            builder.Services.AddEventBus(eventBusConfig);
            builder.Services.AddHttpLogging(httpLoggingOptions =>
            {
                httpLoggingOptions.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                httpLoggingOptions.MediaTypeOptions.Clear();
                httpLoggingOptions.MediaTypeOptions.AddText("text/html");
                httpLoggingOptions.MediaTypeOptions.AddText("text/plain");
                httpLoggingOptions.MediaTypeOptions.AddText("application/json");
                httpLoggingOptions.MediaTypeOptions.AddText("multipart/form-data");
                httpLoggingOptions.MediaTypeOptions.AddText("application/x-www-form-urlencoded");
            });

            // TODO: Endpointler için circut breaker policy eklemek iyi olabilir?

            builder.Services.Scan(scan =>
                scan.FromAssemblyOf<InvoiceCreatedEventHandler>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                    .AsSelf()
                    .WithTransientLifetime()
            );

            // TODO: add area filter to prevent dev api calls for non-development environments
            builder.Services.AddSwaggerGen(options =>
            {
                options.OperationFilter<ResolveDynamicQueryEndpoints>("dqb");
            }).AddSwaggerGenNewtonsoftSupport();

            builder.Services.UseHttpClientMetrics();

            builder.Services.AddHealthChecks()
                .AddCheck<SampleHealthCheck>(nameof(SampleHealthCheck))
                .ForwardToPrometheus();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ITenant, TenantProvider>();

            builder.Services.AddDataAccess("Server=localhost;Database=Boilerplate;User Id=sa;Password=qwe123**;Encrypt=False;TrustServerCertificate=True;");

            var app = builder.Build();

            app.UseElasticApm(configuration,
                    new HttpDiagnosticsSubscriber(),
                    new EfCoreDiagnosticsSubscriber());

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpLogging();
            app.UseMiddleware<EnvironmentControlMiddleware>();

            app.UseHttpMetrics();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
            });

            app.MapControllers();

            app.AddEventSubscriptions();

            app.Run();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment, LogEventLevel logLevel)
        {
            var logLevelName = Enum.GetName(typeof(LogEventLevel), logLevel).ToLower();

            var indexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{logLevelName}-{DateTime.UtcNow:yyyy-MM}";

            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat,
                ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "qwe123**"),
                CustomFormatter = new EcsTextFormatter()
            };
        }
    }

    internal static class WebApplicationExtensions
    {
        internal static WebApplication AddEventSubscriptions(this WebApplication app)
        {
            var eventBus = app.Services.GetRequiredService<IEventBus>();

            eventBus.Subscribe<InvoiceCreatedEvent, InvoiceCreatedEventHandler>();
            eventBus.Subscribe<InvoiceCreatedEvent, InvoiceCreatedAnotherEventHandler>();
            eventBus.Subscribe<InvoiceCreatedEvent, InvoiceCreatedOneMoreHandler>();

            return app;
        }
    }
}