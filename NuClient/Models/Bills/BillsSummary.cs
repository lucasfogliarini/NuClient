using System.Text.Json.Serialization;

namespace NuClient.Models.Bills
{
	public class BillsSummary
	{
		[JsonPropertyName("payments")]
		public string? Payments { get; set; }
		[JsonPropertyName("interest_charge")]
		public string? InterestCharge { get; set; }
		[JsonPropertyName("total_international")]
		public string? TotalInternational { get; set; }
		[JsonPropertyName("due_date")]
		public DateTimeOffset DueDate { get; set; }
		[JsonPropertyName("precise_minimum_payment")]
		public string? PreciseMinimumPayment { get; set; }
		[JsonPropertyName("interest_reversal")]
		public string? InterestReversal { get; set; }
		[JsonPropertyName("close_date")]
		public DateTimeOffset CloseDate { get; set; }
		[JsonPropertyName("expenses")]
		public string? Expenses { get; set; }
		[JsonPropertyName("total_credits")]
		public string? TotalCredits { get; set; }
		[JsonPropertyName("past_balance")]
		public decimal PastBalance { get; set; }
		[JsonPropertyName("effective_due_date")]
		public DateTimeOffset EffectiveDueDate { get; set; }
		[JsonPropertyName("international_tax")]
		public string? InternationalTax { get; set; }
		[JsonPropertyName("tax")]
		public string? Tax { get; set; }
		[JsonPropertyName("adjustments")]
		public string? Adjustments { get; set; }
		[JsonPropertyName("precise_total_balance")]
		public string? PreciseTotalBalance { get; set; }
		[JsonPropertyName("total_financed")]
		public string? TotalFinanced { get; set; }
		[JsonPropertyName("total_balance")]
		public decimal TotalBalance { get; set; }
		[JsonPropertyName("interest_rate")]
		public string? InterestRate { get; set; }
		[JsonPropertyName("total_national")]
		public string? TotalNational { get; set; }
		[JsonPropertyName("previous_bill_balance")]
		public string? PreviousBillBalance { get; set; }
		[JsonPropertyName("interest")]
		public decimal Interest { get; set; }
		[JsonPropertyName("total_cumulative")]
		public decimal TotalCumulative { get; set; }
		[JsonPropertyName("paid")]
		public decimal Paid { get; set; }
		[JsonPropertyName("fees")]
		public string? Fees { get; set; }
		[JsonPropertyName("total_payments")]
		public string? TotalPayments { get; set; }
		[JsonPropertyName("minimum_payment")]
		public decimal MinimumPayment { get; set; }
		[JsonPropertyName("remaining_minimum_payment")]
		public int RemainingMinimumPayment { get; set; }
		[JsonPropertyName("open_date")]
		public DateTimeOffset OpenDate { get; set; }
		[JsonPropertyName("total_installments")]
		public string? TotalInstallments { get; set; }
		[JsonPropertyName("total_accrued")]
		public string? TotalAccrued { get; set; }
	}
}
