using NuClient.Models.Login;
using NuClient.Models.Events;
using System.Security.Authentication;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NuClient
{
	public class NubankClient
    {
		private const string DiscoveryUrl = "https://prod-s0-webapp-proxy.nubank.com.br/api/discovery";
		private const string DiscoveryAppUrl = "https://prod-s0-webapp-proxy.nubank.com.br/api/app/discovery";
		private Dictionary<string, string> _discoveryEndpoints;
		private Dictionary<string, string> _discoveryAppEndpoints;
		private Dictionary<string, string> _autenticatedEndpoints;
		private string? Login => GetDiscoverEndpoint("login");
		private string? Events => _autenticatedEndpoints?.GetValueOrDefault("events");
		private string? Lift => GetDiscoverAppEndpoint("lift");
		private readonly string _login;
        private readonly string _password;
        private readonly HttpClient _httpClient;

		public NubankClient(string login, string password)
            : this(new HttpClient(), login, password)
        { }

        public NubankClient(HttpClient httpClient, string login, string password)
        {
            _login = login;
            _password = password;
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync()
        {
            //await GetTokenAsync();

            //if (Events != null)
            //{
            //    return new LoginResponse();
            //}

            return new LoginResponse(Guid.NewGuid().ToString());
        }
        public async Task AutenticateWithQrCodeAsync(string code)
        {
			await GetTokenAsync();

			var body = new
            {
                qr_code_id = code,
				type = "login-webapp"
            };
			var jsonContent = JsonContent.Create(body);
            var liftResponse = _httpClient.PostAsync(Lift, jsonContent).Result;
            var lift = liftResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>().Result;

			SetAuthToken(lift);
			SetAuthEndpoints(lift);
		}
        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            var eventsResponse = await _httpClient.GetAsync(Events);
            var events = await eventsResponse.Content.ReadFromJsonAsync<GetEventsResponse>();
			return events.Events;
        }
		public async Task<TransactionDetails?> GetTransactionDetailsAsync(Event @event)
		{
            var transactionDetailLink = @event._Links?.Self?.Href ?? throw new Exception("Event must be transaction category (this category provides the '_links' field)");

			var transactionDetailResponse = await _httpClient.GetAsync(transactionDetailLink);
			var transactionDetail = await transactionDetailResponse.Content.ReadFromJsonAsync<TransactionResponse>();
			return transactionDetail?.Transaction;
		}

		private async Task GetTokenAsync()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization != null)
				return;

			var body = new
            {
                client_id = "other.conta",
                client_secret = "yQPeLzoHuJzlMMSAjC-LgNUJdUecx8XO",
                grant_type = "password",
                login = _login,
                password = _password
            };
			var jsonContent = JsonContent.Create(body);
			var loginResponse = await _httpClient.PostAsync(Login, jsonContent);
            var login = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();

			SetAuthToken(login);
        }
		private Dictionary<string, object> GetBody(string fileName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, fileName);
            string fileContent = File.ReadAllText(filePath);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(fileContent);
            return response;
        }
        private void SetAuthToken(Dictionary<string, object> response)
        {
            if (!response.Keys.Any(x => x == "access_token"))
            {
                if (response.Keys.Any(x => x == "error"))
                {
                    throw new AuthenticationException(response["error"].ToString());
                }
                throw new AuthenticationException("Unknow error occurred on trying to do login on Nubank using the entered credentials");
            }
            var accessToken = response["access_token"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        private void SetAuthEndpoints(Dictionary<string, object> response)
        {
			var _links = (JsonElement)response["_links"];
            _autenticatedEndpoints = [];
			foreach (var link in _links.EnumerateObject())
			{
				_autenticatedEndpoints.Add(link.Name, link.Value.GetProperty("href").GetString());
			}
		}
        private Dictionary<string, string?> GetUrls(string url)
        {
			var urlsResponse = _httpClient.GetAsync(url).Result;
			var urls = urlsResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>().Result;
            return urls.ToDictionary(x => x.Key, x => x.Value?.ToString());
		}
		private string? GetDiscoverEndpoint(string key)
		{
			_discoveryEndpoints ??= GetUrls(DiscoveryUrl);
            return _discoveryEndpoints.GetValueOrDefault(key);
		}
		private string? GetDiscoverAppEndpoint(string key)
		{
			_discoveryAppEndpoints ??= GetUrls(DiscoveryAppUrl);
			return _discoveryAppEndpoints.GetValueOrDefault(key);
		}
	}
}