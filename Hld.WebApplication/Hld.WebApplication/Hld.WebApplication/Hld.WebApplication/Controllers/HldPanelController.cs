using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class HldPanelController : Controller
    {
        private readonly IConfiguration _configuration;
        public HldPanelController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetOrderList()
        {
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/HldPanel");
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
                List<HldPanelViewModel> responses = JsonConvert.DeserializeObject<List<HldPanelViewModel>>(strResponse);
                return Json( new { data = responses });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult GetOrderList(HldPanelViewModel hldPanelViewModelData)
        {
            return View();
        }
    }
}