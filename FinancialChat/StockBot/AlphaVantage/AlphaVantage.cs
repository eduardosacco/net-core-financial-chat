using System;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace StockBot.AlphaVantage
{
    public class AlphaVantage : IAlphaVantage
    {
        private readonly StockBotOptions stockCommandOptions;

        private readonly string commandRegexPattern = @"^\/stockav?\s?=";

        public AlphaVantage(IOptions<StockBotOptions> stockCommandOptions)
        {
            this.stockCommandOptions = stockCommandOptions.Value;
        }

        public bool IsValidCommand(string message)
        {
            return Regex.IsMatch(message.Trim(), commandRegexPattern);
        }

        public async Task<StockPriceResult> GetLatestStockPrice(string symbol)
        {
            var apiKey = stockCommandOptions.AlphaVantageApiKey;
            var QUERY_URL = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";
            Uri queryUri = new Uri(QUERY_URL);

            GlobalQuote globalQuote;
            try
            {
                using (WebClient client = new WebClient())
                {
                    // TODO: Add error handling
                    var result = await client.DownloadStringTaskAsync(queryUri);
                    var response = JsonSerializer.Deserialize<GlobalQuoteResponse>(result);
                    globalQuote = response.GlobalQuote;
                }
            }
            catch (Exception)
            {
                return new StockPriceResult
                {
                    Success = false,
                    ErrorMessage = "There was a problem when getting the stock price from AlphaVantage.co. Try again later."
                };
            }

            if (globalQuote == null)
            {
                return new StockPriceResult
                {
                    Success = true,
                    ErrorMessage = $"No data available or symbol unrecognized. Symbol: {symbol}"
                };
            }

            return new StockPriceResult
            {
                Symbol = globalQuote.Symbol,
                Price = globalQuote.Price
            };
        }
    }
}