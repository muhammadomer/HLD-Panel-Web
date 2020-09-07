using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincKeyModel
    {
        [Display(Name = "Zinc Key Value:")]
        [Required(ErrorMessage = "please fill this field")]
        public string ZincKeyValue { get; set; }
    }
}
