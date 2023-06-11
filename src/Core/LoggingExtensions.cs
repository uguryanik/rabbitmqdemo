using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

using System.Diagnostics.CodeAnalysis;

namespace Core
{
    [ExcludeFromCodeCoverage]
    public static class LoggingExtensions
    {
        public static IHostBuilder UseElasticSearchLogging(this IHostBuilder hostBuilder, string serviceName, string elasticUrl, string elasticUsername, string elasticPassword)
        => hostBuilder
            .UseSerilog((ctx, ltx) => ltx
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", serviceName)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(
            new Uri(elasticUrl))
        {
            ModifyConnectionSettings = x => x.BasicAuthentication(elasticUsername, elasticPassword),
            CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
            AutoRegisterTemplate = true,
            TemplateName = "serilog-events-template",
            IndexFormat = "uguryanik-log-{0:yyyy.MM.dd}"
        }));
    }
}