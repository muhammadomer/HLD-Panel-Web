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
    public class ShipmentProductApiAccess
    {
        public int Create(string ApiURL, string token, ShipmentProductViewModel ViewModel)
        {
            int status = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/save");
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
                    status = Convert.ToInt32(JObject.Parse(strResponse)["id"]);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return status;
        }

        public int Update(string ApiURL, string token, ShipmentProductViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/Update");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }

        public List<ShipmentProductListViewModel> GetList(string apiurl, string token, int VendorId, int Limit, int OffSet)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentProducts?VendorId=" + VendorId + "&limit=" + Limit + "&offSet=" + OffSet);
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
            List<ShipmentProductListViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentProductListViewModel>>(strResponse);
            return responses;
        }

        public ShipmentProductHeaderViewModel GetHeaderWithList(string apiurl, string token, string BoxId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentProducts/GetListByBoxId?BoxId=" + BoxId);
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
            ShipmentProductHeaderViewModel responses = JsonConvert.DeserializeObject<ShipmentProductHeaderViewModel>(strResponse);
            return responses;
        }

        public ShipmentProductViewModel GetListbyShipemntId(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentByShipmentId?ShipmentId=" + ShipmentId);
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
            ShipmentProductViewModel responses = JsonConvert.DeserializeObject<ShipmentProductViewModel>(strResponse);
            return responses;
        }

        public bool Delete(string apiurl, string token, int Id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentProduct/Delete?Id=" + Id);
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
            bool responses = Convert.ToBoolean(strResponse);
            return responses;
        }

        public int GetCounterByBarcode(string ApiURL, string token, string BoxId)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/GetCounterByBarcode?BoxId=" + BoxId);
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

        public List<ShipmentProductListViewModel> GetListByBarcode(string apiurl, string token, string BoxId, int Limit, int OffSet)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentProducts/GetByBarcode?BoxId=" + BoxId + "&limit=" + Limit + "&offSet=" + OffSet);
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
            List<ShipmentProductListViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentProductListViewModel>>(strResponse);
            return responses;
        }

        public int UpdateReceivedQty(string ApiURL, string token, ShipmentProductListViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/UpdateRecivedQty");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }

        public int GetPOIID(string ApiURL, string token, int ShipmentProductId)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/GetPOIID?ShipmentProductId=" + ShipmentProductId);
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

        public string SellerCloudUpdateReceivedQty(string ApiURL, string token, POItemRecivedViewModel ViewModel, int POId)
        {
            string Message = "Ok";
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/PurchaseOrders/" + POId + "/receive");
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
                    //status = Convert.ToInt32(JObject.Parse(strResponse)["id"]);
                }
            }
            catch (Exception ex)
            {
                Message = ex.ToString();
                throw;
            }
            return Message;
        }

        public ShipmentProductInventroyViewModel GetSCInventoryBySKU(string ApiURL, string token, string SKU)
        {
            ShipmentProductInventroyViewModel responses = new ShipmentProductInventroyViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest
                    .Create(ApiURL + "/Inventory/" + SKU);
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
                responses = JsonConvert.DeserializeObject<ShipmentProductInventroyViewModel>(strResponse);

            }
            catch (Exception exp)
            {
                throw;
            }
            return responses;
        }

        public int UpdateShipmentProductInventory(string ApiURL, string token, ShipmentProductListViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/UpdateShipmentProductInventory");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }



        public int UpdateShipmentStatus(string ApiURL, string token, ShipmentViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/UpdateShipmentStatus");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }

        public int SetShipmentasReceived(string ApiURL, string token, ShipmentViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentProduct/SetShipmentasReceived");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }

    }
}
