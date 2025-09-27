using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
    public class CategoryCreateResultDto

    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }    // Add this
        public int SlabTariffId { get; set; }     // Add this

        public string CategoryName { get; set; }
        public string? Category { get; set; }
        public List<SubCategoryDto> SubCategories { get; set; } = new List<SubCategoryDto>();
        public List<SlabTariffDto> SlabTariffs { get; set; } = new List<SlabTariffDto>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }
}
