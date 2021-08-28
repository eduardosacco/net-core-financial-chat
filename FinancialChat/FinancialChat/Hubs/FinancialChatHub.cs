using FinancialChat.Commands;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FinancialChat.Hubs
{
    public class FinancialChatHub : Hub
    {
        private readonly IStockBotCommandSender stockBotCommandSender;

        public FinancialChatHub(IStockBotCommandSender stockBotCommandSender)
        {
            this.stockBotCommandSender = stockBotCommandSender;
        }

        public async Task SendMessage(string user, string message)
        {
            if(await HandleCommand(message))
            {
                return;
            }

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        private async Task<bool> HandleCommand(string message)
        {
            if (message.StartsWith('/'))
            {
                // Commands identification logic here
                // for the moment we only care about /stock command
                if (stockBotCommandSender.IsValidCommand(message))
                {
                    stockBotCommandSender.Process(message);
                }
                else
                {
                    await Clients.All.SendAsync("ReceiveMessage", "Server", $"Invalid command: {message}.");
                }

                return true;
            }

            return false;
        }
    }
}