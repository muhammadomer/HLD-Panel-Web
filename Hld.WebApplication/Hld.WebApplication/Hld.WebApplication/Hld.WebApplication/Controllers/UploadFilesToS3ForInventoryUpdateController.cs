using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class UploadFilesToS3ForInventoryUpdateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}