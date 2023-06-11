using System.Threading.Tasks;
using Moq;
using RabbitListener.DataAccess.Repository;
using Models.Entity;
using MongoDB.Driver;
using Xunit;
using Models;
using RabbitListener.DataAccess.Repository.Interface;

namespace RabbitListener.Tests
{
    public class MongoDBRepositoryTests
    {
        private readonly Mock<IMongoClientWrapper> _mockClientWrapper;
        private readonly Mock<IMongoDatabaseWrapper> _mockDatabaseWrapper;
        private readonly Mock<IMongoCollection<LogModel>> _mockCollection;

        private readonly MongoDBRepository _repository;

        public MongoDBRepositoryTests()
        {
            _mockClientWrapper = new Mock<IMongoClientWrapper>();
            _mockDatabaseWrapper = new Mock<IMongoDatabaseWrapper>();
            _mockCollection = new Mock<IMongoCollection<LogModel>>();

            _mockDatabaseWrapper.Setup(x => x.GetCollection<LogModel>(It.IsAny<string>()))
                .Returns(_mockCollection.Object);

            _mockClientWrapper.Setup(x => x.GetDatabase(It.IsAny<string>()))
                .Returns(_mockDatabaseWrapper.Object);

            _repository = new MongoDBRepository(new MongoDBSettings() { ConnectionString = "test", DatabaseName = "testDB", CollectionName = "testCollection" }, _mockClientWrapper.Object);
        }

        [Fact]
        public async Task InsertLog_Should_Call_Collection_With_Correct_Log()
        {
            // Arrange
            var log = new LogModel() { ServiceName = "RabbitProducer", Url = "google.com", StatusCode = 200 };

            // Act
            await _repository.InsertLog(log);

            // Assert
            _mockCollection.Verify(x => x.InsertOneAsync(log, null, default), Times.Once);
        }
    }

}