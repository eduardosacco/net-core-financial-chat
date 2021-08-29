using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;

namespace StockBot.Stooq
{
    public class Stooq : IStooq
    {
        private readonly IHttpClientFactory httpFactory;

        private static readonly string noData = "N/D";
        private static readonly string commandRegexPattern = @"^stock\s*=\s*([\w:.]+)";

        public Stooq(IHttpClientFactory httpFactory)
        {
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

            var queryUri = new Uri($"https://stooq.com/q/l/?s={symbol}&f=sd2t2ohlcv&h&e=csv");

            QuoteResponse quote;
            try
            {
                string result;
                using (HttpClient client = httpFactory.CreateClient())
                using (HttpResponseMessage response = await client.GetAsync(queryUri))
                {
                    using (HttpContent content = response.Content)
                    {
                        result = await content.ReadAsStringAsync();
                    }
                }

                using (TextReader sr = new StringReader(result))
                {
                    var csv = new CsvReader(sr, CultureInfo.InvariantCulture);
                    var quotes = csv.GetRecords<QuoteResponse>();
                    quote = quotes.FirstOrDefault();
                };
            }
            catch (Exception)
            {
                return new StockPriceResult
                {
                    Success = false,
                    ErrorMessage = "There was a problem when getting the stock price from Stooq.com. Try again later."
                };
            }

            if (quote == null || quote.Symbol == noData || quote.Close == noData)
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
                Symbol = quote.Symbol,
                Price = quote.Close
            };
        }
    }
}