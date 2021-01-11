using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincProductSaveViewModel
    {
        public int timestemp { get; set; }
        public string status { get; set; }
        public int bb_product_zinc_id { get; set; }
        public string sellerName { get; set; }
        public int percent_positive { get; set; }
        public decimal itemprice { get; set; }
        public bool itemavailable { get; set; }
        public int handlingday_min { get; set; }
        public int handlingday_max { get; set; }
        public bool item_prime_badge { get; set; }
        public bool item_fba_badge { get; set; }
        public int delivery_days_max { get; set; }
        public int delivery_days_min { get; set; }
        public string item_condition { get; set; }
        [Required(ErrorMessage = "ASIN is Required")]
        public string  ASIN { get; set; }
        public string  Product_sku { get; set; }
        public string  Priorty { get; set; }
        [Required(ErrorMessage = "Max Price is Requried")]
        public string max_price_limit { get; set; }
        public string primeAvailable { get; set; }
        public DateTime updateDate { get; set; }
        public int ValidStatus { get; set; }
        public int Frequency { get; set; }
        public List<ProductSkuFromAsinViewModel> listSKU { get; set; }
    }
}
