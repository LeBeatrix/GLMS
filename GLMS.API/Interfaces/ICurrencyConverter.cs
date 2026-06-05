namespace GLMS.API.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertAsync(decimal amount);
    }
}