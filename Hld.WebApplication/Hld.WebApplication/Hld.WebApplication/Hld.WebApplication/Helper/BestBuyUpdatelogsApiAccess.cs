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
    public class BestBuyUpdatelogsApiAccess
    {
        public int GetCounter(string ApiURL, string token, int JobId)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyUpdatelogs/GetlogsCount?JobId=" + JobId);
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
                        Count = Convert.ToInt32(strResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }
        public List<BestBuyUpdateLogsViewModel> GetShipmentList(string apiurl, string token, int JobId, int Limit, int OffSet)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/BestBuyUpdatelogs/Getlogs?JobId=" + JobId + "&limit=" + Limit + "&offSet=" + OffSet );
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
            //List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            List<BestBuyUpdateLogsViewModel> responses = JsonConvert.DeserializeObject<List<BestBuyUpdateLogsViewModel>>(strResponse);
            return responses;
        }

    }
}
