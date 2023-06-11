using Models.Entity;
using Models;

using MongoDB.Driver;

using RabbitListener.DataAccess.Repository.Interface;

namespace RabbitListener.DataAccess.Repository
{
    public class MongoClientWrapper : IMongoClientWrapper
    {
        private readonly IMongoClient _client;

        public MongoClientWrapper(string connectionString)
        {
            _client = new MongoClient(connectionString);
        }

        public IMongoDatabaseWrapper GetDatabase(string databaseName)
        {
            return new MongoDatabaseWrapper(_client.GetDatabase(databaseName));
        }
    }

    public class MongoDatabaseWrapper : IMongoDatabaseWrapper
    {
        private readonly IMongoDatabase _database;

        public MongoDatabaseWrapper(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

    public class MongoDBRepository : IMongoDBRepository
    {
        private readonly IMongoCollection<LogModel> _logCollection;
        private IMongoClientWrapper _client;

        public MongoDBRepository(MongoDBSettings settings, IMongoClientWrapper client)
        {
            var _client = client;

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
