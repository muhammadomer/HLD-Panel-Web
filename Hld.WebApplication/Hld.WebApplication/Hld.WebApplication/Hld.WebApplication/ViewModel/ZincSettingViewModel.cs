using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincSettingViewModel
    {
        // Billing Address *ba*
        public string BA_first_name { get; set; }
        public string BA_last_name { get; set; }
        public string BA_address_line1 { get; set; }
        public string BA_address_line2 { get; set; }
        public string BA_zip_code { get; set; }
        public string BA_city { get; set; }
        public string BA_state { get; set; }
        public string BA_country { get; set; }
        public string BA_phone_number { get; set; }

        // Retailer Credentials *rc*

        public string RC_email { get; set; }
        public string RC_password { get; set; }
        public string RC_2fa_key { get; set; }


        // Payment method *pm*

        public bool pm_use_gift { get; set; }


    }
}

