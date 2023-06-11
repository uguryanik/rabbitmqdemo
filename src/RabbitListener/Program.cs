using RabbitMQ.Client;

using Models;
using RabbitListener.DataAccess.Repository.Interface;
using RabbitListener.DataAccess.Repository;
using RabbitListener.Services.Interface;
using RabbitListener.Services;
using Core;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Elasticsearch;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace RabbitListener
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

                    var mongoDBSettings = configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
                    services.Configure<MongoDBSettings>(configuration.GetSection("MongoDBSettings"));

                    services.AddSingleton<MongoDBSettings>(sp =>
                        sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);


                    services.AddSingleton<IMongoClient>(sp => { return new MongoClient(mongoDBSettings.ConnectionString); });

                    services.AddSingleton<IMongoDBRepository, MongoDBRepository>();

                    var rabbitMQSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
                    services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

                    var factory = new ConnectionFactory()
                    {
                        Uri = new Uri(rabbitMQSettings.Hostname)
                    };
                    IConnection connection = factory.CreateConnection();
                    services.AddSingleton(connection);

                    services.AddSingleton<ILogService, LogService>();

                    services.AddHostedService<Worker>();

                    var elasticSettings = configuration.GetSection("ElasticSettings").Get<ElasticSettings>();
                    _elasticUrl = elasticSettings.ElasticUrl;
                    _elasticUsername = elasticSettings.ElasticUsername;
                    _elasticPassword = elasticSettings.ElasticPassword;

                }).UseElasticSearchLogging(serviceName: "RabbitListener",
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
