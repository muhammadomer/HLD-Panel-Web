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
    public class ExportImgUrlApiAccess
    {
        public string ApiURL { get; private set; }
        public List<ExportSkuImgUrlViewModel> ExportSkuImgUrl(string ApiURL, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Export/ExportSkuImgUrl");
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
            List<ExportSkuImgUrlViewModel> responses = JsonConvert.DeserializeObject<List<ExportSkuImgUrlViewModel>>(strResponse);
            return responses;
        }
    }
}
