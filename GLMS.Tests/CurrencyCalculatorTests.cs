using GLMS.Web.Services;

namespace GLMS.Tests
{
    public class CurrencyCalculatorTests
    {
        [Fact]
        public void ConvertUsdToZar_ValidAmount_ReturnsCorrectResult()
        {
            var calculator = new CurrencyCalculator();

            var result = calculator.ConvertUsdToZar(100, 18.50m);

            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroAmount_ReturnsZero()
        {
            var calculator = new CurrencyCalculator();

            var result = calculator.ConvertUsdToZar(0, 18.50m);

            Assert.Equal(0, result);
        }

        [Fact]
        public void ConvertUsdToZar_NegativeAmount_ThrowsException()
        {
            var calculator = new CurrencyCalculator();

            Assert.Throws<ArgumentException>(() =>
                calculator.ConvertUsdToZar(-10, 18.50m));
        }
    }
}