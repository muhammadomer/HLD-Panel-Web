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
    public class ZincOrderLogAndDetailApiAccess
    {
        public int SaveZincOrderLog(string ApiURL, string token, ZincOrderLogViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincOrderLog/SaveZincOrderLog");
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
            return Convert.ToInt32(strResponse);
        }



        public int SaveZincOrderLogDetail(string ApiURL, string token, ZincOrderLogDetailViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincOrderLog/SaveZincOrderLogDetail");
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
            return Convert.ToInt32(strResponse);
        }


        public ZincOrderLogDetailViewModel GetZincOrderLogDetailById(string ApiURL, string token, string zincOrderLogDetailId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincOrderLog/GetZincOrderLogDetailById/" + zincOrderLogDetailId + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;


            var response = (HttpWebResponse)request.GetResponse();
            string strResponse = "";
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                strResponse = sr.ReadToEnd();
            }
            ZincOrderLogDetailViewModel responses = JsonConvert.DeserializeObject<ZincOrderLogDetailViewModel>(strResponse);
            return responses;
        }

        public bool UpdateAccounts(string ApiURL, string token, int Id, int ZincAccountId, int CreditCardId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/UpdateAccounts?Id=" + Id + "&ZincAccountId=" + ZincAccountId + "&CreditCardId=" + CreditCardId);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;


            var response = (HttpWebResponse)request.GetResponse();
            string strResponse = "";
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                strResponse = sr.ReadToEnd();
            }
            bool resp = Convert.ToBoolean(strResponse);
            return resp;
        }

    }
}
