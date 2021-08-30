using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FinancialChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;

namespace FinancialChat.Commands
{
    public class StockBotCommandReceiver : IHostedService
    {
        private readonly ISubscriber subscriber;
        private readonly IHubContext<FinancialChatHub> financialChatHub;

        public StockBotCommandReceiver(ISubscriber subscriber,
            IHubContext<FinancialChatHub> financialChatHub)
        {
            this.subscriber = subscriber;
            this.financialChatHub = financialChatHub;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            subscriber.SubscribeAsync(ProcessMessage);

            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers)
        {
            // Send StockBot response to chat
            // We are sending response to all clients but we could identify the command issuer
            // And send response just only to that user if needed
            await financialChatHub.Clients.All.SendAsync("ReceiveMessage", "StockBot", message);

            return true;
        }
    }
}
