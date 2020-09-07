using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    public class ComponentsController : Controller
    {
        public IActionResult ZincASINDetail(string sku)
        {
            return ViewComponent("ZincASINDetail", sku);
        }
    }
}