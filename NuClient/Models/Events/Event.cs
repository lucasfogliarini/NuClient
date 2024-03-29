using System.Text.Json.Serialization;

namespace NuClient.Models.Events
{
	public class Event
    {
		public const string transaction = "transaction";
		public const string payment = "payment";

        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyAmount => Amount / 100;
        public DateTimeOffset Time { get; set; }
        public string? Title { get; set; }
        public string? Account { get; set; }
        public EventDetails? Details { get; set; }
		[JsonPropertyName("_links")]
		public Links? Links { get; set; }
    }
}