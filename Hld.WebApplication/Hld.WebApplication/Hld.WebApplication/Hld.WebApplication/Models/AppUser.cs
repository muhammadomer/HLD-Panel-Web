using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Models
{
    public class AppUser:IdentityUser
    {
        public string UserAlias { get; set; }
    }
}
