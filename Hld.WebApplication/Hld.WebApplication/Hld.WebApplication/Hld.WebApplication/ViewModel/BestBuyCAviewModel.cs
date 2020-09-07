using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyCAviewModel
    {
        [Display(Name = "Best Buy Key:")]
        [Required(ErrorMessage = "please fill this field")]
        public string BestBuyCAKeyValue { get; set; }
    }
}
