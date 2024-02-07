using System.Text.Json.Serialization;

namespace NuClient.Models.Bills
{
	public class BillItem
	{
		public const string chargeType = "charge";
		public const string adjustmentType = "adjustment";

		[JsonPropertyName("post_date")]
		public DateTimeOffset PostDate { get; set; }
		[JsonPropertyName("amount")]
		public decimal Amount { get; set; }
		[JsonPropertyName("title")]
		public string? Title { get; set; }
		[JsonPropertyName("category")]
		public string? Category { get; set; }
		[JsonPropertyName("transaction_id")]
		public string? TransactionId { get; set; }
		[JsonPropertyName("index")]
		public int Index { get; set; }
		[JsonPropertyName("charges")]
		public int Charges { get; set; }
		[JsonPropertyName("type")]
		public string? Type { get; set; }
		[JsonPropertyName("id")]
		public string? Id { get; set; }
		[JsonPropertyName("href")]
		public string? Href { get; set; }
		public decimal CurrencyAmount => Amount / 100;
    }
}
