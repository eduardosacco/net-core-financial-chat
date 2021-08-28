using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace StockBot.AlphaVantage
{
    public class AlphaVantage
    {
        private readonly StockCommandOptions stockCommandOptions;

        public AlphaVantage(IOptions<StockCommandOptions> stockCommandOptions)
        {
            this.stockCommandOptions = stockCommandOptions.Value;
        }

        public async Task<StockPrice> GetLatestStockPrice(string symbol)
        {
            var apiKey = stockCommandOptions.AlphaVantageApiKey;
            var QUERY_URL = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";
            Uri queryUri = new Uri(QUERY_URL);

            using (WebClient client = new WebClient())
            {
                var result = await client.DownloadStringTaskAsync(queryUri);
                var response = JsonSerializer.Deserialize<GlobalQuoteResponse>(result);
                var globalQuote = response.GlobalQuote;

                return new StockPrice
                {
                    Symbol = globalQuote.Symbol,
                    Price = globalQuote.Price
                };
            }
        }
    }
}