using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class MissingSkuViewModel
    {
        [Required(ErrorMessage = "Please enter Sku list")]
        public string Sku { get; set; }
    }
}
