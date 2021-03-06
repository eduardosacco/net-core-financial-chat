using System.Threading.Tasks;

namespace StockBot.AlphaVantage
{
    public interface IAlphaVantage
    {
        bool IsValidCommand(string message);

        Task<StockPriceResult> GetLatestStockPrice(string symbol);
    }
}