using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Boilerplate.API.HealthCheck
{
    internal static class HealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder ForwardToPrometheus(this IHealthChecksBuilder builder, PrometheusHealthCheckPublisherOptions? options = null)
        {
            builder.Services.AddSingleton<IHealthCheckPublisher, PrometheusHealthCheckPublisher>(provider => new PrometheusHealthCheckPublisher(options));

            return builder;
        }
    }
    internal sealed class PrometheusHealthCheckPublisherOptions
    {
        private const string DefaultName = "aspnetcore_healthcheck_status";
        private const string DefaultHelp = "ASP.NET Core health check status (0 == Unhealthy, 0.5 == Degraded, 1 == Healthy)";

        public Gauge Gauge { get; set; } =
            Metrics.CreateGauge(DefaultName, DefaultHelp, labelNames: new[] { "name" });
    }

    internal sealed class PrometheusHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly Gauge _checkStatus;

        public PrometheusHealthCheckPublisher(PrometheusHealthCheckPublisherOptions? options)
        {
            _checkStatus = options?.Gauge ?? new PrometheusHealthCheckPublisherOptions().Gauge;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            foreach (var reportEntry in report.Entries)
                _checkStatus.WithLabels(reportEntry.Key).Set(HealthStatusToMetricValue(reportEntry.Value.Status));

            return Task.CompletedTask;
        }

        private static double HealthStatusToMetricValue(HealthStatus status)
        {
            switch (status)
            {
                case HealthStatus.Unhealthy:
                    return 0;
                case HealthStatus.Degraded:
                    return 0.5;
                case HealthStatus.Healthy:
                    return 1;
                default:
                    throw new NotSupportedException($"Unexpected HealthStatus value: {status}");
            }
        }
    }
    internal sealed class SampleHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // All is well!
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
