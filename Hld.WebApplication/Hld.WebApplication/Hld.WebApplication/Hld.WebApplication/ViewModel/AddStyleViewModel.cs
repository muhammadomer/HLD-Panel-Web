using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class AddStyleViewModel
    {
        public int StyleId { get; set; }
        [Required(ErrorMessage = "Style Name is required")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters allowed")]
        public string StyleName { get; set; }
    }
}
