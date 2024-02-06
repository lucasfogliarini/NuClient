[![NuGet](https://img.shields.io/nuget/v/nucli?label=Nucli&style=for-the-badge)](https://www.nuget.org/packages/nucli/)

# NuClient

`NuClient` é uma biblioteca para interagir com a API do Nubank para buscar eventos(transações, pagamentos, etc) dos cartões de crédito.

## Operações
   - `LoginAsync()`: Autenticar o usuário com as credenciais fornecidas.
   - `AutenticateWithQrCodeAsync(string code)`: Autentica o usuário usando um código QR.
   - `GetEventsAsync()`: Recupera eventos da conta do usuário autenticado.
   - `GetTransactionDetailsAsync(Event @event)`: Obtém detalhes de transação para um evento específico.

## Como usar
O código abaixo exemplifica como usar, mas também é possível rodar o Console.App NubankApp que está na mesma solution do código.

```csharp

string login = "seu_login";
string senha = "sua_senha";

NubankClient nubankClient = new NubankClient(login, senha);
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
	var events = await nubankClient.GetEventsAsync();
}
```


