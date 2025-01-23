namespace CurrencyConverterAPI.Models
{
    public class HistoricalExchangeRatesResponse
    {
        public double amount { get; set; } // The amount in the base currency
        public string @base { get; set; }   // The base currency (e.g., EUR)
        public DateTime start_date { get; set; } // Start date for historical rates
        public DateTime end_date { get; set; }   // End date for historical rates
        public Dictionary<string, Dictionary<string, double>> rates { get; set; }
        // Outer dictionary key: Date (e.g., "1999-12-30")
        // Inner dictionary key: Currency code (e.g., "USD"), value: Exchange rate
    }
}
