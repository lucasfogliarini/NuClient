using ConsoleTables;
using Microsoft.Extensions.Configuration;
using NubankApp;
using NuClient;
using NuClient.Models.Events;

Console.WriteLine("Initing NubankApp using NuClient package ...");
Console.WriteLine("Initing config (local.settings.json) ...");
var config = GetConfig();
Console.WriteLine($"Logging in NubankClient using '{config.Login}' and '{config.Password}'");
Console.WriteLine();

var nubankClient = new NubankClient(config.Login, config.Password);
var result = await nubankClient.LoginAsync();

if (result.NeedsDeviceAuthorization)
{
	Console.WriteLine("You must authenticate with your phone to be able to access your data.");
	Console.WriteLine("Scan the QRCode below with you Nubank application on the following menu:");
	Console.WriteLine("Nu(Seu Nome) > Perfil > Acesso pelo site");
	Console.WriteLine();

	Console.WriteLine(result.GetQrCodeAsAscii());
	Console.WriteLine($"Use your phone to scan and after this press any key to continue...");
	Console.ReadKey();

	await nubankClient.AutenticateWithQrCodeAsync(result.Code);
}



bool exit = false;
while (!exit)
{
	Console.WriteLine("c: Credit Card transactions.");
	Console.WriteLine("e: exit.");
	var opt = Console.ReadKey();
	Console.Clear();
	switch (opt.Key)
	{
		case ConsoleKey.C:
			int currentYear = DateTime.Now.Year;
			int invoiceClosingDay = int.Parse(config.InvoiceClosingDay);
			Console.WriteLine("I need a closing date for a specific invoice to filter transactions.");
			Console.WriteLine($"Invoice closing year: '{currentYear}' (current year)");
			Console.WriteLine($"Invoice closing day: '{invoiceClosingDay}' (from configuration)");
			Console.Write("Then choice the invoice closing month: ");
			var monthKey = Console.ReadLine();
			if (!int.TryParse(monthKey, out int month))
			{
				Console.WriteLine("Month must be number!");
				return;
			}
			Console.Write("Filter a card by typing the last 4 digits or any key for all cards: ");
			var card = Console.ReadLine();

			await GetTransactionsAsync(currentYear, month, invoiceClosingDay, card);
			break;
		case ConsoleKey.E:
			exit = true;
			break;
		default:
			//Console.WriteLine("");
			break;
	}
}

async Task GetTransactionsAsync(int year, int month, int day, string? card = null)
{
	card = string.IsNullOrWhiteSpace(card) ? null : card;
	var cardMessage = card == null ? "all cards" : $"final card '{card}'";
	Console.Clear();
	var to = new DateTime(year, month, day);
	var from = to.AddMonths(-1);
	Console.WriteLine($"Looking for transactions from '{from.ToShortDateString()}' to '{to.ToShortDateString()}' of {cardMessage}");
	var events = await nubankClient.GetEventsAsync();
	events = events
				.Where(e => e.Time >= from && e.Time < to)
				.Where(e => e.Category == Event.transaction);
	Console.WriteLine($"Found {events.Count()} Transactions.");
	var transactions = new List<Transaction>();
	var eventsCount = events.Count();
	for (int i = 0; i < eventsCount; i++)
	{
		var e = events.ElementAt(i);
		if(card != null)
		{
			decimal transactionDetailProcess = (decimal)(i+1) / eventsCount * 100;
			Console.WriteLine($"Looking for Transaction Detail '{e.Id}'. {transactionDetailProcess:F2}%");
			var transactionDetail = await nubankClient.GetTransactionDetailsAsync(e);
			if (transactionDetail?.Card_last_four_digits != card)
				continue;
		}

		var transaction = new Transaction
		{
			Time = e.Time,
			Description = e.Description,
			CurrencyAmount = e.CurrencyAmount,
			ChargesAmount = e.Details?.Charges?.CurrencyAmount ?? e.CurrencyAmount,
			Charges = e.Details?.Charges?.Count,
			Title = e.Title,
			Category = e.Category,
			Card = card,
		};
		transactions.Add(transaction);
	}

	Console.Clear();
	var total = transactions.Sum(t=>t.ChargesAmount);
	Console.WriteLine();
	Console.WriteLine($"Total: {total} (ChargesAmount)");
	ConsoleTable
	.From(transactions)
	.Write(Format.Minimal);
	Console.WriteLine();
	Console.WriteLine($"Total: {total} (ChargesAmount)");
}

Config GetConfig()
{
	IConfiguration config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
			.Build();

	return new Config
	{
		Login = config["login"],
		Password = config["password"],
		InvoiceClosingDay = config["invoiceClosingDay"]
	};
}