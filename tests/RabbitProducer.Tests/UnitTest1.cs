using Xunit;
using Moq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Models;

namespace RabbitProducer.Tests
{
    public class WorkerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldSendCorrectRequests()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<Worker>>();
            var optionsMock = new Mock<IOptions<RabbitMQSettings>>();
            optionsMock.Setup(o => o.Value).Returns(new RabbitMQSettings { QueueName = "queueName" });
            var connectionMock = new Mock<IConnection>();
            var modelMock = new Mock<IModel>();
            connectionMock.Setup(c => c.CreateModel()).Returns(modelMock.Object);

            var urls = new List<string> { "http://test1.com", "http://test2.com" };
            var httpClientMock = new Mock<HttpClient>() { CallBase = true };

            var worker = new Worker(loggerMock.Object,
                httpClientMock.Object,
                optionsMock.Object, 
                //optionsMock.Object, 
                connectionMock.Object, 
                urls
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