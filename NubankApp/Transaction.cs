namespace NubankApp
{
	public class Transaction
	{
		public DateTime Time { get; set; }
		public string? Description { get; set; }
		public decimal CurrencyAmount { get; set; }
		public string? Title { get; set; }
		public decimal? ChargesAmount { get; set; }
		public int? Charges { get; set; }
		public string? Category { get; set; }
		public string? Card { get; set; }
	}
}
