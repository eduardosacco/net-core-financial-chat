using System.Threading.Tasks;

namespace StockBot.AlphaVantage
{
    public interface IAlphaVantage
    {
        Task<StockPrice> GetLatestStockPrice(string symbol);
    }
}