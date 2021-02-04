using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class LogOutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Logout()
        {
            if (Request.Cookies["Token"] != null)
            {
                Response.Cookies.Delete("Token");
                return RedirectToAction("Authenticate", "Authentication");
            }
            return RedirectToAction("Authenticate", "Authentication");

        }
    }
}