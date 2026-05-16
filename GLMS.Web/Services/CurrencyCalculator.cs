namespace GLMS.Web.Services
{
    public class CurrencyCalculator
    {
        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            if (usdAmount < 0)
            {
                throw new ArgumentException("USD amount cannot be negative.");
            }

            if (exchangeRate <= 0)
            {
                throw new ArgumentException("Exchange rate must be greater than zero.");
            }

            return Math.Round(usdAmount * exchangeRate, 2);
        }
    }
}