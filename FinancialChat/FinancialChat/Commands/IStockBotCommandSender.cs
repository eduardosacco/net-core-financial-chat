namespace FinancialChat.Commands
{
    public interface IStockBotCommandSender
    {
        bool IsValidCommand(string message);
        void Process(string message);
    }
}