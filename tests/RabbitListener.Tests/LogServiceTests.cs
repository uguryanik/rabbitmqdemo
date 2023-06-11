using System.Threading.Tasks;
using Moq;
using RabbitListener.Services;
using RabbitListener.DataAccess.Repository.Interface;
using Models.Entity;
using Xunit;

namespace RabbitListener.Tests
{
    public class LogServiceTests
    {
        [Fact]
        public async Task InsertLog_Should_Call_Repository_With_Correct_Log()
        {
            // Arrange
            var repositoryMock = new Mock<IMongoDBRepository>();
            var _mockMongoClientWrapper = new Mock<IMongoClientWrapper>();
            var logService = new LogService(repositoryMock.Object, _mockMongoClientWrapper.Object);

            var testLog = new LogModel { ServiceName = "RabbitProducer", Url = "http://test.url", StatusCode = 200 };

            // Act
            await logService.InsertLog(testLog);

            // Assert
            repositoryMock.Verify(r => r.InsertLog(It.Is<LogModel>(log => log.Url == testLog.Url)), Times.Once);
        }
    }
}
