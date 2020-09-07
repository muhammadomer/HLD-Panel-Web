using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CatageorySub2ViewModel
    {
        public int CatageorySub2Id { get; set; }
        [Required(ErrorMessage = "Catageory 3 is Required")]
        public string CatageorySub2Name { get; set; }
        public int CatageorySub1Id { get; set; }
        public string status { get; set; }
    }
}
