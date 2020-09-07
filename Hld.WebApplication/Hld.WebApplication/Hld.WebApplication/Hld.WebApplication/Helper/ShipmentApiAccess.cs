using DataAccess.ViewModels;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
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
    public class ShipmentApiAccess
    {
        public bool Create(string ApiURL, string token, ShipmentViewModel ViewModel)
        {
            bool Status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/save");
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
                    string array = JObject.Parse(strResponse)["status"].ToString();
                    Status = Convert.ToBoolean(array);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Status;
        }
        public bool Update(string ApiURL, string token, ShipmentViewModel ViewModel)
        {
            bool Status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/Update");
                request.Method = "PUT";
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
                    string array = JObject.Parse(strResponse)["status"].ToString();
                    Status = Convert.ToBoolean(array);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Status;
        }
        public int GetCounter(string ApiURL, string token, string StartDate, string EndDate, int VendorId, string ShipmentId = "", string TrakingNumber = "", string Status = "", string Type = "")
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/GetCounter?VendorId=" + VendorId + "&ShipmentId=" + ShipmentId + "&TrakingNumber=" + TrakingNumber + "&Status=" + Status + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&Type=" + Type);
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }
        public List<ShipmentGetDataViewModel> GetShipmentList(string apiurl, string token, string StartDate, string EndDate, int VendorId, int Limit, int OffSet, string ShipmentId, string TrakingNumber, string Status, string Type)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/Shipments?VendorId=" + VendorId + "&limit=" + Limit + "&offSet=" + OffSet + "&ShipmentId=" + ShipmentId + "&TrakingNumber=" + TrakingNumber + "&Status=" + Status + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&Type=" + Type);
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
            List<ShipmentGetDataViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentGetDataViewModel>>(strResponse);
            return responses;
        }

        public bool Delete(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/Shipment/Delete?ShipmentId=" + ShipmentId);
            request.Method = "DELETE";
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
            bool responses = Convert.ToBoolean(JObject.Parse(strResponse)["Status"]);
            return responses;
        }

        public bool CreateShipmentCourier(string ApiURL, string token, ShipmentCourierViewModel ViewModel)
        {
            bool Status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCourier/save");
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
                    string array = JObject.Parse(strResponse)["status"].ToString();
                    Status = Convert.ToBoolean(array);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Status;
        }

        public bool DeleteShipmentCourier(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCourier/Delete?ShipmentId=" + ShipmentId);
            request.Method = "DELETE";
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
            bool responses = Convert.ToBoolean(JObject.Parse(strResponse)["Status"]);
            return responses;
        }


        public int ShipmentCount(string ApiURL, string token, string ShipmentId, int POID = 0, string SKU = "", string title = "", string OpenItem = "", string ReceivedItem = "", string OrderdItem = "")
        {
            int count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/ShipmentViewProductlistcount?ShipmentId=" + ShipmentId + "&POID=" + POID + "&SKU=" + SKU + "&Title=" + title + "&OpenItem=" + OpenItem + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem);
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
                count = Convert.ToInt32(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public List<ShipmentViewProducListViewModel> ShipmentListData(string ApiURL, string token, string ShipmentId, int Limit, int OffSet, int POID = 0, string SKU = "", string Title = "", string OpenItem = "", string ReceivedItem = "", string OrderdItem = "")
        {
            List<ShipmentViewProducListViewModel> ViewModel = new List<ShipmentViewProducListViewModel>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/ShipmentViewProductlist?ShipmentId=" + ShipmentId + "&limit=" + Limit + "&offSet=" + OffSet + "&POID=" + POID + "&SKU=" + SKU + "&OpenItem=" + OpenItem + "&Title=" + Title + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem);
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
                ViewModel = JsonConvert.DeserializeObject<List<ShipmentViewProducListViewModel>>(strResponse);
            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }

        public ShipmentViewHeaderViewModel ShipmentViewDetail(string ApiURL, string token, string ShipmentId)
        {
            ShipmentViewHeaderViewModel ViewModel = new ShipmentViewHeaderViewModel();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/Detail?ShipmentId=" + ShipmentId);
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
                ViewModel = JsonConvert.DeserializeObject<ShipmentViewHeaderViewModel>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }

        public List<ProductSKUViewModel> GetAllSKUByName(string apiurl, string token, string name, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Shipment/GetAllSKUByName/" + name + "/" + ShipmentId);
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
            List<ProductSKUViewModel> responses = JsonConvert.DeserializeObject<List<ProductSKUViewModel>>(strResponse);
            return responses;
        }


        public int GetShipmentHistoryCount(string ApiURL, string token, string DateTo, string DateFrom, int VendorId, string ShipmentId, string SKU, string Title, string Status)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/GetShipmentHistoryCount?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&Title=" + Title + "&VendorId=" + VendorId + "&SKU=" + SKU + "&Status=" + Status + "&ShipmentId=" + ShipmentId);
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
                        Count = Convert.ToInt32(strResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }

        public List<ShipmentHistoryViewModel> GetShipmentHistoryList(string apiurl, string token, string DateTo, string DateFrom, int VendorId, string ShipmentId, string SKU, string Title, int limit, int offset, string Status)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/Shipment/GetShipmentHistoryList?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&Title=" + Title + "&SKU=" + SKU + "&Title=" + Title + "&limit=" + limit + "&offset=" + offset + "&VendorId=" + VendorId + "&Status=" + Status + "&ShipmentId=" + ShipmentId);
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
            List<ShipmentHistoryViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentHistoryViewModel>>(strResponse);
            return responses;
        }

        public List<ShipmentHistoryViewModel> GetShipmentHistoryBYSKU(string apiurl, string token, int POID, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/Shipment/GetShipmentHistoryBySKU?SKU=" + SKU + "&POID=" + POID);
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
            List<ShipmentHistoryViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentHistoryViewModel>>(strResponse);
            return responses;
        }
    }
}
