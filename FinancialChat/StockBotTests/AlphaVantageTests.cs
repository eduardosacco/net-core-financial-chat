using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using StockBot.AlphaVantage;

namespace FinancialChatTests
{
    public class StockCommandTests
    {
        private AlphaVantage alphaVantage;

        [SetUp]
        public void Setup()
        {
            var stockCommandOptions = new StockCommandOptions();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            configuration.GetSection(StockCommandOptions.AlphaVantageApi)
                .Bind(stockCommandOptions);
            alphaVantage = new AlphaVantage(Options.Create(stockCommandOptions));
        }

        [Test]
        public async Task GetLatestStockPrice_ShouldGetValidStockPrice()
        {
            // Arrange
            var symbol = "MSFT";

            // Act
            var result = await alphaVantage.GetLatestStockPrice(symbol);

            // Assert
            Assert.IsNotNull(result.Symbol);
            Assert.IsNotNull(result.Price);
        }
    }
}