using System.Text.Json.Serialization;

namespace NuClient.Models.Events
{
	public class EventDetails
	{
        public Charges Charges { get; set; }
        public string? Subcategory { get; set; }
        public string? Status { get; set; }
		[JsonPropertyName("lat")]
		public decimal Latitude { get; set; }
		[JsonPropertyName("lon")]
		public decimal Longitude { get; set; }
    }

	public class Charges
	{
        public int Count { get; set; }
        public decimal Amount { get; set; }
		public decimal CurrencyAmount => Amount / 100;
	}
}
