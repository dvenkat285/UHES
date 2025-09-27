using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
    public class SlabAndTariffInputModel
    {
        public int? SubCategoryId { get; set; }
        public int? CategoryId { get; set; }
        public int SlabTariffId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public decimal? FixedCharges { get; set; }
        public decimal Rate { get; set; }
        public string Lines { get; set; }
        public string BillingUnits { get; set; }
        public decimal _11KV { get; set; }
        public decimal _33KV { get; set; }
        public decimal _132KV { get; set; }
        public decimal _220KV { get; set; }
        public decimal? CustomerCharges { get; set; }
        public decimal? ElectricityDutyCharges { get; set; }
        public decimal? MinimumCharges { get; set; }
    }

}
