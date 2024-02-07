namespace NuClient.Models
{
	public class Links
	{
		public SelfLink? Self { get; set; }
		public class SelfLink
		{
			public string? Href { get; set; }
		}
	}
}
