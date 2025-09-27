using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
    public class CategoryCreateDto
    {
        public int? CategoryId { get; set; } // optional, if updating or using existing category
        public string CategoryName { get; set; }
        public string? Category { get; set; }

        public List<SubcategoryCreateDto> Subcategories { get; set; } = new List<SubcategoryCreateDto>();
    }

    public class SubcategoryCreateDto
    {
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string? SubCategory { get; set; }
        public int? SubCategoryNumber { get; set; }
        public string? Lines { get; set; }
        public string? BillingUnits { get; set; }
        public decimal? FixedCharges { get; set; }
        public decimal? CustomerCharges { get; set; }
        public decimal? KV11 { get; set; }
        public decimal? KV33 { get; set; }
        public decimal? KV132 { get; set; }
        public decimal? KV220 { get; set; }
        public decimal? ElectricityDutyCharges { get; set; }
        public decimal? MinimumCharges { get; set; }

        public List<SlabCreateDto> Slabs { get; set; } = new List<SlabCreateDto>();
    }

    public class SlabCreateDto
    {
        public int? SlabTariffId { get; set; }
        //public int? FromUnits { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }

        //public int? ToUnits { get; set; }
        public string? UnitParameter { get; set; }
        public decimal? EnergyCharges { get; set; }
        public DateTimeOffset? CrTs { get; set; }
    }
}

