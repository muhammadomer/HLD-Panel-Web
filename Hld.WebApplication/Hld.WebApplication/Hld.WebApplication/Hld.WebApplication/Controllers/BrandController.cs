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
    public class BrandController : Controller
    {
        private readonly IConfiguration _configuration;
        public BrandController(IConfiguration configuration)
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand");
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
                return PartialView("~/Views/Brand/_index.cshtml", responses);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult AddUpdateBrand(int id = 0)
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
                    return PartialView("~/Views/Brand/_AddUpdateBrand.cshtml", brandViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Brand/_AddUpdateBrand.cshtml", brandViewModel);
        }
        [HttpPost]
        public ActionResult AddUpdateBrand(BrandViewModel brandViewModel)
        {
            if (brandViewModel.BrandId == 0)
            {
                var data = JsonConvert.SerializeObject(brandViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand/save");
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
                var data = JsonConvert.SerializeObject(brandViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand/Update");
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
        public IActionResult DeleteBrand(int id)
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
        public IActionResult DeleteBrand(BrandViewModel brandViewModel)
        {
            string status = "";
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand/Delete/" + brandViewModel.BrandId);
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
                status = JsonConvert.DeserializeObject<string>(strResponse);
                var Status = strResponse;
                ViewBag.Message = Status;
            }
            catch (Exception ex)
            {
            }
            return Json("");
        }


        [HttpPost]
        public JsonResult CheckBrandExists(string name)
        {
            bool status = false;
            try
            {

                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand/CheckExists/" + name + "");
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