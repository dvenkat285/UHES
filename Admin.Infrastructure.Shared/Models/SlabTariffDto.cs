using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Infrastructure.Shared.Models
{
      public class SlabTariffDto
    {
        public int SlabTariffId { get; set; }
        public decimal? FromUnits { get; set; }
        public decimal? ToUnits { get; set; }
        public decimal? EnergyCharges { get; set; }
        public decimal? FixedCharges { get; set; }
        public decimal? CustomerCharges { get; set; }
        public string? Lines { get; set; }
        // Add more if needed: KV11, KV33, etc.
    }
}

