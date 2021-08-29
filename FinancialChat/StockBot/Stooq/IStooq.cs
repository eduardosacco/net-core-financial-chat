using System.Threading.Tasks;

namespace StockBot.Stooq
{
    public interface IStooq
    {
        bool IsValidCommand(string message);

        Task<StockPriceResult> GetLatestStockPrice(string symbol);
    }
}