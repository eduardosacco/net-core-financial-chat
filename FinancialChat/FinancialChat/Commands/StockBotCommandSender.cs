using System.Text.RegularExpressions;
using Plain.RabbitMQ;

namespace FinancialChat.Commands
{
    public class StockBotCommandSender : IStockBotCommandSender
    {
        private readonly IPublisher publisher;

        private readonly string commandRegexPattern = @"^\/stock(av)?\s?=";

        public StockBotCommandSender(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        public bool IsValidCommand(string message)
        {
            return Regex.IsMatch(message.Trim(), commandRegexPattern);
        }

        public void Process(string message)
        {
            var groomedMessage = Regex.Replace(message, @"\s", "").Trim('/');

            publisher.Publish(groomedMessage, "stockbot-request", null);
        }
    }
}