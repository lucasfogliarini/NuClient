using System.Text.Json.Serialization;

namespace NuClient.Models.Bills
{
	public class BillSummaryResponseParent
	{
        public BillSummaryResponse? Bill { get; set; }
	}

	public class BillSummaryResponse
	{
		public string? Id { get; set; }
		public string? State { get; set; }
		public BillSummary? Summary { get; set; }
		[JsonPropertyName("line_items")]
		public IEnumerable<BillItem>? Items { get; set; }
	}
}