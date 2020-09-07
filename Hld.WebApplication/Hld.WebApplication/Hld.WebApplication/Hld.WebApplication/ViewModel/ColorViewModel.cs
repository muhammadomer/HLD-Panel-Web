using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ColorViewModel
    {
        public int ColorId { get; set; }
        [Required(ErrorMessage = "Color Name is Required")]
        public string ColorName { get; set; }
        [Required(ErrorMessage = "Color Code is Required")]
        public string ColorCode { get; set; }
        [Required(ErrorMessage = "Color Alias is Required")]
        public string ColorAlias { get; set; }
    }
}
