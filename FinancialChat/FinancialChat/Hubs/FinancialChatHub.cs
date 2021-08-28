using Microsoft.AspNetCore.SignalR;
using Plain.RabbitMQ;
using System.Threading.Tasks;

namespace FinancialChat.Hubs
{
    public class FinancialChatHub : Hub
    {
        private readonly IPublisher publisher;

        public FinancialChatHub(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}