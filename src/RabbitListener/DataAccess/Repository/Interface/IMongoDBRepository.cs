using Models.Entity;

using MongoDB.Driver;

namespace RabbitListener.DataAccess.Repository.Interface
{
    public interface IMongoClientWrapper
    {
        IMongoDatabaseWrapper GetDatabase(string databaseName);
    }

    public interface IMongoDatabaseWrapper
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }

    public interface IMongoDBRepository
    {
        Task InsertLog(LogModel log);
    }
}
