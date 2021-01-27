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
    public class BBOrderApiAccessNew
    {
        public TotalCountWithBestBuyOrderViewModel GetAllBestBuyOrdersDetailGlobalFilterTotalCount(string apiurl, string token, string FilterName, string FilterValue)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/OrdersListGlobalFilterTotalCount/" + FilterName + "/" + FilterValue);
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
            TotalCountWithBestBuyOrderViewModel responses = JsonConvert.DeserializeObject<TotalCountWithBestBuyOrderViewModel>(strResponse);
            return responses;
        }
        public TotalCountWithBestBuyOrderViewModel GetAllBestBuyOrdersDetailSearchingTotalCount(string apiurl, string token, BestBuyOrderSearchTotalCountViewModel viewModel)
        {
            var data = JsonConvert.SerializeObject(viewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/BestBuyOrdersDetailSearchingTotalCount");
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
            TotalCountWithBestBuyOrderViewModel responses = JsonConvert.DeserializeObject<TotalCountWithBestBuyOrderViewModel>(strResponse);
            return responses;
        }
        public List<BestBuyOrdersViewModel> GetAllBestBuyOrdersDetailGlobalFilter(string apiurl, string token, string FilterName, string FilterValue, int startLimit, int endLimit, string sort)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/OrdersListGlobalFilter/" + FilterName + "/" + FilterValue + "/" + startLimit + "/" + endLimit + "/" + sort);
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
            List<BestBuyOrdersViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyOrdersViewModel>>(strResponse);
            return responses;
        }
        public List<BestBuyOrdersViewModel> GetAllBestBuyOrdersDetailSearch(string apiurl, string token, BestBuyOrderSearchTotalCountViewModel viewModel)
        {
            var data = JsonConvert.SerializeObject(viewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/BestBuyOrdersDetailSearch");
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
            List<BestBuyOrdersViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyOrdersViewModel>>(strResponse);
            return responses;
        }
        //,string search_marketplace,string search_shipmentOrderStatus
        public List<BestBuyOrdersViewModel> GetAllBestBuyOrdersDetail(string apiurl, string token, int startlimit, int endlimit, string sort)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/OrdersList/" + startlimit + "/" + endlimit + "/" + sort);
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
            List<BestBuyOrdersViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyOrdersViewModel>>(strResponse);
            return responses;
        }
        public BestBuyOrdersViewPageModel GetOrderViewPage(string apiurl, string token, string OrderID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders//BBOrderViewOrderDetail/" + OrderID);
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

    }
}
