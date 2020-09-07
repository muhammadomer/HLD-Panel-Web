using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class CatageoryController : Controller
    {

        private readonly IConfiguration _configuration;
        public CatageoryController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MainView()
        {
            CatageoriesViewModel catageoriesViewModel = GetAll_Main_Sub_Catageories();
            return View(catageoriesViewModel);
        }

        [HttpGet]
        public JsonResult CatageorySub1(int id)
        {
            return Json(GetCatageorySub1(id));
        }

        [HttpGet]
        public JsonResult CatageorySub2(int id)
        {
            return Json(GetCatageorySub2(id));
        }

        [HttpGet]
        public JsonResult CatageorySub3(int id)
        {
            return Json(GetCatageorySub3(id));
        }

        [HttpGet]
        public JsonResult CatageorySub4(int id)
        {
            return Json(GetCatageorySub4(id));
        }

        #region add update catageory main

        [HttpGet]
        public ActionResult AddUpdateCatagoeryMain(int id = 0)
        {
            CatageoryMainViewModel ViewModel = new CatageoryMainViewModel();
            try
            {
                if (id > 0)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageoryMain/GetById/" + id);
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
                    ViewModel = JsonConvert.DeserializeObject<CatageoryMainViewModel>(strResponse);
                    return PartialView("~/Views/Catageory/_AddUpdateCatagoeryMain.cshtml", ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Catageory/_AddUpdateCatagoeryMain.cshtml", ViewModel);
        }


        [HttpPost]
        public ActionResult AddUpdateCatageoryMain(CatageoryMainViewModel ViewModel)
        {
            if (ViewModel.CatageoryMainId == 0)
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageoryMain/save");
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


                List<CatageoryMainViewModel> responses = GetMainCatageory();
                List<SelectListItem> _listselectListItems = new List<SelectListItem>();
                if (responses.Count > 0)
                {

                    foreach (var item in responses)
                    {
                        SelectListItem selectListItem = new SelectListItem();

                        selectListItem.Value = item.CatageoryMainId.ToString();
                        selectListItem.Text = item.CatageoryMainName;
                        _listselectListItems.Add(selectListItem);
                    }
                }

                return Json(new { success = true, message = "save successfully", viewModel = _listselectListItems });
            }
            else
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageoryMain/Update");
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

        #endregion

        #region add update Catageory sub 1 
        [HttpGet]
        public ActionResult AddUpdateCatagoerySub1(int id = 0)
        {
            CatageorySub1ViewModel ViewModel = new CatageorySub1ViewModel();
            try
            {
                if (id > 0)
                {
                    ViewModel = GetCatageorySub1ById(id);
                    return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub1.cshtml", ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub1.cshtml", ViewModel);
        }

        [HttpPost]
        public ActionResult AddUpdateCatagoerySub1(CatageorySub1ViewModel ViewModel)
        {
            if (ViewModel.CatageorySubId == 0)
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub1/save");
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

                return Json(new { success = true, message = "save successfully", viewModel = GetCatageorySub1(ViewModel.CatageoryMainId) });
            }
            else
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub1/Update");
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
                return Json(new { success = true, message = "Update successfully", viewModel = GetCatageorySub1(ViewModel.CatageoryMainId) });
            }
        }
        #endregion

        #region add update Catageory sub 2 
        [HttpGet]
        public ActionResult AddUpdateCatagoerySub2(int id = 0)
        {
            CatageorySub2ViewModel ViewModel = new CatageorySub2ViewModel();
            try
            {
                if (id > 0)
                {
                    ViewModel = GetCatageorySub2ById(id);
                    return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub2.cshtml", ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub2.cshtml", ViewModel);
        }

        [HttpPost]
        public ActionResult AddUpdateCatagoerySub2(CatageorySub2ViewModel ViewModel)
        {
            if (ViewModel.CatageorySub2Id == 0)
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub2/save");
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
                return Json(new { success = true, message = "save successfully", viewModel = GetCatageorySub2(ViewModel.CatageorySub1Id) });
            }
            else
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub2/Update");
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
                return Json(new { success = true, message = "Update successfully", viewModel = GetCatageorySub2(ViewModel.CatageorySub1Id) });
            }
        }
        #endregion

        #region add update Catageory sub 3 
        [HttpGet]
        public ActionResult AddUpdateCatagoerySub3(int id = 0)
        {
            CatageorySub3ViewModel ViewModel = new CatageorySub3ViewModel();
            try
            {
                if (id > 0)
                {
                    ViewModel = GetCatageorySub3ById(id);
                    return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub3.cshtml", ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub3.cshtml", ViewModel);
        }

        [HttpPost]
        public ActionResult AddUpdateCatagoerySub3(CatageorySub3ViewModel ViewModel)
        {
            if (ViewModel.CatageorySub3Id == 0)
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub3/save");
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
                return Json(new { success = true, message = "save successfully", viewModel = GetCatageorySub3(ViewModel.CatageorySub2Id) });
            }
            else
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub3/Update");
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
                return Json(new { success = true, message = "Update successfully", viewModel = GetCatageorySub3(ViewModel.CatageorySub2Id) });
            }
        }
        #endregion

        #region add update Catageory sub 4 
        [HttpGet]
        public ActionResult AddUpdateCatagoerySub4(int id = 0)
        {
            CatageorySub4ViewModel ViewModel = new CatageorySub4ViewModel();
            try
            {
                if (id > 0)
                {
                    ViewModel = GetCatageorySub4ById(id);
                    return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub4.cshtml", ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("~/Views/Catageory/_AddUpdateCatagoerySub4.cshtml", ViewModel);
        }

        [HttpPost]
        public ActionResult AddUpdateCatagoerySub4(CatageorySub4ViewModel ViewModel)
        {
            if (ViewModel.CatageorySub4Id == 0)
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub4/save");
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
                return Json(new { success = true, message = "save successfully", viewModel = GetCatageorySub4(ViewModel.CatageorySub3Id) });
            }
            else
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub4/Update");
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
                return Json(new { success = true, message = "Update successfully", viewModel = GetCatageorySub4(ViewModel.CatageorySub3Id) });
            }
        }
        #endregion

        #region Populate All initial Data

        public CatageoriesViewModel GetAll_Main_Sub_Catageories()
        {
            CatageoriesViewModel catageoriesViewModel = new CatageoriesViewModel();

            List<CatageoryMainViewModel> mainModel = GetMainCatageory();

            List<SelectListItem> _listselectListItems = new List<SelectListItem>();
            _listselectListItems.Insert(0, new SelectListItem() { Value = "0", Text = "Select Category" });
            if (mainModel.Count > 0)
            {

                foreach (var item in mainModel)
                {
                    SelectListItem selectListItem = new SelectListItem();

                    selectListItem.Value = item.CatageoryMainId.ToString();
                    selectListItem.Text = item.CatageoryMainName;
                    _listselectListItems.Add(selectListItem);
                }
            }


            catageoriesViewModel.CatageoryMain = _listselectListItems;

            List<SelectListItem> CatageoruSub1 = new List<SelectListItem>()
            {  new SelectListItem(){
                    Value="0",
                    Text="Select Category"
                }
            };

            catageoriesViewModel.CatageorySub1 = CatageoruSub1;

            List<SelectListItem> CatageoruSub2 = new List<SelectListItem>()
            {  new SelectListItem(){
                    Value="0",
                    Text="Select Category"
                }
            };
            catageoriesViewModel.CatageorySub2 = CatageoruSub2;

            List<SelectListItem> CatageoruSub3 = new List<SelectListItem>()
            {  new SelectListItem(){
                    Value="0",
                    Text="Select Category"
                }
            };

            catageoriesViewModel.CatageorySub3 = CatageoruSub3;

            List<SelectListItem> CatageoruSub4 = new List<SelectListItem>()
            {
                new SelectListItem(){
                    Value="0",
                    Text="Select Category"
                }
            };

            catageoriesViewModel.CatageorySub4 = CatageoruSub4;
            return catageoriesViewModel;
        }

        #endregion

        #region Get Main Catageory
        public List<CatageoryMainViewModel> GetMainCatageory()
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageoryMain");
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
            List<CatageoryMainViewModel> responses = JsonConvert.DeserializeObject<List<CatageoryMainViewModel>>(strResponse);


            return responses;
        }
        #endregion

        private List<CatageorySub1ViewModel> GetListOfSub1(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub1/" + id + "");
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
            List<CatageorySub1ViewModel> responses = JsonConvert.DeserializeObject<List<CatageorySub1ViewModel>>(strResponse);
            return responses;
        }
        #region Get Sub Catageory 1
        public List<SelectListItem> GetCatageorySub1(int id)
        {

            List<CatageorySub1ViewModel> responses = GetListOfSub1(id);
            List<SelectListItem> _listselectListItems = new List<SelectListItem>();
            if (responses.Count > 0)
            {

                foreach (var item in responses)
                {
                    SelectListItem selectListItem = new SelectListItem();

                    selectListItem.Value = item.CatageorySubId.ToString();
                    selectListItem.Text = item.CatageorySub1Name;
                    _listselectListItems.Add(selectListItem);
                }
            }

            return _listselectListItems;
        }
        #endregion

        #region Get Sub Catageory 1 By id
        public CatageorySub1ViewModel GetCatageorySub1ById(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub1/ById/" + id + "");
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
            CatageorySub1ViewModel responses = JsonConvert.DeserializeObject<CatageorySub1ViewModel>(strResponse);


            return responses;
        }
        #endregion
        #region Get Sub Catageory 2 By id
        public CatageorySub2ViewModel GetCatageorySub2ById(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub2/ById/" + id + "");
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
            CatageorySub2ViewModel responses = JsonConvert.DeserializeObject<CatageorySub2ViewModel>(strResponse);


            return responses;
        }
        #endregion

        #region Get Sub Catageory 3 By id
        public CatageorySub3ViewModel GetCatageorySub3ById(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub3/ById/" + id + "");
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
            CatageorySub3ViewModel responses = JsonConvert.DeserializeObject<CatageorySub3ViewModel>(strResponse);


            return responses;
        }
        #endregion


        #region Get Sub Catageory 4 By id
        public CatageorySub4ViewModel GetCatageorySub4ById(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub4/ById/" + id + "");
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
            CatageorySub4ViewModel responses = JsonConvert.DeserializeObject<CatageorySub4ViewModel>(strResponse);
            return responses;
        }
        #endregion


        private List<CatageorySub2ViewModel> GetListOfSub2(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub2/" + id + "");
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
            List<CatageorySub2ViewModel> responses = JsonConvert.DeserializeObject<List<CatageorySub2ViewModel>>(strResponse);
            return responses;
        }

        #region Get Sub Catageory 2
        public List<SelectListItem> GetCatageorySub2(int id)
        {
            List<CatageorySub2ViewModel> responses = GetListOfSub2(id);
            List<SelectListItem> _listselectListItems = new List<SelectListItem>();
            if (responses.Count > 0)
            {

                foreach (var item in responses)
                {
                    SelectListItem selectListItem = new SelectListItem();

                    selectListItem.Value = item.CatageorySub2Id.ToString();
                    selectListItem.Text = item.CatageorySub2Name;
                    _listselectListItems.Add(selectListItem);
                }
            }

            return _listselectListItems;
        }
        #endregion



        private List<CatageorySub3ViewModel> GetListOfSub3(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub3/" + id + "");
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
            List<CatageorySub3ViewModel> responses = JsonConvert.DeserializeObject<List<CatageorySub3ViewModel>>(strResponse);
            return responses;
        }

        #region Get Sub Catageory 3
        public List<SelectListItem> GetCatageorySub3(int id)
        {
            List<CatageorySub3ViewModel> responses = GetListOfSub3(id);
            List<SelectListItem> _listselectListItems = new List<SelectListItem>();
            if (responses.Count > 0)
            {

                foreach (var item in responses)
                {
                    SelectListItem selectListItem = new SelectListItem();

                    selectListItem.Value = item.CatageorySub3Id.ToString();
                    selectListItem.Text = item.CatageorySub3Name;
                    _listselectListItems.Add(selectListItem);
                }
            }

            return _listselectListItems;
        }
        #endregion


        private List<CatageorySub4ViewModel> GetListOfSub4(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub4/" + id + "");
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
            List<CatageorySub4ViewModel> responses = JsonConvert.DeserializeObject<List<CatageorySub4ViewModel>>(strResponse);
            return responses;
        }

        #region Get Sub Catageory 4
        public List<SelectListItem> GetCatageorySub4(int id)
        {
            List<CatageorySub4ViewModel> responses = GetListOfSub4(id);
            List<SelectListItem> _listselectListItems = new List<SelectListItem>();
            if (responses.Count > 0)
            {

                foreach (var item in responses)
                {
                    SelectListItem selectListItem = new SelectListItem();

                    selectListItem.Value = item.CatageorySub4Id.ToString();
                    selectListItem.Text = item.CatageorySub4Name;
                    _listselectListItems.Add(selectListItem);
                }
            }

            return _listselectListItems;
        }
        #endregion


        [HttpGet]
        public ActionResult CategoriesListMainView()
        {
            var mainCategory = GetMainCatageory();
            foreach (var item in mainCategory)
            {
                var list = GetListOfSub1(item.CatageoryMainId);
                if (list.Count > 0)
                {
                    item.status = "exists";
                }
                else
                {
                    item.status = "not-exists";
                }
            }

            return View(mainCategory);
        }

        private List<CatageorySub1ViewModel> GetCategorySub1ListFromDatabase(int id)
        {
            var CategorySub1 = GetListOfSub1(id);

            foreach (var item in CategorySub1)
            {
                var list = GetListOfSub2(item.CatageorySubId);
                if (list.Count > 0)
                {
                    item.status = "exists";
                }
                else
                {
                    item.status = "not-exists";
                }
            }
            return CategorySub1;
        }

        [HttpGet]
        public ActionResult CategoriesListSub1(int id)
        {
            return PartialView("~/Views/Catageory/_CategoriesListSub1.cshtml", GetCategorySub1ListFromDatabase(id));
        }

        private List<CatageorySub2ViewModel> GetCategorySub2ListFromDatabase(int id)
        {
            var CategorySub1 = GetListOfSub2(id);

            foreach (var item in CategorySub1)
            {
                var list = GetListOfSub3(item.CatageorySub2Id);
                if (list.Count > 0)
                {
                    item.status = "exists";
                }
                else
                {
                    item.status = "not-exists";
                }
            }
            return CategorySub1;
        }

        [HttpGet]
        public ActionResult CategoriesListSub2(int id)
        {
            return PartialView("~/Views/Catageory/_CategoriesListSub2.cshtml", GetCategorySub2ListFromDatabase(id));
        }

        private List<CatageorySub3ViewModel> GetCategorySub3ListFromDatabase(int id)
        {
            var CategorySub1 = GetListOfSub3(id);

            foreach (var item in CategorySub1)
            {
                var list = GetListOfSub4(item.CatageorySub3Id);
                if (list.Count > 0)
                {
                    item.status = "exists";
                }
                else
                {
                    item.status = "not-exists";
                }
            }
            return CategorySub1;
        }



        [HttpGet]
        public ActionResult CategoriesListSub3(int id)
        {
            return PartialView("~/Views/Catageory/_CategoriesListSub3.cshtml", GetCategorySub3ListFromDatabase(id));
        }
        private List<CatageorySub4ViewModel> GetCategorySub4ListFromDatabase(int id)
        {
            var CategorySub1 = GetListOfSub4(id);

            foreach (var item in CategorySub1)
            {
                item.status = "exists";
            }
            return CategorySub1;
        }


        [HttpGet]
        public ActionResult CategoriesListSub4(int id)
        {
            return PartialView("~/Views/Catageory/_CategoriesListSub4.cshtml", GetCategorySub4ListFromDatabase(id));
        }
        [HttpGet]
        public ActionResult DeleteCategoryMain()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DeleteCategorySub1(int id)
        {
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            string token = Request.Cookies["Token"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub1/Delete/" + id + "");
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
            StatusMessage responses = JsonConvert.DeserializeObject<StatusMessage>(strResponse);
            if (responses.status)
            {

            }
            return Json(GetCategorySub1ListFromDatabase(id));
        }
        [HttpGet]
        public ActionResult DeleteCategorySub2()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DeleteCategorySub3()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DeleteCategorySub4()
        {
            return View();
        }




        [HttpPost]
        public JsonResult CatageoryMainExists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageoryMain/CheckExists/" + name + "");
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
        public JsonResult CatageorySub1Exists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub1/CheckExists/" + name + "");
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
        public JsonResult CatageorySub2Exists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub2/CheckExists/" + name + "");
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
        public JsonResult CatageorySub3Exists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub3/CheckExists/" + name + "");
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
        public JsonResult CatageorySub4Exists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/CatageorySub4/CheckExists/" + name + "");
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


    public class StatusMessage
    {
        public bool status { get; set; }
        public string Message { get; set; }
    }
}