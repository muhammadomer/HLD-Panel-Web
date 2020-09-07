using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{

    public class AliasViewModel
    {
         

        public int AliaseID { get; set; }
        [Required(ErrorMessage = "Alias Name is required")]
        public String AliaName { get; set; }
    }

}
