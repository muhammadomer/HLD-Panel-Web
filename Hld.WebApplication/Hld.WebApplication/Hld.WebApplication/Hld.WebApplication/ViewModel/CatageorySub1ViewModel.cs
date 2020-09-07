using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CatageorySub1ViewModel
    {
        public int CatageorySubId { get; set; }
        [Required(ErrorMessage = "Catageory 2 is Required")]
        public string CatageorySub1Name { get; set; }
        public int CatageoryMainId { get; set; }
        public string status { get; set; }
    }
}
