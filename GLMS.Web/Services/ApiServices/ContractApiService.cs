using GLMS.Web.Models;
using System.Net.Http.Json;

namespace GLMS.Web.ApiServices
{
    public class ContractApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthApiService _authApiService;

        public ContractApiService(HttpClient httpClient, AuthApiService authApiService)
        {
            _httpClient = httpClient;
            _authApiService = authApiService;
        }

        public async Task<List<Contract>> GetContractsAsync()
        {
            await AddJwtTokenAsync();
            var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>("api/contracts");
            return contracts ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
                        await AddJwtTokenAsync();
            return await _httpClient.GetFromJsonAsync<Contract>($"api/contracts/{id}");
        }

        public async Task<bool> CreateContractAsync(Contract contract, IFormFile agreementFile)
        {
            await AddJwtTokenAsync();
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

        public async Task<bool> UpdateContractAsync(Contract contract)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.PutAsJsonAsync(
                $"api/contracts/{contract.Id}",
                new
                {
                    contract.ClientId,
                    contract.StartDate,
                    contract.EndDate,
                    contract.Status,
                    contract.ServiceLevel
                }
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            await AddJwtTokenAsync();
            var response = await _httpClient.DeleteAsync(
                $"api/contracts/{id}"
            );

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