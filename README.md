[![NuGet](https://img.shields.io/nuget/v/nucli?label=Nucli&style=for-the-badge)](https://www.nuget.org/packages/nucli/)

# NuClient

`NuClient` � uma biblioteca para interagir com a API do Nubank para buscar eventos(transa��es, pagamentos, etc) dos cart�es de cr�dito.

## Opera��es
   - `LoginAsync()`: Autenticar o usu�rio com as credenciais fornecidas.
   - `AutenticateWithQrCodeAsync(string code)`: Autentica o usu�rio usando um c�digo QR.
   - `GetEventsAsync()`: Recupera eventos da conta do usu�rio autenticado.
   - `GetTransactionDetailsAsync(Event @event)`: Obt�m detalhes de transa��o para um evento espec�fico.

## Como usar
O c�digo abaixo exemplifica como usar, mas tamb�m � poss�vel rodar o Console.App NubankApp que est� na mesma solution do c�digo.

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


