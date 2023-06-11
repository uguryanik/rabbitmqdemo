using System.Threading.Tasks;
using Moq;
using RabbitListener.DataAccess.Repository;
using Models.Entity;
using Xunit;
using MongoDB.Driver;
using System;
using Models;
using RabbitListener.DataAccess.Repository.Interface;

namespace RabbitListener.Tests
{
    public class MongoDBRepositoryTests
    {
        private readonly Mock<IMongoCollection<LogModel>> _mockCollection;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDBRepository> _mockRepository;

        public MongoDBRepositoryTests()
        {
            _mockClient = new Mock<IMongoClient>();
            _mockRepository = new Mock<IMongoDBRepository>();
            _mockClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null)).Returns(Mock.Of<IMongoDatabase>());
            _mockRepository.Setup(x => x.InsertLog(It.IsAny<LogModel>())).Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task InsertLog_Should_Call_Collection_With_Correct_Log()
        {
            var log = new LogModel() { ServiceName = "RabbitProducer", Url = "google.com", StatusCode = 200 };

            await _mockRepository.Object.InsertLog(log);

            _mockRepository.Verify(x => x.InsertLog(log), Times.Once);
        }
    }
}