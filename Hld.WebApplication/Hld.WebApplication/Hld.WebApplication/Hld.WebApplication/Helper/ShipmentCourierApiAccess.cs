using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class ShipmentCourierApiAccess
    {
        public bool SaveCourier(string ApiURL, string token, SaveAndEditShipmentCourierVM ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCourier/SaveAndEditShipmentCourier/");
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
        public List<SaveAndEditShipmentCourierVM> CourierDetail(string ApiURL, string token)
        {
            List<SaveAndEditShipmentCourierVM> listmodel = new List<SaveAndEditShipmentCourierVM>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCourier/GetShipmentCourierList/");
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
                listmodel = JsonConvert.DeserializeObject<List<SaveAndEditShipmentCourierVM>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }

        public SaveAndEditShipmentCourierVM EditShipmentCourier(string ApiURL, string token, int id)
        {
            SaveAndEditShipmentCourierVM ViewModel = new SaveAndEditShipmentCourierVM();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ShipmentCourier/GetShipmentCourierById?id=" + id);
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
                ViewModel = JsonConvert.DeserializeObject<SaveAndEditShipmentCourierVM>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }
    }
}
