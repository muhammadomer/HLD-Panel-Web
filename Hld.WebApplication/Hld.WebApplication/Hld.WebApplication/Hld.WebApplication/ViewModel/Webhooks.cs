using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class Webhooks
    {
        public string request_succeeded { get; set; }
        public string request_failed { get; set; }
        public string tracking_obtained { get; set; }
    }
}
