namespace GLMS.Web.Models.API
{
	public class ExchangeRateResponse
	{
		public string Result { get; set; } = string.Empty;

		public Dictionary<string, decimal> Rates { get; set; }
			= new Dictionary<string, decimal>();
	}
}