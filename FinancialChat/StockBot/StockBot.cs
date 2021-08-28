using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;
using StockBot.AlphaVantage;

namespace StockBot
{
    class StockBot : IHostedService
    {
        private readonly ISubscriber subscriber;
        private readonly IPublisher publisher;
        private readonly IAlphaVantage alphaVantage;

        public StockBot(ISubscriber subscriber,
            IPublisher publisher,
            IAlphaVantage alphaVantage)
        {
            this.subscriber = subscriber;
            this.publisher = publisher;
            this.alphaVantage = alphaVantage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // SubscribeAsync doesnt seem to be working correctly 
            subscriber.Subscribe(ProcessMessage);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {
            // Message should be only symbol at this point
            // We should validate symbol with Regex
            Console.WriteLine($"Getting stock value for {message}");

            // TODO: Add error handling
            var stockPrice = alphaVantage.GetLatestStockPrice(message).Result;
            var responseMessage = $"{stockPrice.Symbol} is {stockPrice.Price} per share.";

            publisher.Publish(responseMessage, "stockbot-report", null);
            
            // Returning true will dequeue the message, we can implement retry logic in the future
            return true;
        }
    }
}
