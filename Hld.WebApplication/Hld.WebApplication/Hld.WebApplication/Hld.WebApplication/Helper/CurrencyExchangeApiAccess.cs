using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;

namespace Hld.WebApplication.Helper
{
    public class CurrencyExchangeApiAccess
    {
        public bool UpdateZincProductASIN(string ApiURL, string token, CurrencyExchangeViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CurrencyExchange/UpdateCurrencyExchange");
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
            return Convert.ToBoolean(strResponse);
        }

        public bool SaveCurrencyExchange(string ApiURL, string token, CurrencyExchangeViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CurrencyExchange/SaveCurrencyExchange");
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
            return Convert.ToBoolean(strResponse);
        }

        public List<CurrencyExchangeViewModel> GetCurrencyExchangeList(string ApiURL, string token)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CurrencyExchange/GetCurrencyExchangeList");
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
            List<CurrencyExchangeViewModel> responses = JsonConvert.DeserializeObject<List<CurrencyExchangeViewModel>>(strResponse);

            return responses;
        }

        public double GetLatestCurrencyRate(string ApiURL, string token)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CurrencyExchange/GetLatestCurrencyRate");
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
            return Convert.ToDouble(strResponse);
        }
        
        public CurrencyExchangeViewModel GetCurrencyExchangeByID(string ApiURL, string token, int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CurrencyExchange/GetCurrencyExchangeById/" + id);
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
                CurrencyExchangeViewModel responses = JsonConvert.DeserializeObject<CurrencyExchangeViewModel>(strResponse);
                return responses;
            }
        }
    }
}
