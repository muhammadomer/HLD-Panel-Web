using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hld.WebApplication.Helper;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class DropshipEnableDisableLogController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        SellerCloudController ctrl = null;
        public static IHostingEnvironment _environment;
        DropshipEnableDisableLogApiAccess dropshipEnableDisableApiAcccess = null;
         
        

        public DropshipEnableDisableLogController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _environment = environment;
            dropshipEnableDisableApiAcccess = new DropshipEnableDisableLogApiAccess();
            
            
        }

        public IActionResult ShowEnableDisableLog(string ProductSku)
        {
            token = Request.Cookies["Token"];
            List<DropShipEnableDisableLogViewModel> model= dropshipEnableDisableApiAcccess.GetDropshipEnableDisableLog_SKU(ApiURL, token, ProductSku);
            return View(model);
        }
         
    }
}