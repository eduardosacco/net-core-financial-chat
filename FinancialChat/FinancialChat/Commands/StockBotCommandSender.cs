using System.Text.RegularExpressions;
using FinancialChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Plain.RabbitMQ;

namespace FinancialChat.Commands
{
    public class StockBotCommandSender : IStockBotCommandSender
    {
        private readonly IPublisher publisher;

        private readonly string stockRegexPattern = "^\\/stock=([A-Z]{1,5})";

        public StockBotCommandSender(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        public bool IsValidCommand(string message)
        {
            return Regex.IsMatch(message, stockRegexPattern);
        }

        public void Process(string message)
        {
            var match = Regex.Match(message, stockRegexPattern);
            var capture = match.Groups[1].Value;

            publisher.Publish(capture, "stockbot-request", null);
        }

    }
}
