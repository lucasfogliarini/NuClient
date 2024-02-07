namespace NubankApp
{
	public class InvoiceItem
	{
		public DateTime Time { get; set; }
		public string? Title { get; set; }
		public decimal CurrencyAmount { get; set; }
		public string? Type { get; set; }
		public int? Charges { get; set; }
		public string? Category { get; set; }
		public string? Card { get; set; }
	}
}
