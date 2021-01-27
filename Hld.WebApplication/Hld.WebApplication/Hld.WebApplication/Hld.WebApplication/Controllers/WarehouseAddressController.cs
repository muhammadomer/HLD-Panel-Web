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
    public class WarehouseAddressController : Controller
    {
        private readonly IConfiguration _configuration;
        WarehouseApiAccess _warehouseApiAccess = new WarehouseApiAccess();

        string ApiURL = null;

        public WarehouseAddressController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
        }
        public ActionResult WareHouseAddressList()
        {

            string token = Request.Cookies["Token"];
            List<WarehouseAddressViewModel> listmodel = new List<WarehouseAddressViewModel>();
            listmodel = _warehouseApiAccess.WareHouseAddressList(ApiURL, token);
            return View(listmodel);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WarehouseAddressViewModel model)
        {
           
            if (ModelState.IsValid)
            {
               

                try
                {

                    string token = Request.Cookies["Token"];
                    if (model.ID>0)
                    {

                        _warehouseApiAccess.UpdateWarehouseAddress(ApiURL, token, model);
                       
                    }
                    else
                    {
                        _warehouseApiAccess.SaveWarehouseAddress(ApiURL, token, model);
                    }
                    return RedirectToAction("WareHouseAddressList", "WarehouseAddress");
                }
                catch
                {
                    return View();
                }
            }
            else
                return View(model);
        }
        public ActionResult Edit(int id)
        {

            string token = Request.Cookies["Token"];
            WarehouseAddressViewModel ViewModel = new WarehouseAddressViewModel();
            ViewModel = _warehouseApiAccess.EidtWHAddressByid(ApiURL, token, id);
            return View("Create", ViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
               

                return RedirectToAction(nameof(WareHouseAddressList));
            }
            catch
            {
                return View();
            }
        }

    }
}