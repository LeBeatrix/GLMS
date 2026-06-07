using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GLMS.Tests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5001/")
            };
        }

        private async Task<string> GetJwtTokenAsync()
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", new
            {
                username = "admin",
                password = "Admin123!"
            });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result!.Token;
        }

        [Fact]
        public async Task GetContracts_WithJwt_ReturnsSuccess()
        {
            var token = await GetJwtTokenAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/contracts");

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetClients_WithJwt_ReturnsSuccess()
        {
            var token = await GetJwtTokenAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/clients");

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_WithJwt_ReturnsSuccess()
        {
            var token = await GetJwtTokenAsync();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/servicerequests");

            Assert.True(response.IsSuccessStatusCode);
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}