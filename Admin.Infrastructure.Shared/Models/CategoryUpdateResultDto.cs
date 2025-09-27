using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
     public class CategoryUpdateResultDto
    {
        public int CategoryId { get; set; }

        // ✅ Add these missing properties:
        public int SubCategoryId { get; set; }
        public int SlabTariffId { get; set; }

        public string CategoryName { get; set; }
        public string? Category { get; set; }

        public List<SubCategoryDto> SubCategories { get; set; } = new List<SubCategoryDto>();
        public List<SlabTariffDto> SlabTariffs { get; set; } = new List<SlabTariffDto>();

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
    }
}

