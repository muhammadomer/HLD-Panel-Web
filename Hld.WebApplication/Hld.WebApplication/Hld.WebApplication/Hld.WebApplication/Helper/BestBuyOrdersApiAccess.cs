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
    public class BestBuyOrdersApiAccess
    {/// <summary>
    /// Get Detail for Best Buy Order
    /// </summary>
    /// <param name="apiurl">BB URL</param>
    /// <param name="token">BB Token</param>
    /// <param name="OrderIds">Orders Id to find in Seller Cloud</param>
    /// <returns></returns>
        public  BestBuyRootObject  GetBestBuyOrdersAgainstIDs(string apiurl, string token, List<string> OrderIds)
        {
            string orderIdsTobeSend = String.Join(",", OrderIds);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl+ orderIdsTobeSend);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
             BestBuyRootObject  responses = JsonConvert.DeserializeObject<BestBuyRootObject>(strResponse);
            return responses;
        }



        public bool SaveBestBuyOrders(string apiurl, string token, List<BestBuyOrdersImportMainViewModel> mainViewModel)
        {

            try
            {
                var data = JsonConvert.SerializeObject(mainViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/SaveBestBuyOrders");
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
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
        }
        public bool SaveBestBuyOrderDropShipMovement(string apiurl, string token, BestBuyDropShipQtyMovementViewModel viewModel )
        {

            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/SaveBestBuyOrderDropShipMovement");
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
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
        }

        public List<string> GetBestBuyOrderIdsToImportData(string apiurl, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BestBuyOrders/GetBestBuyOrderIdsFromSellerCloud");
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
            List<string> responses = JsonConvert.DeserializeObject<List<string>>(strResponse);
            return responses;
        }

       

    }
}
