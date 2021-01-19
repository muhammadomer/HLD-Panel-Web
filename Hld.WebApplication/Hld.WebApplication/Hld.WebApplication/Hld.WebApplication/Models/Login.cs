using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Models
{
    public class Login
    {
        [Required]
        public string Email { get; set; }

        
        public string Password { get; set; }
        public bool Checkboxstatus { get; set; }

       
    }
}
