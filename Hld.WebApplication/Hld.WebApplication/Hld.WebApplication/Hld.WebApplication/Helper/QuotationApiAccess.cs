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
    public class QuotationApiAccess
    {
        public bool SaveQuotaion(string ApiURL, string token, SaveQuotationMainVM ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Quotation/SaveMainQoute/");
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

        public List<SaveQuotationMainVM> QuotaionDetail(string ApiURL, string token)
        {
            List<SaveQuotationMainVM> listmodel = new List<SaveQuotationMainVM>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Quotation/list/");
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
                listmodel = JsonConvert.DeserializeObject<List<SaveQuotationMainVM>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }
        public string GenrateSKU(string apiurl, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Quotation/GenerateMainSku/");
            request.Method = "POST";
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
            var responses = Convert.ToString(strResponse);
            return responses;
        }
        public bool DeleteQuotaion(string ApiURL, string token, int Quotation_main_id)
        {
            bool status = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Quotation/DeleteMainQoute?Quotation_main_id=" + Quotation_main_id);
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
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
        public SaveQuotationMainVM EditQuotation(string ApiURL, string token, int id)
        {
            SaveQuotationMainVM ViewModel = new SaveQuotationMainVM();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Quotation/Edit?id=" + id);
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
                ViewModel = JsonConvert.DeserializeObject<SaveQuotationMainVM>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }

    }
}
