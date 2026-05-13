namespace GLMS.Web.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertAsync(decimal amount);
    }
}