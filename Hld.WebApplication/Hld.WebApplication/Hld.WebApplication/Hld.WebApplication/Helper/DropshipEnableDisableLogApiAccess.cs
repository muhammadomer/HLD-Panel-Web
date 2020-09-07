using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class DropshipEnableDisableLogApiAccess
    {
        public List<DropShipEnableDisableLogViewModel> GetDropshipEnableDisableLog_SKU(string apiurl, string token,string sku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/DropshipEnableDisableLog/"+sku+"");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<DropShipEnableDisableLogViewModel> responses = JsonConvert.DeserializeObject<List<DropShipEnableDisableLogViewModel>>(strResponse);
            return responses;
        }
    }
}
