using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;

namespace StockBot
{
    class StockBot : IHostedService
    {
        private readonly ISubscriber subscriber;

        public StockBot(ISubscriber subscriber)
        {
            this.subscriber = subscriber;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(ProcessMessage);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {
            // Process message here
            Console.WriteLine(message);
            return true;
        }
    }
}
