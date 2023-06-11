using Microsoft.Extensions.Options;

using Models;
using Models.Entity;

using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitProducer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly List<string> _urls;
        private readonly HttpClient _httpClient;

        public Worker(ILogger<Worker> logger, HttpClient httpClient, IOptions<RabbitMQSettings> rabbitMqOptions, IConnection connection, List<string> urls)
        {
            _logger = logger;
            _channel = connection.CreateModel();
            _queueName = rabbitMqOptions.Value.QueueName;
            _urls = urls;
            _httpClient = httpClient;

            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var httpClient = new HttpClient();

                foreach (var url in _urls)
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);

                        var log = new LogModel
                        {
                            ServiceName = "RabbitProducer",
                            Url = url,
                            StatusCode = (int)response.StatusCode
                        };

                        var message = JsonConvert.SerializeObject(log);

                        _channel.BasicPublish(exchange: "",
                                              routingKey: _queueName,
                                              basicProperties: null,
                                              body: Encoding.UTF8.GetBytes(message));

                        _logger.LogInformation($"Message sent: {message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An error occurred while sending request to {url}");
                    }
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}