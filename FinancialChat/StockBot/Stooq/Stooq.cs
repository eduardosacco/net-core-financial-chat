using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;

namespace StockBot.Stooq
{
    public class Stooq : IStooq
    {
        private static readonly string noData = "N/D";
        private static readonly string commandRegexPattern = @"^\/stock?\s?=";

        public bool IsValidCommand(string message)
        {
            return Regex.IsMatch(message.Trim(), commandRegexPattern);
        }

        public async Task<StockPriceResult> GetLatestStockPrice(string symbol)
        {
            var QUERY_URL = $"https://stooq.com/q/l/?s={symbol}&f=sd2t2ohlcv&h&e=csv";
            Uri queryUri = new Uri(QUERY_URL);

            QuoteResponse quote;
            try
            {
                string result;
                using (WebClient client = new WebClient())
                {
                    result = await client.DownloadStringTaskAsync(queryUri);
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

            if ( quote.Symbol == noData || quote.Close == noData)
            {
                return new StockPriceResult
                {
                    Success = true,
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