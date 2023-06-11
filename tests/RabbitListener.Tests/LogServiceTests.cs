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
            var repositoryMock = new Mock<IMongoDBRepository>();
            var logService = new LogService(repositoryMock.Object);

            var testLog = new LogModel { ServiceName = "RabbitProducer", Url = "http://test.url", StatusCode = 200 };

            await logService.InsertLog(testLog);

            repositoryMock.Verify(r => r.InsertLog(It.Is<LogModel>(log => log.Url == testLog.Url)), Times.Once);
        }
    }
}
