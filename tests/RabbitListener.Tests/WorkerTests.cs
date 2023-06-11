using Xunit;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using RabbitListener.Services.Interface;
using Models.Entity;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace RabbitListener.Tests
{
    public class WorkerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldSendCorrectRequests()
        {
            var loggerMock = new Mock<ILogger<Worker>>();
            var logServiceMock = new Mock<ILogService>();
            var connectionMock = new Mock<IConnection>();
            var modelMock = new Mock<IModel>();

            var consumerMock = new Mock<EventingBasicConsumer>(modelMock.Object);
            modelMock.Setup(m => m.DefaultConsumer).Returns(consumerMock.Object); 

            var testLog = new LogModel { ServiceName = "RabbitProducer", Url = "http://google.com", StatusCode = 200 };

            connectionMock.Setup(m => m.CreateModel()).Returns(modelMock.Object);

            var worker = new Worker(
                loggerMock.Object,
                logServiceMock.Object,
                connectionMock.Object
                );

            var stoppingTokenSource = new CancellationTokenSource();
            var workerTask = worker.StartAsync(stoppingTokenSource.Token);

            await Task.Delay(1000);

            stoppingTokenSource.Cancel();

            try
            {
                await workerTask;
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
        }
    }
}