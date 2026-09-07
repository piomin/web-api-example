using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace web_api_example
{
    public class RabbitMqService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqService> _logger;
        private IConnection _connection;

        public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "rabbitmq",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? "dotnet",
                Password = _configuration["RabbitMQ:Password"] ?? "dotnet123",
                VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/"
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}", factory.HostName, factory.Port);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_connection is not null)
                await _connection.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
