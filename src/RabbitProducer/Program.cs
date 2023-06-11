using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Elasticsearch;

using Core;
using Models;
using System.Diagnostics.CodeAnalysis;

namespace RabbitProducer
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static string _elasticUrl = "";
        private static string _elasticUsername = "";
        private static string _elasticPassword = "";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    var rabbitMQSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
                    services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

                    services.AddSingleton<List<string>>(provider =>
                    {
                        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
                        var urls = configuration.GetSection("Urls").Get<List<string>>();
                        return urls;
                    });

                    var factory = new ConnectionFactory()
                    {
                        Uri = new Uri(rabbitMQSettings.Hostname)
                    };

                    IConnection connection = factory.CreateConnection();
                    services.AddSingleton(connection);
                    services.AddHttpClient();
                    services.AddHostedService<Worker>();

                    var elasticSettings = configuration.GetSection("ElasticSettings").Get<ElasticSettings>();
                    _elasticUrl = elasticSettings.ElasticUrl;
                    _elasticUsername = elasticSettings.ElasticUsername;
                    _elasticPassword = elasticSettings.ElasticPassword;
                })
                .UseElasticSearchLogging(serviceName: "RabbitProducer",
                elasticUrl: _elasticUrl,
                elasticUsername: _elasticUsername,
                elasticPassword: _elasticPassword)
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(_elasticUrl))
                    {
                        ModifyConnectionSettings = x => x.BasicAuthentication(_elasticUsername, _elasticPassword),
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog-events-template",
                        IndexFormat = "uguryanik-log-{0:yyyy.MM.dd}"
                    }));
    }
}
