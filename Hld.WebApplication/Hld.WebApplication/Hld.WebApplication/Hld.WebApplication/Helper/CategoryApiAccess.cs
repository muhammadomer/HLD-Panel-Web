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
    public class CategoryApiAccess
    {

        public List<CategoriesAutoCompleteViewModel> GetListOfCategoriesForAutoComplete(string categoryName, string ApiURL, string token)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/AllCategoriesForAutoComplete/" + categoryName + "");
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
            List<CategoriesAutoCompleteViewModel> responses = JsonConvert.DeserializeObject<List<CategoriesAutoCompleteViewModel>>(strResponse);
            return responses;
        }
    }
}
