using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using RabbitListener.Services.Interface;

using Models.Entity;
using RabbitListener.DataAccess.Repository.Interface;

namespace RabbitListener
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILogService _logService;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "urls";
        private readonly IMongoDatabaseWrapper _databaseWrapper;

        public Worker(ILogger<Worker> logger, ILogService logService, IConnection connection, IMongoDatabaseWrapper databaseWrapper)
        {
            _databaseWrapper = databaseWrapper;
            _logger = logger;
            _logService = logService;
            _connection = connection;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.Span);
                var log = JsonConvert.DeserializeObject<LogModel>(content);

                try
                {
                    await _logService.InsertLog(log);
                    _logger.LogInformation($"Log inserted to MongoDB: {log}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while inserting log to MongoDB: {log}");
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(QueueName, false, consumer);

            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
