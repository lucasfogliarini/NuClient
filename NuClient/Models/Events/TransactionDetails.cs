namespace NuClient.Models.Events
{
	public class TransactionDetails
	{
        public string? Card_last_four_digits { get; set; }
        public string? Card { get; set; }
        public string? Event_type { get; set; }
        public string? Card_type { get; set; }
        public string Mcc { get; set; }
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
