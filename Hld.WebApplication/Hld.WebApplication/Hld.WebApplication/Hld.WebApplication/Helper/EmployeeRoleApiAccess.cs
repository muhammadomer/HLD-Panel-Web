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
    public class EmployeeRoleApiAccess
    {
        public bool SaveEmployeeRole(string ApiURL, string token, EmployeeRoleViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/EmployeeRole/save/");
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
            catch (Exception ex)
            {

                throw;
            }
            return status;
        }

        public List<EmployeeRoleViewModel> EmployeeRolesList(string ApiURL, string token)
        {
            List<EmployeeRoleViewModel> listmodel = new List<EmployeeRoleViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/EmployeeRole/GetEmployeeRoles/");
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                listmodel = JsonConvert.DeserializeObject<List<EmployeeRoleViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }
    }
}
