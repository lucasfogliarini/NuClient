using NuClient.Models.Login;
using NuClient.Models.Events;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using NuClient.Models.Bills;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

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
		private string? Bills => _autenticatedEndpoints?.GetValueOrDefault("bills_summary");
		private string? Lift => GetDiscoverAppEndpoint("lift");
		private readonly string _login;
        private readonly string _password;
        private readonly HttpClient _httpClient;
        private bool _saveLift = false;

		public NubankClient(string login, string password, string? liftCachedFilePath = null)
            : this(new HttpClient(), login, password)
        {
            UseCache(liftCachedFilePath);        
        }

        public NubankClient(HttpClient httpClient, string login, string password)
        {
            _login = login;
            _password = password;
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync()
        {
            bool mustAuthenticate = Events == null;
            return new LoginResponse(mustAuthenticate);
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

            if(_saveLift)
                File.WriteAllText("lift.json", await liftResponse.Content.ReadAsStringAsync());

			SetLift(lift);
		}
        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            var eventsResponse = await _httpClient.GetAsync(Events);
            var events = await eventsResponse.Content.ReadFromJsonAsync<GetEventsResponse>();
			return events.Events;
        }
		public async Task<IEnumerable<Bill>?> GetBillsAsync()
		{
			var billsSummaryResponse = await _httpClient.GetAsync(Bills);
			var bills = await billsSummaryResponse.Content.ReadFromJsonAsync<BillsResponse>();
            return bills?.Bills;
		}
		public async Task<BillSummaryResponse?> GetBillSummaryAsync(Bill bill)
		{
			ArgumentNullException.ThrowIfNull(bill);
			var billSummaryLink = bill.Links?.Self?.Href ?? throw new Exception("BillSummaryResponse must have _links");

			var billSummaryResponse = await _httpClient.GetAsync(billSummaryLink);
			var billSummary = await billSummaryResponse.Content.ReadFromJsonAsync<BillSummaryResponseParent>();
			return billSummary?.Bill;
		}
		public async Task<TransactionDetails?> GetTransactionDetailsAsync(Event @event)
		{
            var transactionDetailLink = @event.Links?.Self?.Href ?? throw new Exception("Event must be transaction category (this category provides the '_links' field)");

			var transactionDetailResponse = await _httpClient.GetAsync(transactionDetailLink);
			var transactionDetail = await transactionDetailResponse.Content.ReadFromJsonAsync<TransactionResponse>();
			return transactionDetail?.Transaction;
		}

        private void UseCache(string liftCachedFilePath)
        {
			_saveLift = true;
			if (!File.Exists(liftCachedFilePath))
                return;

			string liftContent = File.ReadAllText(liftCachedFilePath);
			var lift = JsonSerializer.Deserialize<Dictionary<string, object>>(liftContent);
            SetLift(lift);
		}
        private void SetLift(Dictionary<string, object> lift)
        {
			SetAuthToken(lift);
			SetAuthEndpoints(lift);
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
        private void SetAuthToken(Dictionary<string, object> response)
        {
			var accessToken = ValidateTokenExpiration(response);
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
		public string? ValidateTokenExpiration(Dictionary<string, object> response)
		{
			if (!response.TryGetValue("access_token", out object accessToken))
				throw new Exception("Lift must have a access_token");

			var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken.ToString());
			if (jwtToken.ValidTo <= DateTime.UtcNow)
				throw new SecurityTokenExpiredException($"JwtToken has expired at {jwtToken.ValidTo}");

			return accessToken.ToString();
		}
	}
}