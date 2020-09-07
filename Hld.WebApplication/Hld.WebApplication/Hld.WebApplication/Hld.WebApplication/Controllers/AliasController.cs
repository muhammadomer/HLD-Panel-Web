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
using PagedList;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class AliasController : Controller
    {
        private readonly IConfiguration _configuration;
        public AliasController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public   IActionResult  MainView()
        {

           // GetDate();

        //    Dim orderIds As Integer() = sellerCloud.Orders_Get(filters)

        //If orderIds.Length > 0 Then
        //    ' Download all orders
        //    Dim orders As SC.OrderData() = sellerCloud.Orders_GetDatas(orderIds)

        //    ' Do something with the order objects
        //End If
            //sellerCloud.Timeout = 5 * 60 * 1000; // 5 minutes in milliseconds

            //sellerCloud.AuthHeaderValue = new SC.AuthHeader();
            //sellerCloud.AuthHeaderValue.UserName = "user@email.com";
            //sellerCloud.AuthHeaderValue.Password = "password";
            // var data=  S.Orders_GetDatasAsync(new AuthHeader() { Password = "U0tMIMrpeS*2qoIe9X%b", UserName = "xpress.shop77@gmail.com" }, new ServiceOptions(),new int[] { 6100767 });
            //            S.Orders_GetDatasAsync()
            //S.Orders_GetDataResponse
            //            S.Orders_GetDatasRe = S.Orders_GetDatasAsync(new AuthHeader() { Password = "U0tMIMrpeS*2qoIe9X%b", UserName = "xpress.shop77@gmail.com" }, new ServiceOptions(), new int[] { 6100718 });

            //            Orders_GetDatasRequest orders_GetDataRequest = new Orders_GetDatasRequest(
            //                new AuthHeader() { Password = "U0tMIMrpeS*2qoIe9X%b", UserName = "xpress.shop77@gmail.com" }, new ServiceOptions(), new int[] { 6100718 }
            //);
            return View();
        }

        [HttpGet]
        public IActionResult Index(int? page, int? pageSize)
        {
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


            //Ddefault size is 5 otherwise take pageSize value
            int defaSize = (pageSize ?? 5);

            ViewBag.psize = defaSize;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Alias");
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
                List<AliasViewModel> responses = JsonConvert.DeserializeObject<List<AliasViewModel>>(strResponse);
                responses.ToPagedList(pageIndex, defaSize);
                return PartialView("~/Views/Alias/_index.cshtml", responses);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult AddUpdateAlias(int id = 0)
        {
            AliasViewModel aliasViewModel = new AliasViewModel();
            try
            {
                if (id > 0)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Alias/GetById/" + id);
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
                    aliasViewModel = JsonConvert.DeserializeObject<AliasViewModel>(strResponse);
                    return PartialView("~/Views/Alias/_AddUpdateAlias.cshtml", aliasViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Alias/_AddUpdateAlias.cshtml", aliasViewModel);
        }
        [HttpPost]
        public ActionResult AddUpdateAlias(AliasViewModel aliasViewModel)
        {
            if (aliasViewModel.AliaseID == 0)
            {
                var data = JsonConvert.SerializeObject(aliasViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Alias/save");
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
                var data = JsonConvert.SerializeObject(aliasViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Alias/Update");
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
            AliasViewModel aliasViewModel = new AliasViewModel();
            try
            {
                if (id > 0)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Alias/GetById/" + id);
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
                    aliasViewModel = JsonConvert.DeserializeObject<AliasViewModel>(strResponse);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(aliasViewModel);
        }


        //public void  GetDate()
        //{
        //    ServiceReference1.SCServiceSoapClient S = new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);            
        //    ServiceReference1.AuthHeader SCAuth = new ServiceReference1.AuthHeader();
        //    SCAuth.UserName = "xpress.shop77@gmail.com";
        //    SCAuth.Password = "U0tMIMrpeS*2qoIe9X%b";
        //    SCAuth.ValidateDeviceID = false;

        //    ServiceReference2.SCServiceSoapClient news = new ServiceReference2.SCServiceSoapClient(ServiceReference2.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap);
        //    ServiceReference1.ServiceOptions SCSettings = new ServiceReference1.ServiceOptions();
        //    string[] keys = new string[10];
        //    string[] values = new string[10];
        //    SerializableDictionaryOfStringString filters = new SerializableDictionaryOfStringString();
        //    filters.Keys = keys;
        //    filters.Values = values;            
        //    var datas =  S.Orders_GetDatasAsync(SCAuth, null, new int[] { 6149475 });
            
            
        //   // var response =  S.Orders_GetDatasAsync(SCAuth, SCSettings, new int[] { 6149475 });

        //}


        [HttpPost]
        public IActionResult DeleteBrand(AliasViewModel aliasViewModel)
        {
            string status = "";
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Alias/Delete/" + aliasViewModel.AliaseID);
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

            }
            catch (Exception ex)
            {
            }
            return Json("");
        }
    }
}