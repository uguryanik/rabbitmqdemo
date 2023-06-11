using Models.Entity;
using Models;

using MongoDB.Driver;

using RabbitListener.DataAccess.Repository.Interface;

namespace RabbitListener.DataAccess.Repository
{
    public class MongoDBRepository : IMongoDBRepository
    {
        private readonly IMongoCollection<LogModel> _logCollection;
        private IMongoClient _client;

        public MongoDBRepository(MongoDBSettings settings, IMongoClient client)
        {
            _client = client;

            if(_client != null)
            {
                var database = _client.GetDatabase(settings.DatabaseName);
                _logCollection = database.GetCollection<LogModel>(settings.CollectionName);
            }
            else
            {
                throw new Exception("mongodb client is null");
            }
        }

        public async Task InsertLog(LogModel log)
        {
            await _logCollection.InsertOneAsync(log);
        }
    }
}
