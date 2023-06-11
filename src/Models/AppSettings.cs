using System.Diagnostics.CodeAnalysis;

namespace Models
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public RabbitMQSettings RabbitMQ { get; set; }
        public List<string> Urls { get; set; }

        public ElasticSettings ElasticSettings { get; set; }
    }
}
