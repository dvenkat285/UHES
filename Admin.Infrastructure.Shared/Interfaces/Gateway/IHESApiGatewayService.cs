using Admin.Infrastructure.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Interfaces.Gateway
    {
        public interface IHESApiGatewayService
        {
            Task<List<SlabAndTariffInputModel>> GetSlabsAndTariffsList();
            Task<List<CategoryListDto>> GetAllCategoriesAsync();
            Task<CategoryCreateDto> SlabsCreate(CategoryCreateDto dto);
            Task<CategoryUpdateDto> SlabsUpdate(CategoryUpdateDto dto);
            Task<SlabsDetailsModel> GetSlabByCategorySubcategoryAndLines(int categoryId, int subcategoryId, string lines);
        }
    }
