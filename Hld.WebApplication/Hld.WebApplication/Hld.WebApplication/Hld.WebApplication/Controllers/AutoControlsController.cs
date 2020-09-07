using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    public class AutoControlsController : Controller
    {
        private readonly IConfiguration _configuration;
        AutoControlsApiAccess controlApiAccess = new AutoControlsApiAccess();

        string ApiURL = null;

        public AutoControlsController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
        }

        // GET: AutoControls
        public ActionResult Index()
        {

            string token = Request.Cookies["Token"];
            List<AutoControlViewModel> listmodel = new List<AutoControlViewModel>();
            listmodel = controlApiAccess.GetControls(ApiURL, token);
            return View(listmodel);
            
        }

        // GET: AutoControls/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AutoControls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AutoControls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AutoControlViewModel collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AutoControls/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AutoControls/Edit/5
        [HttpPost]
     
        public bool Update(string statusID,string jobName)
        {
            bool status = false;
            try
            {
                AutoControlViewModel viewModel = new AutoControlViewModel();
                viewModel.StatusID = Convert.ToInt32(statusID);
                viewModel.JobName = jobName;

                string token = Request.Cookies["Token"];

                status = controlApiAccess.EnableDisableZincJobs(ApiURL, token, viewModel);

                return status;
            }
            catch
            {
                return false;
            }


        }

        // GET: AutoControls/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AutoControls/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}