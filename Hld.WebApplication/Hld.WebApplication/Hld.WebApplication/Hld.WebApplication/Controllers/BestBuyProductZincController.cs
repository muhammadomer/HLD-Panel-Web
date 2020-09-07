using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class BestBuyProductZincController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";

        BBProductApiAccess _bBProductApiAccess = null;
        ProductApiAccess _productApiAccess = null;
        BestBuyProductZincApiAccess _productZinc = null;
        public BestBuyProductZincController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _bBProductApiAccess = new BBProductApiAccess();
            _productApiAccess = new ProductApiAccess();
            _productZinc = new BestBuyProductZincApiAccess();
        }
        public IActionResult Index(string name)
        {
            return PartialView("~/Views/Brand/_index.cshtml", _productZinc.GetAllBestBuyProductZincByName(ApiURL,token,name));
        }

        public IActionResult MainView()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddUpdateProductZinc(BestBuyProductZincViewModel viewModel )
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddUpdateProductZinc()
        {
            return View();
        }



    }
}