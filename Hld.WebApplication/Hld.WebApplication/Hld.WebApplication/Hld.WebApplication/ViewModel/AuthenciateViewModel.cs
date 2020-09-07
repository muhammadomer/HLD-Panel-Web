using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class AuthenciateViewModel
    {
        [Required(ErrorMessage = "Please Enter User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [ScaffoldColumn(false)]
        public string Token { get; set; }

        [ScaffoldColumn(false)]
        public DateTime expiration { get; set; }

        [ScaffoldColumn(false)]
        public int UserId { get; set; }

    }
}
