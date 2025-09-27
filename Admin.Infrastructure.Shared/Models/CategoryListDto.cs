using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
     public class CategoryListDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? Category { get; set; }
        public object SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public object Lines { get; set; }
        public string SubCategory { get; set; }
    }
}

