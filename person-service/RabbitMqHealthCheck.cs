using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace web_api_example
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly RabbitMqService _rabbitMqService;

        public RabbitMqHealthCheck(RabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_rabbitMqService.IsConnected
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("RabbitMQ connection is not open"));
        }
    }
}
