using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProxeyResponseViewModel
    {
        public string id { get; set; }
        public string proxy_id { get; set; }
        public DateTime created_at { get; set; }
        public string external_id { get; set; }
        public string website { get; set; }
        public bool active { get; set; }
        public string user_id { get; set; }
        public bool dont_auto_rotate { get; set; }
        public string state { get; set; }
        public string provider { get; set; }
        public string provider_region { get; set; }
        public string provider_id { get; set; }
        public string host { get; set; }
        public string auth_user { get; set; }
        public string auth_password { get; set; }
        public IList<string> connstrs { get; set; }
    }
}