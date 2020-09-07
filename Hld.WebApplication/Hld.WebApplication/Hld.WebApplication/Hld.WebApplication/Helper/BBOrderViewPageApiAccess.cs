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
    public class BBOrderViewPageApiAccess
    {
        public BestBuyOrdersViewPageModel GetOrderViewPage(string apiurl, string token, string OrderID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/SCOrderPageViewOrderDetail/" + OrderID);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            BestBuyOrdersViewPageModel responses = JsonConvert.DeserializeObject<BestBuyOrdersViewPageModel>(strResponse);
            return responses;
        }


        public decimal GetCountCad(string ApiURL, string token)
        {
            decimal CadPrice = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CurrencyExchange/GetLatestCurrencyRate");
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                CadPrice = JsonConvert.DeserializeObject<decimal>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return CadPrice;
        }
    }
}
