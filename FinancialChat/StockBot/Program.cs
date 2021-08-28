using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using StockBot.AlphaVantage;

namespace StockBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting StockBot...");

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add services here
                    services.Configure<StockCommandOptions>(
                        context.Configuration.GetSection(StockCommandOptions.AlphaVantageApi));

                    services.AddSingleton<IConnectionProvider>(
                        new ConnectionProvider("amqp://guest@localhost:5672"));

                    services.AddScoped<IPublisher>(x => new Publisher(
                        x.GetService<IConnectionProvider>(),
                        "stockbot-report-exchange",
                        ExchangeType.Direct));

                    services.AddScoped<ISubscriber>(x => new Subscriber(
                        x.GetService<IConnectionProvider>(),
                        "stockbot-request-exchange",
                        "stockbot-request-queue",
                        "stockbot-request",
                        ExchangeType.Direct));

                    services.AddHostedService<StockBot>();
                })
                .Build();

            // Publish message as test
            //var publisher = ActivatorUtilities.GetServiceOrCreateInstance<IPublisher>(host.Services);
            //publisher.Publish("Hello everyone", "stockbot-request", null);

            host.Run();
        }

        public static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }
    }
}
//