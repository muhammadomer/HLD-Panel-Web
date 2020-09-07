using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class BestBuyProductZincApiAccess
    {
        public bool AddBestBuyProductZinc(string ApiURL, string token, BestBuyProductZincViewModel ViewModel)
        {
            bool status = false;
            try
            {
                    var data = JsonConvert.SerializeObject(ViewModel);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BBProductZinc/save");
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
                        status = true;
                    }
                }
           
            catch (Exception)
            {
                throw;
            }
            return status;
        }

        public List<BestBuyProductZincViewModel> GetAllBestBuyProductZincByName(string apiurl, string token, string name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/BBProductZinc/ByName/" + name + "");
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
            List<BestBuyProductZincViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyProductZincViewModel>>(strResponse);
            return responses;
        }
    }
}
