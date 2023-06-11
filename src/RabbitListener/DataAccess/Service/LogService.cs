using Models.Entity;

using RabbitListener.DataAccess.Repository.Interface;
using RabbitListener.Services.Interface;

namespace RabbitListener.Services
{
    public class LogService : ILogService
    {
        private readonly IMongoDBRepository _repository;
        private readonly IMongoClientWrapper _clientWrapper;

        public LogService(IMongoDBRepository repository, IMongoClientWrapper clientWrapper)
        {
            _repository = repository;
            _clientWrapper = clientWrapper;
        }

        public async Task InsertLog(LogModel log)
        {
            await _repository.InsertLog(log);
        }
    }
}
