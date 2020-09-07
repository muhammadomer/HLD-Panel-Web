using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class UploadFilesToS3ForJobsAPIAccess
    {
        public JobIdReturnViewModel SaveJobDetail(string ApiURL,string token, UploadFilesToS3ViewModel model)
        {
            JobIdReturnViewModel returnViewModel = new JobIdReturnViewModel();

             var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/UploadFilesToS3");
            request.Method = "POST";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            string strResponse = "";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            returnViewModel = JsonConvert.DeserializeObject<JobIdReturnViewModel>(strResponse);
            return returnViewModel;
        }

        public List<GetJobDetailViewModel> GetJobDetail(string ApiURL, string token)
        {
           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/UploadFilesToS3");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            string strResponse = "";

          
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<GetJobDetailViewModel> responses = JsonConvert.DeserializeObject<List<GetJobDetailViewModel>>(strResponse);

            return responses;
        }


        public S3LogViewModel GetJobLogs(string ApiURL, string token,int id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/UploadFilesToS3/"+id);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            string strResponse = "";


            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            S3LogViewModel responses = JsonConvert.DeserializeObject<S3LogViewModel>(strResponse);

            return responses;
        }
    }
}
