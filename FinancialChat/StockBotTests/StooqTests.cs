using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using StockBot.Stooq;
using StockBotTests;

namespace FinancialChatTests
{
    public class StooqTests
    {
        private Stooq stooq;
        private HttpClient httpClient;
        private IHttpClientFactory httpClientFactory;

        [SetUp]
        public void Setup()
        {
            httpClientFactory = Substitute.For<IHttpClientFactory>();
            stooq = new Stooq(httpClientFactory);
        }

        [Test]
        public async Task GetLatestStockPrice_ShouldGetValidStockPrice()
        {
            // Arrange
            var response = "Symbol,Date,Time,Open,High,Low,Close,Volume\r\nMSFT.US,2021-08-27,22:00:08,298.987,300.87,296.83,299.72,22605726";
            httpClient = new HttpClient(new MockHttpMessageHandler(response, HttpStatusCode.OK));
            httpClientFactory.CreateClient().ReturnsForAnyArgs(httpClient);

            var symbol = "stock=MSFT.US";

            // Act
            var result = await stooq.GetLatestStockPrice(symbol);

            // Assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual("MSFT.US", result.Symbol);
            Assert.IsNotNull("299.72", result.Price);
        }
    }
}