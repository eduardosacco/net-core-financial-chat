using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using StockBot;
using StockBot.AlphaVantage;
using StockBotTests;

namespace FinancialChatTests
{
    public class AlphavantageTests
    {
        private AlphaVantage alphaVantage;
        private HttpClient httpClient;
        private IHttpClientFactory httpClientFactory;

        [SetUp]
        public void Setup()
        {
            var stockCommandOptions = new StockBotOptions
            {
                AlphaVantageApiKey = "apikey"
            };

            httpClientFactory = Substitute.For<IHttpClientFactory>();
            alphaVantage = new AlphaVantage(Options.Create(stockCommandOptions), httpClientFactory);
        }

        [Test]
        public async Task GetLatestStockPrice_ValidCommandValidResponse_ShouldGetValidStockPrice()
        {
            // Arrange
            var response = "{\r\n    \"Global Quote\": {\r\n        \"01. symbol\": \"IBM\",\r\n        \"02. open\": \"138.7100\",\r\n        \"03. high\": \"139.5850\",\r\n        \"04. low\": \"138.4000\",\r\n        \"05. price\": \"139.4100\",\r\n        \"06. volume\": \"2459643\",\r\n        \"07. latest trading day\": \"2021-08-27\",\r\n        \"08. previous close\": \"138.7800\",\r\n        \"09. change\": \"0.6300\",\r\n        \"10. change percent\": \"0.4540%\"\r\n    }\r\n}";
            httpClient = new HttpClient(new MockHttpMessageHandler(response, HttpStatusCode.OK));
            httpClientFactory.CreateClient().ReturnsForAnyArgs(httpClient);

            var symbol = "stockav=MSFT";

            // Act
            var result = await alphaVantage.GetLatestStockPrice(symbol);

            // Assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual("IBM", result.Symbol);
            Assert.IsNotNull("139.4100", result.Price);
        }

        [Test]
        public async Task GetLatestStockPrice_ValidCommandEmptyResponse_ShouldGetErrorResponse()
        {
            // Arrange
            var response = "{}";
            httpClient = new HttpClient(new MockHttpMessageHandler(response, HttpStatusCode.OK));
            httpClientFactory.CreateClient().ReturnsForAnyArgs(httpClient);

            var symbol = "stockav=XYZ";

            // Act
            var result = await alphaVantage.GetLatestStockPrice(symbol);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("No data available or symbol unrecognized. Symbol: XYZ", result.ErrorMessage);
        }

        [Test]
        public async Task GetLatestStockPrice_NoSymbol_ShouldGetErrorResponse()
        {
            // Arrange
            var response = "{}";
            httpClient = new HttpClient(new MockHttpMessageHandler(response, HttpStatusCode.OK));
            httpClientFactory.CreateClient().ReturnsForAnyArgs(httpClient);

            var symbol = "stockav=";

            // Act
            var result = await alphaVantage.GetLatestStockPrice(symbol);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("No symbol provided.", result.ErrorMessage);
        }

        [Test]
        public async Task GetLatestStockPrice_HttpError_ShouldGetErrorResponse()
        {
            // Arrange
            httpClientFactory.CreateClient().ReturnsForAnyArgs(x =>  throw new Exception());

            var symbol = "stockav=IBM";

            // Act
            var result = await alphaVantage.GetLatestStockPrice(symbol);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("There was a problem when getting the stock price from AlphaVantage.co. Try again later.", result.ErrorMessage);
        }
    }
}