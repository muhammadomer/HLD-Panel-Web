using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ConditionController : Controller
    {
        private readonly IConfiguration _configuration;
        public ConditionController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpGet]
        public IActionResult MainView()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Index()
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition");
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
            List<ConditionViewModel> responses = JsonConvert.DeserializeObject<List<ConditionViewModel>>(strResponse);
            return PartialView("~/Views/Condition/_Index.cshtml",responses);
         
        }

        [HttpGet]
        public IActionResult GetList()
        {
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition");
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
                List<ConditionViewModel> responses = JsonConvert.DeserializeObject<List<ConditionViewModel>>(strResponse);
                return Json(responses);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult AddUpdateCondition(int id = 0)
        {
            ConditionViewModel conditionViewModel = AddOrUpdateCondition(id);

            return PartialView("~/Views/Condition/_AddUpdateCondition.cshtml",conditionViewModel);

        }
        [HttpPost]
        public IActionResult AddUpdateCondition(ConditionViewModel conditionView)
        {
           
                try
                {
                    if (conditionView.ConditionId == 0)
                    {
                        var data = JsonConvert.SerializeObject(conditionView);
                        string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                        string token = Request.Cookies["Token"];
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition/save");
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
                        List<ConditionViewModel> models = RenderPartialView();
                        //return PartialView("~/Views/Condition/_indeX.cshtml", models);
                        return Json(new { success = true, message = "save successfully" });
                    }
                    else
                    {
                        var data = JsonConvert.SerializeObject(conditionView);
                        string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                        string token = Request.Cookies["Token"];
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition/Update");
                        request.Method = "PUT";
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
                        List<ConditionViewModel> models = RenderPartialView();

                        return Json(new { success = true, message = "update successfully" });
                    }
                }
                catch (Exception ex)
                {
                }

            return View();
        }


        public List<ConditionViewModel> RenderPartialView()
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition");
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
            List<ConditionViewModel> responses = JsonConvert.DeserializeObject<List<ConditionViewModel>>(strResponse);
            return responses;
        }

        private ConditionViewModel AddOrUpdateCondition(int id = 0)
        {
            ConditionViewModel conditionViewModel = new ConditionViewModel();

            try
            {
                if (id > 0)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition/ById/" + id);
                    request.Method = "GET";
                    request.Accept = "application/json;";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + token;

                    var response = (HttpWebResponse)request.GetResponse();
                    string strResponse = "";
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                    conditionViewModel = JsonConvert.DeserializeObject<ConditionViewModel>(strResponse);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return conditionViewModel;
        }
 
        [HttpPost]
        public JsonResult CheckConditionExists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/condition/CheckExists/" + name + "");
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = Convert.ToBoolean(JObject.Parse(strResponse)["status"].ToString());

            }
            catch (Exception)
            {
                throw;
            }
            return Json(new { success = status });
        }
    }
}