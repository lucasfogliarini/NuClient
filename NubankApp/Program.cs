using ConsoleTables;
using NuClient;
using NuClient.Models.Events;

Console.WriteLine("Nubank Client");
Console.WriteLine("Please, type your login (CPF):");
var login = Console.ReadLine().Trim();
Console.WriteLine("Type your password:");
var password = Console.ReadLine().Trim();


var nubankClient = new NubankClient(login, password);
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
	Console.WriteLine("c: credit card events.");
	Console.WriteLine("s: savings.");
	Console.WriteLine("e: exit.");
	var opt = Console.ReadKey();
	Console.Clear();
	switch (opt.Key)
	{
		case ConsoleKey.C:
			await GetEventsAsync();
			break;
		case ConsoleKey.E:
			exit = true;
			break;
		default:
			//Console.WriteLine("");
			break;
	}
}

async Task GetEventsAsync()
{
	var month = 1;
	var from = new DateTime(2024, month, 4);
	var to = from.AddMonths(1);
	var events = await nubankClient.GetEventsAsync();
	var ev = events
				.Where(e => e.Time >= from && e.Time < to)
				.Where(e=>e.Category == Event.transaction)
				.Select(e=>new
				{
					e.Time,
					e.Description,
					Amount = e.Details.Charges?.CurrencyAmount ?? e.CurrencyAmount,
					e.CurrencyAmount,
					e.Title,
					ChargesAmount = e.Details.Charges?.CurrencyAmount,
					Charges = e.Details.Charges?.Count,
					e.Category
					
				});
	var total = ev.Sum(e=>e.Amount);
	Console.WriteLine($"Total: {total}");
	ConsoleTable
	.From(ev)
	.Write(Format.Minimal);
}