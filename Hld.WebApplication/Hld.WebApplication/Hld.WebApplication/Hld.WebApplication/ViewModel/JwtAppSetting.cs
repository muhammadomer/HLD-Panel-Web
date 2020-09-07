using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class JwtAppSetting
    {
        public string SigningKey { get; set; }
        public string Site { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}
