using System.Text.Json.Serialization;

namespace NuClient.Models.Bills
{
	public class BillSummary
	{
		[JsonPropertyName("due_date")]
		public DateTimeOffset DueDate { get; set; }
		[JsonPropertyName("close_date")]
		public DateTimeOffset CloseDate { get; set; }
		[JsonPropertyName("late_interest_rate")]
		public decimal LateInterestRate { get; set; }
		[JsonPropertyName("past_balance")]
		public decimal PastBalance { get; set; }
		[JsonPropertyName("late_fee")]
		public decimal LateFee { get; set; }
		[JsonPropertyName("effective_due_date")]
		public DateTimeOffset EffectiveDueDate { get; set; }
		[JsonPropertyName("spent_amount")]
		public decimal SpentAmount { get; set; }
		[JsonPropertyName("total_balance")]
		public decimal TotalBalance { get; set; }
		[JsonPropertyName("interest_rate")]
		public decimal InterestRate { get; set; }
		[JsonPropertyName("interest")]
		public decimal Interest { get; set; }
		[JsonPropertyName("total_cumulative")]
		public decimal TotalCumulative { get; set; }
		[JsonPropertyName("paid")]
		public decimal Paid { get; set; }
		[JsonPropertyName("minimum_payment")]
		public decimal MinimumPayment { get; set; }
		[JsonPropertyName("open_date")]
		public DateTimeOffset OpenDate { get; set; }
	}
}