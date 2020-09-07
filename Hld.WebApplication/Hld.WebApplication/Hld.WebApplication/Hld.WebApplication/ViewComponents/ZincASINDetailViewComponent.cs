using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewComponents
{
    public class ZincASINDetailViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        ZincApiAccess _zincApiAccess = null;
        string ApiULR = "";
        string token = "";
        public ZincASINDetailViewComponent(IConfiguration configuration)
        {
            this._configuration = configuration;
            _zincApiAccess = new ZincApiAccess();
            this._configuration = configuration;
            ApiULR = _configuration.GetValue<string>("WebApiURL:URL");
        }



        public IViewComponentResult Invoke(string SKU)
        {
            token = Request.Cookies["Token"];
            var articles = _zincApiAccess.GetProductZincDetailBySKU(ApiULR, token, SKU);
            return View(articles);
        }
    }
}
