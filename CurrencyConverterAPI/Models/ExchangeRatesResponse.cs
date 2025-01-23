namespace CurrencyConverterAPI.Models
{
    public class ExchangeRatesResponse
    {
        public double amount { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
        public Dictionary<string, double> rates { get; set; }
    }

}
