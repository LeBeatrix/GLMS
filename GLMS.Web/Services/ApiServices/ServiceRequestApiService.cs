using GLMS.Web.Models;
using System.Net.Http.Json;

namespace GLMS.Web.ApiServices
{
    public class ServiceRequestApiService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            var requests = await _httpClient.GetFromJsonAsync<List<ServiceRequest>>("api/servicerequests");
            return requests ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ServiceRequest>($"api/servicerequests/{id}");
        }

        public async Task<bool> CreateServiceRequestAsync(ServiceRequest serviceRequest)
        {
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
            var response = await _httpClient.DeleteAsync($"api/servicerequests/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}