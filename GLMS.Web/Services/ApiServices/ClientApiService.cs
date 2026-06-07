using GLMS.Web.Models;
using System.Net.Http.Json;

namespace GLMS.Web.ApiServices
{
    public class ClientApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthApiService _authApiService;

        public ClientApiService(HttpClient httpClient, AuthApiService authApiService)
        {
            _httpClient = httpClient;
            _authApiService = authApiService;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            await AddJwtTokenAsync();
            var clients = await _httpClient.GetFromJsonAsync<List<Client>>("api/clients");
            return clients ?? new List<Client>();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            await AddJwtTokenAsync();
            return await _httpClient.GetFromJsonAsync<Client>($"api/clients/{id}");
        }

        public async Task<bool> CreateClientAsync(Client client)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/clients/{client.Id}", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
                        await AddJwtTokenAsync();
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }

        private async Task AddJwtTokenAsync()
        {
            var token = await _authApiService.LoginAsync();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}