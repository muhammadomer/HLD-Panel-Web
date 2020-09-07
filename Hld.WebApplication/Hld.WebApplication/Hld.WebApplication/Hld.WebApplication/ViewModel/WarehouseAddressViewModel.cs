using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class WarehouseAddressViewModel
    {
        public int? ID { get; set; }
        [Required]
        [Display(Name = "WH Name")]
        public string whname { get; set; }
       

        [Required]
        [Display(Name = "WH ID ")]

        public string whid { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string companyname { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
       
        public string street1 { get; set; }

        [Required]
        [Display(Name = "Address Line 2")]
       
        public string street2 { get; set; }
        [Required]
        [Display(Name = "Unit")]
        
        public string unit { get; set; }
        [Required]
        [Display(Name = "Postel Code")]
       
        public string postelcode { get; set; }
        [Required]
        [Display(Name = "City")]
      
        public string city { get; set; }
        [Required]
        [Display(Name = "State")]
        
        public string state { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string country { get; set; }
        [Display(Name = "Phone.no")]
        [Required]
        public string phone { get; set; }
    }
}
