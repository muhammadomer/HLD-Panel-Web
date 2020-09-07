using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Hld.WebApplication.Controllers
{
    public class TestController : Controller
    {
        public static IHostingEnvironment _environment;

        public TestController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
           
        }

            public IActionResult Index()
        {
            string rootPath=_environment.WebRootPath;
            return View();
        }
    }
}