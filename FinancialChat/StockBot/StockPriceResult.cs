namespace StockBot
{
    public class StockPriceResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public string Symbol { get; set; }

        public string Price { get; set; }


    }
}
