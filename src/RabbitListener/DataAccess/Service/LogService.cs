using Models.Entity;

using RabbitListener.DataAccess.Repository.Interface;
using RabbitListener.Services.Interface;

namespace RabbitListener.Services
{
    public class LogService : ILogService
    {
        private readonly IMongoDBRepository _repository;

        public LogService(IMongoDBRepository repository)
        {
            _repository = repository;
        }

        public async Task InsertLog(LogModel log)
        {
            await _repository.InsertLog(log);
        }
    }
}
