using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Hld.WebApplication.Helper
{
    public class EmployeeApiAccess
    {
        public bool SaveEmployee(string ApiURL, string token, EmployeeViewModel ViewModel)
        {
            bool status = false;
            try
            {

                EmployeeModel model = new EmployeeModel()
                { 
                    EmployeeName=ViewModel.EmployeeName,
                    Id=ViewModel.Id,
                    EmployeeId=ViewModel.EmployeeId,
                    Active=ViewModel.Active,
                    CreatedOn=ViewModel.CreatedOn,
                    EmployeeRole=ViewModel.EmployeeRole
                };
                var data = JsonConvert.SerializeObject(model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Employee/save/");
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

        public List<EmployeeViewModel> EmployeeList(string ApiURL, string token)
        {
            List<EmployeeViewModel> listmodel = new List<EmployeeViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Employee/GetEmployees/");
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
                listmodel = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }

        public EmployeeViewModel EditEmployeeByid(string ApiURL, string token, int id)
        {
            EmployeeViewModel ViewModel = new EmployeeViewModel();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Employee/GetEmployeeById/" + id);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                //request.Headers["Authorization"] = "Bearer " + token;
                //request.Headers["Authorization"] = "Bearer ";
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<EmployeeViewModel>(strResponse);

            }
            catch (Exception ex)
            {

                throw;
            }
            return ViewModel;
        }

        public bool UpdateEmployeeById(string ApiURL, string token, EmployeeViewModel ViewModel)
        {
            bool status = false;
            try
            {
                EmployeeModel model = new EmployeeModel()
                {
                    EmployeeName = ViewModel.EmployeeName,
                    Id = ViewModel.Id,
                    EmployeeId = ViewModel.EmployeeId,
                    Active = ViewModel.Active,
                    CreatedOn = ViewModel.CreatedOn,
                    EmployeeRole = ViewModel.EmployeeRole
                };
                var data = JsonConvert.SerializeObject(model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Employee/UpdateEmployee/");
                //request.Method = "POST";
                request.Method = "PUT";
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

        public bool UpdateEmpActiveStatusById(string ApiURL, string token, EmployeeViewModel ViewModel)
        {
            bool status = false;
            try
            {
                EmployeeModel model = new EmployeeModel()
                {
                    EmployeeName = ViewModel.EmployeeName,
                    Id = ViewModel.Id,
                    EmployeeId = ViewModel.EmployeeId,
                    Active = ViewModel.Active,
                    CreatedOn = ViewModel.CreatedOn,
                    EmployeeRole = ViewModel.EmployeeRole
                };
                var data = JsonConvert.SerializeObject(model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Employee/UpdateEmpActiveStatusById/");
                //request.Method = "POST";
                request.Method = "PUT";
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

    }
}
