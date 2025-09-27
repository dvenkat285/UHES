
//using EFICAA.UHES.Infrastructure.Shared.Interfaces.SlabsAndTariffs;
namespace Admin.MVCWebUI.Models
{
       public class SlabWrapperUnifiedModel
    {
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string SubCategory { get; set; }
        public string SubCategoryName { get; set; }
        public string Lines { get; set; }
        public List<SlabAndTariffInputModel> SubCategoriesDetails { get; set; }
    }

}

