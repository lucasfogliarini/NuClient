namespace NuClient.Models.Events
{
	public class EventDetails
	{
        public Charges Charges { get; set; }
        public string? Subcategory { get; set; }
        public string? Status { get; set; }
        public decimal Lat { get; set; }
		public decimal Lon { get; set; }
    }

	public class Charges
	{
        public int Count { get; set; }
        public int Amount { get; set; }
		public decimal CurrencyAmount => Amount / 100;
	}
}
