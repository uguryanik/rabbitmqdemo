using Xunit;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using RabbitListener.Services.Interface;
using System.Text;
using Models.Entity;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RabbitListener.DataAccess.Repository.Interface;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Options;
using Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System;

namespace RabbitListener.Tests
{
    public class WorkerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldSendCorrectRequests()
        {
            var _mockClientWrapper = new Mock<IMongoClientWrapper>();
            var _mockDatabaseWrapper = new Mock<IMongoDatabaseWrapper>();

            var loggerMock = new Mock<ILogger<Worker>>();
            var logServiceMock = new Mock<ILogService>();
            var connectionMock = new Mock<IConnection>();
            var modelMock = new Mock<IModel>();

            var consumerMock = new Mock<EventingBasicConsumer>(modelMock.Object);
            modelMock.Setup(m => m.DefaultConsumer).Returns(consumerMock.Object); // Set up DefaultConsumer

            var testLog = new LogModel { ServiceName = "RabbitProducer", Url = "http://google.com", StatusCode = 200 };

            connectionMock.Setup(m => m.CreateModel()).Returns(modelMock.Object);

            var worker = new Worker(
                loggerMock.Object,
                logServiceMock.Object,
                connectionMock.Object,
                _mockDatabaseWrapper.Object
                );


            // Act
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