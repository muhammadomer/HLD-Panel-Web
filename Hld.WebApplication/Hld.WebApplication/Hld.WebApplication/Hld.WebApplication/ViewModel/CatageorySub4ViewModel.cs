using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CatageorySub4ViewModel
    {
        public int CatageorySub4Id { get; set; }
        [Required(ErrorMessage = "Catageory 5 is Required")]
        public string CatageorySub4Name { get; set; }
        public int CatageorySub3Id { get; set; }
        public string status { get; set; }
    }
}
