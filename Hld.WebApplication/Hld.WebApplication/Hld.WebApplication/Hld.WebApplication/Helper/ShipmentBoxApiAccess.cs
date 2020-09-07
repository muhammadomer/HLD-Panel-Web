using DataAccess.ViewModels;
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
    public class ShipmentBoxApiAccess
    {
        public string Create(string ApiURL, string token, ShipmentBoxViewModel ViewModel)
        {
            string Id = "";
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentBox/save");
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
                    Id = JObject.Parse(strResponse)["id"].ToString();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Id;
        }

        public bool Update(string ApiURL, string token, ShipmentBoxViewModel ViewModel)
        {
            bool Status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentBox/Update");
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

        public List<ShipmentBoxListViewModel> GetList(string apiurl, string token, int VendorId, int Limit, int OffSet)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentBoxs?VendorId=" + VendorId + "&limit=" + Limit + "&offSet=" + OffSet);
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
            List<ShipmentBoxListViewModel> responses = JsonConvert.DeserializeObject<List<ShipmentBoxListViewModel>>(strResponse);
            return responses;
        }

        public ShipmentHeaderViewModel GetListbyShipemntId(string apiurl, string token, string ShipmentId)
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
            ShipmentHeaderViewModel responses = JsonConvert.DeserializeObject<ShipmentHeaderViewModel>(strResponse);
            return responses;
        }

        public bool Delete(string apiurl, string token, string BoxId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentBox/Delete?Id=" + BoxId);
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
            bool responses = Convert.ToBoolean(JObject.Parse(strResponse)["id"]);
            return responses;
        }
        public ShipmentBoxDetailViewModel GetBoxDeatilById(string apiurl, string token, string BoxId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ShipmentBox/GetBoxDetailById?BoxId=" + BoxId);
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
            ShipmentBoxDetailViewModel responses = JsonConvert.DeserializeObject<ShipmentBoxDetailViewModel>(strResponse);
            return responses;
        }

    }
}
