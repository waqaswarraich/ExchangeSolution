# Currency Converter API

This is a .NET Core Web API that provides functionality for retrieving the latest exchange rates and converting currencies using data from the [Frankfurter API](https://www.frankfurter.app). The API supports basic currency conversion operations and provides endpoints for fetching exchange rates.

---

## Features
- Fetch the latest exchange rates for a specified base currency.
- Convert amounts between different currencies (feature under development).
- Unit tests implemented with xUnit to verify API behavior.

 ## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/) (version 6.0 or higher)
- [Visual Studio](https://visualstudio.microsoft.com/) or any code editor
- [Git](https://git-scm.com/) to clone the repository

- ### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/waqaswarraich/ExchangeSolution.git
   cd ExchangeSolution
2.Restore the NuGet packages:
  dotnet restore
3. Build the application:
  dotnet build
  
## Running the Application

1.Start the application:
  dotnet run

## API Endpoints
1. Get Latest Exchange Rates
Endpoint: /api/ExchangeRates/latest
Method: GET

Query Parameters:
baseCurrency (optional): The base currency to get rates for. Default is EUR.

Example Request:
GET https://localhost:5001/api/ExchangeRates/latest?baseCurrency=USD

Response:{
  "base": "USD",
  "rates": {
    "EUR": 0.85,
    "GBP": 0.75
  }
}

2. Convert Currency (Future Feature)
Planned functionality to convert a specified amount from one currency to another.

Assumptions
The application fetches exchange rates using the Frankfurter API.
The default base currency is EUR unless specified otherwise in the request.
Error handling is minimal, and more validation is needed for unsupported or invalid currencies.
