using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    [Authorize(Policy = "Access to Setting Tab")]
    public class CurrencyExchangeController : Controller
    {
        private readonly IConfiguration _configuration=null;
        string ApiURL = "";
        string token = "";
        CurrencyExchangeApiAccess _currencyApiAccess = null;
 
        public CurrencyExchangeController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _currencyApiAccess = new CurrencyExchangeApiAccess();
        }

        public IActionResult Index()
        {
            token = Request.Cookies["Token"];
            List<CurrencyExchangeViewModel> list = _currencyApiAccess.GetCurrencyExchangeList(ApiURL, token);
            return PartialView("~/Views/CurrencyExchange/Index.cshtml", list);
        }
        [HttpGet]
        public IActionResult AddUpdateCurrencyExchange(int id = 0)
        {
            token = Request.Cookies["Token"];
            CurrencyExchangeViewModel _currencyViewModel = new CurrencyExchangeViewModel(); ;

            if (id > 0)
            {
                _currencyViewModel = _currencyApiAccess.GetCurrencyExchangeByID(ApiURL, token, id);

                return PartialView("~/Views/CurrencyExchange/AddUpdateCurrencyExchange.cshtml", _currencyViewModel);
            }
            else
            {
                _currencyViewModel.dateTime = DateTime.Now;
                return PartialView("~/Views/CurrencyExchange/AddUpdateCurrencyExchange.cshtml", _currencyViewModel);
            }
        }

        [HttpPost]
        public IActionResult AddUpdateCurrencyExchange(CurrencyExchangeViewModel viewModel)
        {
            token = Request.Cookies["Token"];
            if (viewModel.CurrencyExchangeID > 0)
            {
                bool status = _currencyApiAccess.UpdateZincProductASIN(ApiURL, token, viewModel);
                return Json(new { success = true, message = "Update successfully" });
            }
            else
            {
                bool status = _currencyApiAccess.SaveCurrencyExchange(ApiURL, token, viewModel);
            }

            return Json(new { success = true, message = "Save successfully" });
        }

        public IActionResult MainView()
        {
            return View();
        }


    }
}