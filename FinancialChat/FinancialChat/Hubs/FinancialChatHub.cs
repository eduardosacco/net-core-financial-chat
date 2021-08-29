using FinancialChat.Commands;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FinancialChat.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FinancialChat.Hubs
{
    public class FinancialChatHub : Hub
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IStockBotCommandSender stockBotCommandSender;

        public FinancialChatHub(IHttpContextAccessor httpContextAccessor,
            IStockBotCommandSender stockBotCommandSender)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.stockBotCommandSender = stockBotCommandSender;
        }

        public async Task SendMessage(string message)
        {
            if(await HandleCommand(message))
            {
                return;
            }

            var email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var userName = email.Split('@')[0];

            await Clients.All.SendAsync("ReceiveMessage", userName, message);
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