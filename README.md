[![NuGet](https://img.shields.io/nuget/v/nucli?label=Nucli&style=for-the-badge)](https://www.nuget.org/packages/nucli/)

# NuClient

`NuClient` é uma biblioteca para interagir com a API do Nubank para buscar eventos(transações, pagamentos, etc) dos cartões de crédito.

## Operações
   - `LoginAsync()`: Logar o usuário com as credenciais fornecidas.
   - `AutenticateWithQrCodeAsync(string code)`: Autentica o usuário usando um código QR.
   - `GetEventsAsync()`: Obtém eventos da conta do usuário autenticado.
   - `GetBillsAsync()`: Obtém todas as faturas da conta do usuário autenticado.
   - `GetBillSummaryAsync()`: Obtém o resumo de uma fatura específica.
   - `GetTransactionDetailsAsync(Event @event)`: Obtém detalhes de um evento do tipo transação.
   - `UseCache(string filePath) (private)`: Permite usar cache usando informações da requisição em AutenticateWithQrCodeAsync salvos em um arquivo lift.json. (O token desse cache expira em aproximadamente 1 semana)

## Como usar
O código abaixo exemplifica como usar, mas também é possível rodar o Console.App NubankApp que está na mesma solution do código.

```csharp

string login = "seu_login";
string senha = "sua_senha";

string liftCachedFilePath = "lift.json";//opcional
var nubankClient = new NubankClient(login, senha, liftCachedFilePath);
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
```


