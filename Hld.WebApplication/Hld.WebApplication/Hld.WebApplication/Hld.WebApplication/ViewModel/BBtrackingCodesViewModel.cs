using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BBtrackingCodesViewModel
    {
        public int IdBBtrackingCodes { get; set; }
      
        public string CarrierCode { get; set; }
        
        public string CarrierName { get; set; }
       
        public string CarrierUrl { get; set; }
        [Required(ErrorMessage = "tracking number reqiured!")]
        public string TrackingNumberCode { get; set; }

    }
}
