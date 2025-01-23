using System.Net;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverterAPI.Controller;
using Moq.Protected;
using CurrencyConverterAPI.Models;
namespace CurrencyConverterTests
{
    public class CurrencyConversionTests
    {
        [Fact]
        public async Task GetLatestRates_ReturnsOkResult_WithExchangeRates()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"base\":\"USD\",\"rates\":{\"EUR\":0.85,\"GBP\":0.75}}")
            };

            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.frankfurter.app")
            };
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);
            var controller = new ExchangeRatesController(mockHttpClientFactory.Object);
            var result = await controller.GetLatestRates("USD");
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var response = Assert.IsType<ExchangeRatesResponse>(okResult.Value); 
            Assert.Equal("USD", response.@base);
            Assert.Equal(2, response.rates.Count);
            Assert.Equal(0.85m, Convert.ToDecimal(response.rates["EUR"]));
            Assert.Equal(0.75m, Convert.ToDecimal(response.rates["GBP"]));
        }


    }
}
