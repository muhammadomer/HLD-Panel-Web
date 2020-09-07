using DataAccess.ViewModels;
using Hld.WebApplication.ViewModel;
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
    public class ShipmentCasePackApiAccess
    {
        public int Create(string ApiURL, string token, ShipmentCasePackProductViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCasePackProduct/saveCasePack");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }

        public int Update(string ApiURL, string token, ShipmentCasePackProductViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCasePackProduct/Update");
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

        public CasePackViewModel GetTemplate(string apiurl, string token, int VendorId, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/GetTemplateCasePack?VendorId=" + VendorId + "&SKU=" + SKU);
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
            CasePackViewModel responses = JsonConvert.DeserializeObject<CasePackViewModel>(strResponse);
            return responses;
        }

        public List<ShipmentCasePackProductViewModel> GetShipmentCasePackProducts(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/GetShipmentCasePackProducts?ShipmentId=" + ShipmentId);
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
            List<ShipmentCasePackProductViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentCasePackProductViewModel>>(strResponse);
            return responses;
        }

        public int Delete(string apiurl, string token, int Id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/Delete?Id=" + Id);
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
            int responses = Convert.ToInt32(strResponse);
            return responses;
        }

        public ShipmentCasePackHeaderViewModel GetShipmentCasePackProductHeader(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/GetShipmentCasePackProductHeader?ShipmentId=" + ShipmentId);
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
            ShipmentCasePackHeaderViewModel responses = JsonConvert.DeserializeObject<ShipmentCasePackHeaderViewModel>(strResponse);
            return responses;
        }

        public ShipmentViewHeaderViewModel GetShipmentViewCasePackHeader(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/GetShipmentViewCasePackHeader?ShipmentId=" + ShipmentId);
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
            ShipmentViewHeaderViewModel responses = JsonConvert.DeserializeObject<ShipmentViewHeaderViewModel>(strResponse);
            return responses;
        }

        public List<ShipmentViewProducListViewModel> GetShipmentViewProductCasPackList(string apiurl, string token, string ShipmentId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/GetShipmentViewProductCasPackList?ShipmentId=" + ShipmentId);
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
            List<ShipmentViewProducListViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentViewProducListViewModel>>(strResponse);
            return responses;
        }

        public int SaveShipmentSKUCasePackTemplate(string ApiURL, string token, CasePackViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCasePackProduct/saveCasePackTemplate");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Id;
        }

        public int GetTemplateCounter(string ApiURL, string token, int VendorId, string SKU, string Title)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCasePackProduct/GetTemplateCasePackCount?VendorId=" + VendorId + "&SKU=" + SKU + "&Title=" + Title);
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

        public List<CasePackViewModel> GetTemplateList(string apiurl, string token, int VendorId, string SKU, string Title, int limit, int offset)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/GetTemplateCasePackList?VendorId=" + VendorId + "&SKU=" + SKU + "&Title=" + Title + "&limit=" + limit + "&offset=" + offset);
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
            List<CasePackViewModel> responses = JsonConvert.DeserializeObject<List<CasePackViewModel>>(strResponse);
            return responses;
        }

        public int DeleteCasePack(string apiurl, string token, int Id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentCasePackProduct/DeleteCasePack?Id=" + Id);
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
            int responses = Convert.ToInt32(strResponse);
            return responses;
        }

    }
}
