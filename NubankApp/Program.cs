using ConsoleTables;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NubankApp;
using NuClient;
using NuClient.Models.Bills;
using NuClient.Models.Events;

Console.WriteLine("Iniciando NubankApp usando o pacote 'NuCli' ...");
Console.WriteLine("Iniciando as configurações (local.settings.json) ...");
var config = GetConfig();
Console.WriteLine($"Logando se no Nubank usando o Login: '{config.Login}' e Senha: '{config.Password}'");
Console.WriteLine();

NubankClient nubankClient;
try
{
	string liftCachedFilePath = "lift.json";//opcional
	nubankClient = new NubankClient(config.Login, config.Password, liftCachedFilePath);
}
catch (SecurityTokenExpiredException ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine($"O jwtToken do lift.json está expirado. Digite 'd' para deletar o lift.json, reinicie o app e tente se reautenticar. {ex.Message}");
	var key = Console.ReadKey();
	if (key.Key == ConsoleKey.D)
	{
		File.Delete("lift.json");
	}
	throw ex;
}

var loginResponse = await nubankClient.LoginAsync();

if (loginResponse.MustAuthenticate)
{
	Console.WriteLine("Você deve se autenticar com o aplicativo do Nubank para acessar os dados.");
	Console.WriteLine("Entre no aplicativo Nubank > Perfil > Segurança > Acesso no Navegador");
	Console.WriteLine("E scaneie o QRCode abaixo:");
	Console.WriteLine();
	Console.WriteLine(loginResponse.GetQrCodeAsAscii());
	Console.WriteLine($"Depois de autorizado clique em qualquer tecla para continuar ...");
	Console.ReadKey();

	await nubankClient.AutenticateWithQrCodeAsync(loginResponse.Code);
}

Console.Clear();

bool exit = false;
while (!exit)
{
	Console.WriteLine("Digite uma opção do menu:");
	Console.WriteLine("f: Buscar fatura de Cartão de Crédito.");
	Console.WriteLine("s: Sair.");
	var opt = Console.ReadKey();
	Console.Clear();
	switch (opt.Key)
	{
		case ConsoleKey.F:
			int invoiceClosingYear = int.Parse(config.InvoiceClosingYear);
			int invoiceClosingDay = int.Parse(config.InvoiceClosingDay);
			Console.WriteLine("Para buscar uma fatura preciso da data de fechamento.");
			Console.WriteLine($"Ano de fechamento da fatura: '{invoiceClosingYear}' (via configuração)");
			Console.WriteLine($"Dia de fechamento da fatura: '{invoiceClosingDay}' (via configuração)");
			Console.Write("Digite o mês de fechamento da fatura: ");
			var monthKey = Console.ReadLine();
			if (!int.TryParse(monthKey, out int invoiceClosingMonth))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Mês precisa ser um número!");
				Console.ResetColor();
				break;
			}
			var invoiceClosingDate = new DateTime(invoiceClosingYear, invoiceClosingMonth, invoiceClosingDay);
			Console.Write("É possível filtrar por um cartão digitando os últimos 4 dígitos ou digite qualquer tecla para todos os cartões: ");
			var card = Console.ReadLine();
			await GetBillAsync(invoiceClosingDate, card);
			break;
		case ConsoleKey.S:
			exit = true;
			break;
		default:
			Console.WriteLine("Não existe essa opção");
			break;
	}
}

async Task GetBillAsync(DateTime invoiceClosingDate, string? card = null)
{
	card = string.IsNullOrWhiteSpace(card) ? null : card;
	var cardMessage = card == null ? "todos os cartões" : $"o cartão de final '{card}'";
	Console.Clear();
	Console.WriteLine($"Buscando fatura de fechamento '{invoiceClosingDate.ToShortDateString()}' com {cardMessage}");
	var bills = await nubankClient.GetBillsAsync();
	Console.WriteLine($"Encontrei {bills.Count()} faturas na sua conta.");
	var bill = bills.FirstOrDefault(b=> b.Summary.CloseDate.Date == invoiceClosingDate);
	Console.WriteLine($"Encontrei a fatura que você quer {bill?.Summary?.CloseDate.Date.ToShortDateString()}");
	if (bill?.State == "future")
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"Esta fatura está no futuro, precisa estar Em Aberto ou Fechada.");
		Console.ResetColor();
		return;
	}
	var billSummary = await nubankClient.GetBillSummaryAsync(bill);
	var events = await nubankClient.GetEventsAsync();
	var invoiceItems = new List<InvoiceItem>();
	var billItemsCount = billSummary.Items.Count();
	for (int i = 0; i < billItemsCount; i++)
	{
		var billItem = billSummary.Items.ElementAt(i);

		if (card != null)
		{
			Event billTransaction = events.FirstOrDefault(e => e.Id == billItem.TransactionId);
			if (billTransaction == null)
				continue;

			decimal billTransactionProcess = (decimal)(i+1) / billItemsCount * 100;
			Console.WriteLine($"{billTransactionProcess:F2}% Buscando por detalhes da transação '{billItem.Title}'.");
			var transactionDetail = await nubankClient.GetTransactionDetailsAsync(billTransaction);
			if (transactionDetail?.CardLastFourDigits != card)
				continue;
		}

		var invoiceItem = new InvoiceItem
		{
			Time = billItem.PostDate.LocalDateTime,
			Title = billItem.Title,
			CurrencyAmount = billItem.CurrencyAmount,
			Charges = billItem.Charges,
			Category = billItem.Category,
			Type = billItem.Type ?? BillItem.openType,
			Card = card ?? "any",
		};
		invoiceItems.Add(invoiceItem);
	}

	var total = invoiceItems.Where(i=> new[] { BillItem.adjustmentType , BillItem.chargeType, BillItem.openType }.Contains(i.Type)).Sum(t=>t.CurrencyAmount);
	Console.WriteLine();
	Console.WriteLine($"Total: {total}");
	ConsoleTable
	.From(invoiceItems.OrderBy(i=>i.Time))
	.Write(Format.Default);
	Console.WriteLine();
	Console.WriteLine($"Total: {total}");
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