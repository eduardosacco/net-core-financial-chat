using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace StockBot.AlphaVantage
{
    public class AlphaVantage : IAlphaVantage
    {
        private readonly StockBotOptions stockCommandOptions;
        private readonly IHttpClientFactory httpFactory;

        private readonly string commandRegexPattern = @"^stockav\s*=\s*([\w:.]+)";

        public AlphaVantage(IOptions<StockBotOptions> stockCommandOptions,
            IHttpClientFactory httpFactory)
        {
            this.stockCommandOptions = stockCommandOptions.Value;
            this.httpFactory = httpFactory;
        }

        public bool IsValidCommand(string message)
        {
            return Regex.IsMatch(message.Trim(), commandRegexPattern);
        }

        public async Task<StockPriceResult> GetLatestStockPrice(string command)
        {
            var match = Regex.Match(command, commandRegexPattern);
            var symbol = match.Groups[1].Value;

            if (string.IsNullOrEmpty(symbol))
            {
                return new StockPriceResult
                {
                    Success = false,
                    ErrorMessage = "No symbol provided."
                };
            }

            var apiKey = stockCommandOptions.AlphaVantageApiKey;
            var queryUri = new Uri($"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}");

            GlobalQuote globalQuote;
            try
            {
                using (HttpClient client = httpFactory.CreateClient())
                using (HttpResponseMessage response = await client.GetAsync(queryUri))
                {
                    using (HttpContent content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        var quoteResponse = JsonSerializer.Deserialize<GlobalQuoteResponse>(result);
                        globalQuote = quoteResponse.GlobalQuote;
                    }
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

            if (globalQuote == null || globalQuote.Symbol == null || globalQuote.Price == null)
            {
                return new StockPriceResult
                {
                    Success = false,
                    ErrorMessage = $"No data available or symbol unrecognized. Symbol: {symbol}"
                };
            }

            return new StockPriceResult
            {
                Success = true,
                Symbol = globalQuote.Symbol,
                Price = globalQuote.Price
            };
        }
    }
}