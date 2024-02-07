using System.Text.Json.Serialization;

namespace NuClient.Models.Bills
{
	public class Bill
	{
        public string? State { get; set; }
        public BillsSummary? Summary { get; set; }
		[JsonPropertyName("_links")]
		public Links? Links { get; set; }
	}
}
