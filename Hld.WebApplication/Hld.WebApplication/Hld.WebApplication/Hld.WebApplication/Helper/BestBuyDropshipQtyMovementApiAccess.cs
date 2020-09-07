using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class BestBuyDropshipQtyMovementApiAccess
    {
        //public List<BestBuyDropShipQtyMovementViewModel> GetBestBuyDropshipQtyMovement(string apiurl, string token)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyDropshipQtyMovement/GetListOfAllSKU");
        //    request.Method = "GET";
        //    request.Accept = "application/json;";
        //    request.ContentType = "application/json";
        //    request.Headers["Authorization"] = "Bearer " + token;

        //    string strResponse = "";
        //    using (WebResponse webResponse = request.GetResponse())
        //    {
        //        using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
        //        {
        //            strResponse = stream.ReadToEnd();
        //        }
        //    }
        //    List<BestBuyDropShipQtyMovementViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyDropShipQtyMovementViewModel>>(strResponse);
        //    return responses;
        //}

        public List<BestBuyQTYLogsDetailViewModel> GetBestBuyDropshipQtyMovement(string ApiURL, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyDropshipQtyMovement/GetListOfAllSKU");
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
            List<BestBuyQTYLogsDetailViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyQTYLogsDetailViewModel>>(strResponse);
            return responses;
        }
        public List<BestBuyQTYLogsDetailViewModel> GetBestBuyDropshipQty(string ApiURL, string token, string query)
        {
            SearchQueryViewModel queryViewModel = new SearchQueryViewModel();
            queryViewModel.query = query;
            var data = JsonConvert.SerializeObject(queryViewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyDropshipQtyMovement/GetByQuery");
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
            List<BestBuyQTYLogsDetailViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyQTYLogsDetailViewModel>>(strResponse);
            return responses;
        }
    }
}
