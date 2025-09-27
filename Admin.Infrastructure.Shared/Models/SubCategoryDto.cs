using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
     public class SubCategoryDto
    {
        public int SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public int? SubCategoryNo { get; set; }
        public string? Lines { get; set; }
    }
}

