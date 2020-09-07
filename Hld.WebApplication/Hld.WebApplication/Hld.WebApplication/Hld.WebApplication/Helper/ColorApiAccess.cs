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
    public class ColorApiAccess
    {
        public List<ColorViewModel> GetAllColors(string ApiURL, string token)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Color");
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
            List<ColorViewModel> responses = JsonConvert.DeserializeObject<List<ColorViewModel>>(strResponse);
            return responses;
        }



        public List<ColorAutoCompleteViewModel> GetAllColorsByName(string apiurl, string token, string name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Color/ByName/" + name + "");
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
            List<ColorAutoCompleteViewModel> responses = JsonConvert.DeserializeObject<List<ColorAutoCompleteViewModel>>(strResponse);
            return responses;
        }

    }
}

