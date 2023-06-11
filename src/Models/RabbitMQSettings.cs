using System.Diagnostics.CodeAnalysis;

namespace Models
{
    [ExcludeFromCodeCoverage]
    public class RabbitMQSettings
    {
        public string Hostname { get; set; }
        public string QueueName { get; set; }
    }
}
