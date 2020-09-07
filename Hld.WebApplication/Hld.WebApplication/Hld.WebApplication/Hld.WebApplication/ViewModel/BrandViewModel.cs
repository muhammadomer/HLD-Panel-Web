using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BrandViewModel
    {
        public int BrandId { get; set; }
        [Required(ErrorMessage ="Brand Name is Required")]
        [Display(Name ="Name")]
        public string BrandName { get; set; }
    }
}
