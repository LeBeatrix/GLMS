using System.Text.Json;
using GLMS.Web.Models.API;

namespace GLMS.Web.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync(
                "https://open.er-api.com/v6/latest/USD"
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(
                json,
                options
            );

            if (data == null || !data.Rates.ContainsKey("ZAR"))
            {
                throw new Exception("Could not retrieve ZAR exchange rate.");
            }

            return data.Rates["ZAR"];
        }
    }
}