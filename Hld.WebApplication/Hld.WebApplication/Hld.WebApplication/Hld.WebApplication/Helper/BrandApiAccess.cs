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
    public class BrandApiAccess
    {
        public List<BrandViewModel> GetAllBrands(string apiurl,string token)
        {                       
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Brand");
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
            List<BrandViewModel> responses = JsonConvert.DeserializeObject<List<BrandViewModel>>(strResponse);
            return responses;
        }

        public List<BrandViewModel> GetAllBrandsByName(string apiurl, string token,string name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Brand/ByName/"+name+"");
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
            List<BrandViewModel> responses = JsonConvert.DeserializeObject<List<BrandViewModel>>(strResponse);
            return responses;
        }

        
    }
}
