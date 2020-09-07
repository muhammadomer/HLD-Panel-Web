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
    public class ProductReportApiAccess
    {
        public List<ProductSalesViewModel> GetProductSales(string apiurl, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/ProductSales/PredictList/"+"2020-04-05"+ "/2020-04-10" + "/25"+"/0"+ "/Qty"+ "/desc");
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
            List<ProductSalesViewModel> responses = JsonConvert.DeserializeObject<List<ProductSalesViewModel>>(strResponse);
            return responses;
        }
    }
}
