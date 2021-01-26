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
    public class BBProductApiAccess
    {
        public bool AddBestBuyProduct(string ApiURL, string token, BBProductViewModel ViewModel)
        {
            bool status = false;
            try
            {
                if (ViewModel.BBProductID == 0)
                {
                    var data = JsonConvert.SerializeObject(ViewModel);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BBProduct/save");
                    request.Method = "POST";
                    request.Accept = "application/json;";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + token;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    var response = (HttpWebResponse)request.GetResponse();
                    string strResponse = "";
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        strResponse = sr.ReadToEnd();
                        status = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return status;
        }

        public List<BBProductViewModel> GetAllBestBuyProducts(string apiurl, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProduct");
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
            List<BBProductViewModel> responses = JsonConvert.DeserializeObject<List<BBProductViewModel>>(strResponse);
            return responses;
        }

        public  TotalCountWithBestBuyOrderViewModel  GetAllBestBuyOrdersDetailGlobalFilterTotalCount(string apiurl, string token, string FilterName, string FilterValue)
        {
           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProduct/OrdersListGlobalFilterTotalCount/" + FilterName + "/" + FilterValue);
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
             TotalCountWithBestBuyOrderViewModel  responses = JsonConvert.DeserializeObject< TotalCountWithBestBuyOrderViewModel>(strResponse);
            return responses;
        }


        public TotalCountWithBestBuyOrderViewModel GetAllBestBuyOrdersDetailSearchingTotalCount(string apiurl, string token, BestBuyOrderSearchTotalCountViewModel viewModel)
        {
            var data = JsonConvert.SerializeObject(viewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProduct/BestBuyOrdersDetailSearchingTotalCount");
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



        //,string search_marketplace,string search_shipmentOrderStatus
        public List<BestBuyOrdersViewModel> GetAllBestBuyOrdersDetail(string apiurl, string token,int startlimit,int endlimit,string sort)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProduct/OrdersList/"+startlimit+"/"+endlimit+"/"+sort);
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

        public List<BestBuyOrdersViewModel> GetAllBestBuyOrdersDetailSearch(string apiurl, string token,BestBuyOrderSearchTotalCountViewModel viewModel)
        {
            var data = JsonConvert.SerializeObject(viewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProduct/BestBuyOrdersDetailSearch");
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





        public List<BestBuyOrdersViewModel> GetAllBestBuyOrdersDetailGlobalFilter(string apiurl, string token,string FilterName,string FilterValue,int startLimit,int endLimit,string sort)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProduct/OrdersListGlobalFilter/"+FilterName+"/"+FilterValue+"/"+startLimit+"/"+endLimit+"/"+sort);
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

        public BBProductViewModel GetBestBuyProductDetail_ProductID(string ApiURL, string token, string id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BBProduct/" + id + "");
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
            BBProductViewModel responses = JsonConvert.DeserializeObject<BBProductViewModel>(strResponse);

            return responses;
        } 
        public BBProductViewModel BBProductByBBSKU(string ApiURL, string token, string id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BBProduct/BBSKU/" + id + "");
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
            BBProductViewModel responses = JsonConvert.DeserializeObject<BBProductViewModel>(strResponse);

            return responses;
        }

        public List<BestBuyUpdateViewModel> bestBuyUpdateLogs(string ApiURL, string token, int offset)
        {
            List<BestBuyUpdateViewModel> listmodel = new List<BestBuyUpdateViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyProduct/GetBestBuyUpdate/");
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
                listmodel = JsonConvert.DeserializeObject<List<BestBuyUpdateViewModel>>(strResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listmodel;
        }
        public int GetWatchListSummaryCount(string ApiURL, string token)
        {
            int count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyProduct/getcount");
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                    count = Convert.ToInt32(strResponse);
                }
                
            }
            catch (Exception exp)
            {
                throw;
            }
            return count;
        }


    }
}
