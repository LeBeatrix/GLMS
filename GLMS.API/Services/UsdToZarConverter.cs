using GLMS.API.Interfaces;

namespace GLMS.API.Services
{
    public class UsdToZarConverter : ICurrencyConverter
    {
        private readonly CurrencyService _currencyService;

        public UsdToZarConverter(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<decimal> ConvertAsync(decimal amount)
        {
            decimal rate =
                await _currencyService.GetUsdToZarRateAsync();

            return Math.Round(amount * rate, 2);
        }
    }
}