using System.Net.Http.Json;

namespace GLMS.Web.ApiServices
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> LoginAsync()
        {
            var loginData = new
            {
                username = "admin",
                password = "Admin123!"
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result?.Token;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}