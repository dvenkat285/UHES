using Admin.Infrastructure.Shared.Interfaces.Gateway;
using Admin.Infrastructure.Shared.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Implementation

namespace Admin.Infrastructure.Shared.Services.Gateway
{

    public class HESApiGatewayService : IHESApiGatewayService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HESApiGatewayService> _logger;

        public HESApiGatewayService(HttpClient httpClient, ILogger<HESApiGatewayService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<SlabAndTariffInputModel>> GetSlabsAndTariffsList()
        {
            var response = await _httpClient.GetAsync("api/SlabsAndTariffs");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SlabAndTariffInputModel>>(content);
        }

        public async Task<List<CategoryListDto>> GetAllCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/SlabsAndTariffs/GetCategories");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CategoryListDto>>(content);
        }

        public async Task<CategoryCreateDto> SlabsCreate(CategoryCreateDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/SlabsAndTariffs/CreateCategoryWithSubcategoryAndSlab", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CategoryCreateDto>(result);
        }

        public async Task<CategoryUpdateDto> SlabsUpdate(CategoryUpdateDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("api/SlabsAndTariffs/UpdateCategoryWithSubcategoryAndSlab", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CategoryUpdateDto>(result);
        }

        public async Task<SlabsDetailsModel> GetSlabByCategorySubcategoryAndLines(int categoryId, int subcategoryId, string lines)
        {
            var url = $"api/SlabsAndTariffs/GetSlabByCategorySubcategoryAndLines?categoryId={categoryId}&subcategoryId={subcategoryId}&lines={lines}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SlabsDetailsModel>(content);
        }
    }
}
