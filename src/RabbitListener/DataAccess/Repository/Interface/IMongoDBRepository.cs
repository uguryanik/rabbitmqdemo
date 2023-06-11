using Models.Entity;

using MongoDB.Driver;

namespace RabbitListener.DataAccess.Repository.Interface
{
    public interface IMongoDBRepository
    {
        Task InsertLog(LogModel log);
    }
}
