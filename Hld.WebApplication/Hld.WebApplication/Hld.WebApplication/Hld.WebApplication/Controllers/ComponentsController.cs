using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ComponentsController : Controller
    {
        public IActionResult ZincASINDetail(string sku)
        {
            return ViewComponent("ZincASINDetail", sku);
        }
        public IActionResult ZincASINDetailLatest(string sku)
        {

            return ViewComponent("ZincASINDetailLatest", sku);
        }
    }
}