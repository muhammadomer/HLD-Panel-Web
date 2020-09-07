using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    public class BootstrapModelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowModel()
        {
            return View();
        }
    }
}