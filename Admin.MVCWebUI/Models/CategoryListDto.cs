namespace Admin.MVCWebUI.Models
{
    public class CategoryListDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? Category { get; set; }
        //public string Category { get; set; }
        //public string CategoryName { get; set; }
        public string SubCategory { get; set; }
        public string SubCategoryName { get; set; }
        public string Lines { get; set; } // <-- Add this

        public List<SlabAndTariffInputModel> SubCategoriesDetails { get; set; }
    }

}