using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Admin.MVCWebUI.Models
{
       public class SlabsAndTariffViewModel
    {


        // Category details
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public decimal? FixedCharges { get; set; }
        public decimal EnergyCharges { get; set; }
        public string? BillingUnits { get; set; }
        public decimal? K11KV { get; set; }
        public decimal? K33KV { get; set; }
        public decimal? K132KV { get; set; }
        public decimal? K220KV { get; set; }
    }
}

