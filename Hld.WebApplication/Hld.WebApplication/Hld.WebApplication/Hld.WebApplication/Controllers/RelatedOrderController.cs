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

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class RelatedOrderController : Controller
    {
        private IConfiguration _configuration;
        string ApiURL = "";
        public IActionResult Index()
        {
            return View();
        }
        public RelatedOrderController(IConfiguration configuration)
        {
           
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
       
        }

        [HttpGet]
        public List<RelatedOrderModel> GetChildOrderByParent(int OrderId)
        {
            List<RelatedOrderModel> RelatedViewModel = new List<RelatedOrderModel>();
            try
            {
                if (OrderId > 0)
                {
                    
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/OrderRelation/" + OrderId);
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
                    RelatedViewModel = JsonConvert.DeserializeObject<List<RelatedOrderModel>>(strResponse);
                    return RelatedViewModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RelatedViewModel;
        }
    }
}