using System.Text.Json.Serialization;

namespace NuClient.Models.Events
{
	public class TransactionDetails
	{
		[JsonPropertyName("card_last_four_digits")]
		public string? CardLastFourDigits { get; set; }
        public string? Card { get; set; }
		[JsonPropertyName("event_type")]
		public string? EventType { get; set; }
		[JsonPropertyName("card_type")]
		public string? CardType { get; set; }
        public string? Mcc { get; set; }
        public DateTimeOffset Time { get; set; }
		public string? Category { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; }
    }

    public class TransactionResponse
    {
        public TransactionDetails? Transaction { get; set; }
    }
}
