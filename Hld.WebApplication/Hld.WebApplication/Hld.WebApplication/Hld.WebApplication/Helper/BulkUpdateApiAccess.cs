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
    public class BulkUpdateApiAccess
    {
        public List<BulkUpdateViewModel> GetBulkUpdate(string apiurl, string token,string shadowSku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BulkUpdate/GetBulkUpdate/" + shadowSku + "");
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
            List<BulkUpdateViewModel> responses = JsonConvert.DeserializeObject<List<BulkUpdateViewModel>>(strResponse);
            return responses;
        }
    }
}
