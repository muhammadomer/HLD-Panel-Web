using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class BestBuyTrackingUpdateLogApiAccess
    {
        public List<BestBuyTrackingUpdate> GetTrackingData(string ApiURL, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyTrackingUpdateLog");
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
            List<BestBuyTrackingUpdate> responses = JsonConvert.DeserializeObject<List<BestBuyTrackingUpdate>>(strResponse);
            return responses;
        }

        public List<BestBuyTrackingUpdate> GetTrackingDynamicQuery(string ApiURL, string token, string query)
        {
            SearchQueryViewModel queryViewModel = new SearchQueryViewModel();
            queryViewModel.query = query;
            var data = JsonConvert.SerializeObject(queryViewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyTrackingUpdateLog");
            request.Method = "POST";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            string strResponse = "";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<BestBuyTrackingUpdate> responses = JsonConvert.DeserializeObject<List<BestBuyTrackingUpdate>>(strResponse);
            return responses;
        }
    }


}
