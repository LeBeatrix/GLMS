using GLMS.Web.Models;
using System.Net.Http.Json;

namespace GLMS.Web.ApiServices
{
    public class ContractApiService
    {
        private readonly HttpClient _httpClient;

        public ContractApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Contract>> GetContractsAsync()
        {
            var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>("api/contracts");
            return contracts ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Contract>($"api/contracts/{id}");
        }

        public async Task<bool> CreateContractAsync(Contract contract, IFormFile agreementFile)
        {
            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(contract.ClientId.ToString()), "ClientId");
            formData.Add(new StringContent(contract.StartDate.ToString("yyyy-MM-dd")), "StartDate");
            formData.Add(new StringContent(contract.EndDate.ToString("yyyy-MM-dd")), "EndDate");
            formData.Add(new StringContent(((int)contract.Status).ToString()), "Status");
            formData.Add(new StringContent(contract.ServiceLevel), "ServiceLevel");

            using var fileStream = agreementFile.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            formData.Add(fileContent, "AgreementFile", agreementFile.FileName);

            var response = await _httpClient.PostAsync("api/contracts", formData);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractStatusAsync(int id, ContractStatus status)
        {
            var response = await _httpClient.PatchAsJsonAsync(
                $"api/contracts/{id}/status",
                new { status }
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/contracts/{id}"
            );

            return response.IsSuccessStatusCode;
        }
    }
}