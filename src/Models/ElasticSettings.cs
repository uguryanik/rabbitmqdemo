using System.Diagnostics.CodeAnalysis;

namespace Models
{
    [ExcludeFromCodeCoverage]
    public class ElasticSettings
    {
        public string ElasticUrl { get; set; } = "";

        public string ElasticUsername { get; set; } = "";

        public string ElasticPassword { get; set; } = "";
    }
}
