using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;
using StockBot.AlphaVantage;
using StockBot.Stooq;

namespace StockBot
{
    class StockBot : IHostedService
    {
        private readonly ISubscriber subscriber;
        private readonly IPublisher publisher;
        private readonly IAlphaVantage alphaVantage;
        private readonly IStooq stooq;

        public StockBot(ISubscriber subscriber,
            IPublisher publisher,
            IAlphaVantage alphaVantage,
            IStooq stooq)
        {
            this.subscriber = subscriber;
            this.publisher = publisher;
            this.alphaVantage = alphaVantage;
            this.stooq = stooq;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // SubscribeAsync doesnt seem to be working correctly 
            subscriber.SubscribeAsync(ProcessMessage);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers)
        {
            string responseMessage = $"Invalid command: {message}";
            StockPriceResult stockPriceResult = null;
            if (alphaVantage.IsValidCommand(message))
            {
                Console.WriteLine($"Getting stock value for {message} using Alpha Vantage");
                stockPriceResult = await alphaVantage.GetLatestStockPrice(message);
            }
            else if (stooq.IsValidCommand(message))
            {
                Console.WriteLine($"Getting stock value for {message} using Alpha Vantage");
                stockPriceResult = stooq.GetLatestStockPrice(message).Result;
            }

            if (stockPriceResult != null)
            { 
                if (stockPriceResult.Success)
                {
                    responseMessage = $"{stockPriceResult.Symbol} is {stockPriceResult.Price} per share.";
                }
                else if (!string.IsNullOrEmpty(stockPriceResult.ErrorMessage))
                {
                    responseMessage = stockPriceResult.ErrorMessage;
                }
            }

            Console.WriteLine($"Bot response: {responseMessage}");
            publisher.Publish(responseMessage, "stockbot-report", null);
            
            // Returning true will dequeue the message, we can implement retry logic in the future
            return true;
        }
    }
}
