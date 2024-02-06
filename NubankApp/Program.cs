using ConsoleTables;
using Microsoft.Extensions.Configuration;
using NubankApp;
using NuClient;
using NuClient.Models.Events;

Console.WriteLine("Iniciando NubankApp usando o pacote 'NuCli' ...");
Console.WriteLine("Iniciando as configurações (local.settings.json) ...");
var config = GetConfig();
Console.WriteLine($"Logando se no Nubank usando o Login: '{config.Login}' e Senha: '{config.Password}'");
Console.WriteLine();

var nubankClient = new NubankClient(config.Login, config.Password);
var result = await nubankClient.LoginAsync();

if (result.NeedsDeviceAuthorization)
{
	Console.WriteLine("Você deve se autenticar com o aplicativo do Nubank para acessar os dados.");
	Console.WriteLine("Entre no aplicativo Nubank > Perfil > Segurança > Acesso no Navegador");
	Console.WriteLine("E scaneie o QRCode abaixo:");
	Console.WriteLine();
	Console.WriteLine(result.GetQrCodeAsAscii());
	Console.WriteLine($"Depois de autorizado clique em qualquer tecla para continuar ...");
	Console.ReadKey();

	await nubankClient.AutenticateWithQrCodeAsync(result.Code);
}

Console.Clear();

bool exit = false;
while (!exit)
{
	Console.WriteLine("c: Transações de Cartão de Crédito.");
	Console.WriteLine("e: Sair.");
	var opt = Console.ReadKey();
	Console.Clear();
	switch (opt.Key)
	{
		case ConsoleKey.C:
			int invoiceClosingYear = int.Parse(config.InvoiceClosingYear);
			int invoiceClosingDay = int.Parse(config.InvoiceClosingDay);
			Console.WriteLine("Eu preciso de uma data da fatura para filtrar as transações.");
			Console.WriteLine($"Ano de fechamento da fatura: '{invoiceClosingYear}' (from configuration)");
			Console.WriteLine($"Dia de fechamento da fatura: '{invoiceClosingDay}' (from configuration)");
			Console.Write("Por último preciso do mês de fechamento da fatura: ");
			var monthKey = Console.ReadLine();
			if (!int.TryParse(monthKey, out int invoiceClosingMonth))
			{
				Console.WriteLine("Mês precisa ser um número!");
				return;
			}
			Console.Write("É possível filtrar por um cartão digitando os últimos 4 dígitos ou digite qualquer tecla para todos os cartões: ");
			var card = Console.ReadLine();

			await GetTransactionsAsync(invoiceClosingYear, invoiceClosingMonth, invoiceClosingDay, card);
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
	var cardMessage = card == null ? "todos os cartões" : $"o cartão com final '{card}'";
	Console.Clear();
	var to = new DateTime(year, month, day);
	var from = to.AddMonths(-1);
	Console.WriteLine($"Procurando por transações de '{from.ToShortDateString()}' até '{to.ToShortDateString()}' com {cardMessage}");
	var events = await nubankClient.GetEventsAsync();
	events = events
				.Where(e => e.Time >= from && e.Time < to)
				.Where(e => e.Category == Event.transaction);
	Console.WriteLine($"Encontrei {events.Count()} transações.");
	var transactions = new List<Transaction>();
	var eventsCount = events.Count();
	for (int i = 0; i < eventsCount; i++)
	{
		var e = events.ElementAt(i);
		if(card != null)
		{
			decimal transactionDetailProcess = (decimal)(i+1) / eventsCount * 100;
			Console.WriteLine($"Procurando por detalhes da transação '{e.Id}'. {transactionDetailProcess:F2}%");
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
		InvoiceClosingDay = config["invoiceClosingDay"],
		InvoiceClosingYear = config["invoiceClosingYear"]
	};
}