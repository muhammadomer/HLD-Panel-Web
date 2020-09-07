
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
using jQWidgets.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class ApprovedPriceApiAccess
    {
        public int ApprovedPrice(string ApiURL, string token, ApprovedPriceViewModel ViewModel)
        {
            int productId = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ApprovedPrice/save");
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
                    string array = JObject.Parse(strResponse)["approvedPriceID"].ToString();
                    productId = int.Parse(array);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return productId;
        }
        public bool UpdatePrice(string ApiURL, string token, ApprovedPriceViewModel ViewModel)
        {
            bool productId = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ApprovedPrice/Update");
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
                productId = Convert.ToBoolean(strResponse);
            }
            catch (Exception ex)
            {

                throw;
            }
            return productId;
        }
        public List<ApprovedPriceViewModel> GetApprovedPriceLogs(string apiurl, string token, int VendorId, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ApprovedPrice/Logs?VendorId=" + VendorId + "&SKU=" + SKU);
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
            //List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            return responses;
        }

        public List<ApprovedPriceViewModel> GetApprovedPriceList(string apiurl, string token, int VendorId, int Limit, int OffSet, string SKU, string Title, string skuList = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/ApprovedPrice?VendorId=" + VendorId + "&limit=" + Limit + "&offSet=" + OffSet + "&SKU=" + SKU + "&Title=" + Title + "&skuList=" + skuList);
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
            //List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            return responses;
        }
        public int GetCounter(string ApiURL, string token, int VendorId, string SKU, string Title, string skuList = "")
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ApprovedPrice/GetCounter?VendorId=" + VendorId + "&SKU=" + SKU + "&title=" + Title + "&skuList=" + skuList);
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
                        Count = (int)JObject.Parse(strResponse)["counter"];
                        //Count = int.Parse(array);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Count;
        }

        public List<GetVendorListViewModel> GetAllVendorByName(string apiurl, string token, string name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/ApprovedPrice/Vendor/" + name + "");
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
            List<GetVendorListViewModel> responses = JsonConvert.DeserializeObject<List<GetVendorListViewModel>>(strResponse);
            return responses;
        }
        public ApprovedPriceViewModel GetApprovedPricesForedit(string apiurl, string token, int name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/ApprovedPrice/Edit/" + name + "");
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
            ApprovedPriceViewModel responses = JsonConvert.DeserializeObject<ApprovedPriceViewModel>(strResponse);
            return responses;
        }
        public bool AddNotesInApprovedPrice(string ApiURL, string token, ApprovedPriceViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ApprovedPrice/AddNotesInApprovedPrice");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
    }
}
