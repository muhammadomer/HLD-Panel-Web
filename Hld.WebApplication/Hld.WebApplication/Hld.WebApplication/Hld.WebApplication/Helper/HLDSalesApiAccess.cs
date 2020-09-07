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
    public class HLDSalesApiAccess
    {
        public List<SKUSalesHistoryFromOrders> GetSKU_OrderHistoryBy_SKU(string apiurl, string token, string productSKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/HldHistory/GetSKU_OrderHistoryBy_SKU/" + productSKU);
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
            List<SKUSalesHistoryFromOrders> responses = JsonConvert.DeserializeObject<List<SKUSalesHistoryFromOrders>>(strResponse);
            return responses;
        }

        public List<Order_SKU_ProfitHistory_CalculationViewmodel> GetSkuProfitHistory(string apiurl, string token, string productSKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/HldHistory/GetSkuProfitHistory/" + productSKU);
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
            List<Order_SKU_ProfitHistory_CalculationViewmodel> responses = JsonConvert.DeserializeObject<List<Order_SKU_ProfitHistory_CalculationViewmodel>>(strResponse);
            return responses;
        }

        public List<Order_SKU_ProfitHistory_CalculationViewmodel> GetSlaesHistoryForDashBoardByDate(string apiurl, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/HldHistory/GetSlaesHistoryForDashBoardByDate");
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
            List<Order_SKU_ProfitHistory_CalculationViewmodel> responses = JsonConvert.DeserializeObject<List<Order_SKU_ProfitHistory_CalculationViewmodel>>(strResponse);
            return responses;
        }


        public Order_SKU_ProfitHistory_CalculationViewmodel GetSlaesHistoryForDashBoardCustomRange(string apiurl, string token, SalesHistoryDashboardSearchViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/HldHistory/GetSlaesHistoryForDashBoardCustomRange");
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
            Order_SKU_ProfitHistory_CalculationViewmodel responses = JsonConvert.DeserializeObject<Order_SKU_ProfitHistory_CalculationViewmodel>(strResponse);
            return responses;
        }
    }
}



