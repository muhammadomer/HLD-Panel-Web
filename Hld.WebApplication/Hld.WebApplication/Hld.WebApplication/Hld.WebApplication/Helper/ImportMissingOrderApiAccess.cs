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
    public class ImportMissingOrderApiAccess
    {
        public MissingOrderReturnViewModel CheckOrderINDB(string ApiURL, string token, List<CheckMissingOrderViewModel> OrderList)
        {
            try
            {
                var data = JsonConvert.SerializeObject(OrderList);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ImportMissingOrder/");
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

                MissingOrderReturnViewModel status = JsonConvert.DeserializeObject<MissingOrderReturnViewModel>(strResponse);
                return status;
            }
            catch (Exception)
            {

                throw;
            }
          
        }
    }
}
