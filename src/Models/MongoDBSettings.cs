using System.Diagnostics.CodeAnalysis;

namespace Models
{
    [ExcludeFromCodeCoverage]
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
