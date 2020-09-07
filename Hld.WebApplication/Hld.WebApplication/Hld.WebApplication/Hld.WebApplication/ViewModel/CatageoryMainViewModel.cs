using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CatageoryMainViewModel
    {
        public int CatageoryMainId { get; set; }
        [Required(ErrorMessage ="Catageory 1 is Required")]
        public string CatageoryMainName { get; set; }
        public string status { get; set; }
    }
}
