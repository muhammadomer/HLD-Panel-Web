using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(string Id)
        {
            if (Id == "404")
            {
                return View("Error404");
            }
            if (Id == "500")
            {
                return View("Error500");
            }
            return View();
        }

    }
}