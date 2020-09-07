using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CatageorySub3ViewModel
    {
        public int CatageorySub3Id { get; set; }
        [Required(ErrorMessage = "Catageory 4 is Required")]
        public string CatageorySub3Name { get; set; }
        public int CatageorySub2Id { get; set; }
        public string status { get; set; }
    }
}
