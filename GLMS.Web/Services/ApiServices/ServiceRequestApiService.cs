using GLMS.Web.Models;
using System.Net.Http.Json;

namespace GLMS.Web.ApiServices
{
    public class ServiceRequestApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthApiService _authApiService;

        public ServiceRequestApiService(HttpClient httpClient, AuthApiService authApiService)
        {
            _httpClient = httpClient;
            _authApiService = authApiService;
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            await AddJwtTokenAsync();
            var requests = await _httpClient.GetFromJsonAsync<List<ServiceRequest>>("api/servicerequests");
            return requests ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
        {
            await AddJwtTokenAsync();
            return await _httpClient.GetFromJsonAsync<ServiceRequest>($"api/servicerequests/{id}");
        }

        public async Task<bool> CreateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.PostAsJsonAsync(
                "api/servicerequests",
                new
                {
                    serviceRequest.ContractId,
                    serviceRequest.Description,
                    serviceRequest.CostUSD,
                    serviceRequest.Status
                }
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.PutAsJsonAsync(
                $"api/servicerequests/{serviceRequest.Id}",
                new
                {
                    serviceRequest.ContractId,
                    serviceRequest.Description,
                    serviceRequest.CostUSD,
                    serviceRequest.Status
                }
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.DeleteAsync($"api/servicerequests/{id}");
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