using CurrencyConverterAPI.Controller;
using CurrencyConverterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverterAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string BaseUrl = "https://api.frankfurter.app";

        public ExchangeRatesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Endpoint 1: Retrieve the latest exchange rates for a specific base currency
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency = "EUR")
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{BaseUrl}/latest?base={baseCurrency}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch latest rates.");

            var data = await response.Content.ReadAsStringAsync();
            var rates = JsonSerializer.Deserialize<ExchangeRatesResponse>(data);

            return Ok(rates);
        }

        // Endpoint 2: Convert amounts between different currencies (excluding specified currencies)
        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] double amount)
        {
            var excludedCurrencies = new[] { "TRY", "PLN", "THB", "MXN" };
            if (excludedCurrencies.Contains(from.ToUpper()) || excludedCurrencies.Contains(to.ToUpper()))
            {
                return BadRequest("Conversions involving TRY, PLN, THB, and MXN are not allowed.");
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}/latest?base={from}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch exchange rates.");

            var data = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var exchangeRates = JsonSerializer.Deserialize<ExchangeRatesResponse>(data, options);

            if (exchangeRates == null || !exchangeRates.rates.ContainsKey(to.ToUpper()))
                return BadRequest("Invalid target currency or no exchange rate available.");

            var conversionRate = exchangeRates.rates[to.ToUpper()];
            var convertedAmount = amount * conversionRate;

            return Ok(new
            {
                FromCurrency = from,
                ToCurrency = to,
                OriginalAmount = amount,
                ConvertedAmount = convertedAmount,
                Rate = conversionRate
            });
        }


        // Endpoint 3: Retrieve historical rates with pagination
        [HttpPost("historical")]
        public async Task<IActionResult> GetHistoricalRates([FromBody] HistoricalRatesRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"{BaseUrl}/{request.StartDate}..{request.EndDate}?base={request.BaseCurrency}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Failed to fetch historical rates.");

                var data = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var historicalRates = JsonSerializer.Deserialize<HistoricalExchangeRatesResponse>(data, options);

                if (historicalRates == null || historicalRates.rates == null || !historicalRates.rates.Any())
                    return NotFound("No historical rates found for the specified period.");

                var paginatedRates = historicalRates.rates
                    .OrderBy(x => DateTime.Parse(x.Key)) 
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

              
                var paginatedResponse = new
                {
                    amount = historicalRates.amount,
                    @base = historicalRates.@base,
                    start_date = request.StartDate,
                    end_date = request.EndDate,
                    rates = paginatedRates
                };

                return Ok(paginatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }
    }
}