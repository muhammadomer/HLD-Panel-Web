using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ColorController : Controller
    {
        private readonly IConfiguration _configuration;
        public ColorController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult MainView()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
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
                return PartialView("~/Views/Color/_index.cshtml", responses);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult AddUpdateColor(int id = 0)
        {
            ColorViewModel colorViewModel = new ColorViewModel();
            try
            {
                if (id > 0)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Color/GetById/" + id);
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
                    colorViewModel = JsonConvert.DeserializeObject<ColorViewModel>(strResponse);
                    return PartialView("~/Views/Color/_AddUpdateColor.cshtml", colorViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Color/_AddUpdateColor.cshtml", colorViewModel);
        }
        [HttpPost]
        public ActionResult AddUpdateColor(ColorViewModel colorViewModel)
        {
            colorViewModel.ColorAlias = colorViewModel.ColorAlias.ToUpper();
            if (colorViewModel.ColorId == 0)
            {
                var data = JsonConvert.SerializeObject(colorViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Color/save");
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
                return Json(new { success = true, message = "save successfully" });
            }
            else
            {
                var data = JsonConvert.SerializeObject(colorViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Color/Update");
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

                var Status = strResponse;
                return Json(new { success = true, message = "Update successfully" });
            }
        }


        [HttpGet]
        public IActionResult DeleteColor(int id)
        {
            BrandViewModel brandViewModel = new BrandViewModel();

            try
            {
                if (id > 0)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand/GetById/" + id);
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
                    brandViewModel = JsonConvert.DeserializeObject<BrandViewModel>(strResponse);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(brandViewModel);
        }


        [HttpPost]
        public JsonResult CheckColorExists(string name)
        {
            bool status = false;
            try
            {

                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Color/CheckExists/" + name+"");
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

        [HttpPost]
        public JsonResult CheckColorAliasExists(string name)
        {
            bool status = false;
            try
            {

                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Color/CheckColorAliasExists/" + name + "");
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