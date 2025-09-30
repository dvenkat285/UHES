namespace Admin.MVCWebUI.Models
{
    public class SlabWrapperUnifiedUpdateModel
    {
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Category { get; set; }
        public List<SubcategoryModel> Subcategories { get; set; }
    }

    public class SubcategoryModel
    {
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategory { get; set; }
        public int? SubCategoryNumber { get; set; }  // if you have it
        public string Lines { get; set; }
        public string BillingUnits { get; set; }
        public decimal? FixedCharges { get; set; }
        public decimal? CustomerCharges { get; set; }
        public decimal? KV11 { get; set; }
        public decimal? KV33 { get; set; }
        public decimal? KV132 { get; set; }
        public decimal? KV220 { get; set; }
        public decimal? ElectricityDutyCharges { get; set; }
        public decimal? MinimumCharges { get; set; }
        public List<SlabModel> Slabs { get; set; }
    }

    public class SlabModel
    {
        public int? SlabTariffId { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string UnitParameter { get; set; }
        public decimal? EnergyCharges { get; set; }
    }
}