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
    public class ChannelsApiAccess
    {
        public bool UpdateChannelCred(string ApiURL, string token, UpdateChannelsViewModel ViewModel)
        {
            bool status = false;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Channels/save/");
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


        public List<ChannelLogs> GetLogsOfCred(string ApiURL, string token, string Method )
        {
            List<ChannelLogs> ViewModel = new List<ChannelLogs>();
            try
            {
                  
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Channels/"+Method);
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
                ViewModel = JsonConvert.DeserializeObject<List<ChannelLogs>>(strResponse);
                return ViewModel;
                
            }
            catch (Exception)
            {
                throw;
            }
           
        }


        public GetChannelCredViewModel GetCred(string ApiURL, string token, string Method)
        {
            GetChannelCredViewModel ViewModel = new GetChannelCredViewModel();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Channels/dec/" + Method);
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
                ViewModel = JsonConvert.DeserializeObject<GetChannelCredViewModel>(strResponse);
                return ViewModel;

            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
