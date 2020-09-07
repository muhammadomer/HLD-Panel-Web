using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class PurchaseModel
    {
        [Required(ErrorMessage = "Please enter orders")]
        public string Order { get; set; }
    }
}
