using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Admin.Infrastructure.Shared.Models
{
   public class SlabsDetailsModel
    {

        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(20, ErrorMessage = "Category name can't exceed 20 characters")]
        public string CategoryName { get; set; }

        public string? Category { get; set; }

        [Required(ErrorMessage = "Subcategory ID is required")]
        public int? SubCategoryId { get; set; }

        public string? SubCategory { get; set; }

        [Required(ErrorMessage = "Subcategory name is required")]
        [StringLength(25, ErrorMessage = "Subcategory name can't exceed 25 characters")]
        public string SubCategoryName { get; set; }

        [Range(100, int.MaxValue, ErrorMessage = "Subcategory number must be a positive integer")]
        public int? SubCategoryNumber { get; set; }

        [Required(ErrorMessage = "From Units is required")]
        [Range(0, int.MaxValue, ErrorMessage = "From Units must be a non-negative number")]
        public int? FromUnits { get; set; }

        [Required(ErrorMessage = "To Units is required")]
        [Range(0, int.MaxValue, ErrorMessage = "To Units must be a non-negative number")]
        public int? ToUnits { get; set; }

        public string? Lines { get; set; }

        public string? UnitParameter { get; set; }

        [Required(ErrorMessage = "Energy charges are required")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Energy charges must be a positive number")]
        public decimal? EnergyCharges { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Fixed charges must be a positive number")]
        public decimal? FixedCharges { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Customer charges must be a positive number")]
        public decimal? CustomerCharges { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "KV11 value must be positive")]
        public decimal? Kv11 { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "KV33 value must be positive")]
        public decimal? Kv33 { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "KV132 value must be positive")]
        public decimal? Kv132 { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "KV220 value must be positive")]
        public decimal? Kv220 { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Electricity Duty Charges must be positive")]
        public decimal? ElectricityDutyCharges { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Minimum Charges must be positive")]
        public decimal? MinimumCharges { get; set; }

        public int SlabTariffId { get; set; }

        public List<SlabsDetailsModel> SlabTariffDetails { get; set; } = new List<SlabsDetailsModel>();
    }
}



