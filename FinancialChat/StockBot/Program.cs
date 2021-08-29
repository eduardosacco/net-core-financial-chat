using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using StockBot.AlphaVantage;
using StockBot.Stooq;

namespace StockBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting StockBot...");

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add services here
                    services.Configure<StockBotOptions>(
                        context.Configuration.GetSection(StockBotOptions.Key));

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

                    services.AddScoped<IAlphaVantage, AlphaVantage.AlphaVantage>();
                    services.AddScoped<IStooq, Stooq.Stooq>();

                    services.AddHostedService<StockBot>();
                })
                .Build()
                .Run();
        }

        public static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }
    }
}
//