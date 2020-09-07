using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ZincAccountsController : Controller
    {
        private readonly IConfiguration _configuration;
        ZincAcountApiDataAccess _zincaccountApiAccess = new ZincAcountApiDataAccess();
        EncDecChannel encDecChannel = new EncDecChannel();
        string ApiURL = null;

        public ZincAccountsController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(ZincAccountsViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    viewmodel.UserNameShort = viewmodel.UserName;
                    viewmodel.PasswordShort = viewmodel.Password.Trim().Substring(viewmodel.Password.Length - 4);
                    viewmodel.KeyShort = viewmodel.Key.Trim().Substring(viewmodel.Key.Length - 2);

                    viewmodel.Password = encDecChannel.EncryptStringToBytes_Aes(viewmodel.Password.Trim());
                    viewmodel.Key = encDecChannel.EncryptStringToBytes_Aes(viewmodel.Key.Trim());
                    viewmodel.UserName = encDecChannel.EncryptStringToBytes_Aes(viewmodel.UserName.Trim());


                    string token = Request.Cookies["Token"];

                    _zincaccountApiAccess.SaveZincAccount(ApiURL, token, viewmodel);

                    return RedirectToAction("ZincAccountDetail", "ZincAccounts");
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
                return View(viewmodel);

        }

        public IActionResult ZincAccountDetail()
        {
            string token = Request.Cookies["Token"];
            List<ZincAccountsViewModel> listmodel = new List<ZincAccountsViewModel>();
            listmodel = _zincaccountApiAccess.ZincAccountList(ApiURL, token);
            return View(listmodel);
        }

        public int IsActive(int id, bool IsActive)
        {
            string token = Request.Cookies["Token"];
            ZincAccountsViewModel Viewmodel = new ZincAccountsViewModel();
            Viewmodel.IsActive = IsActive;
            Viewmodel.ZincAccountsId = id;
            _zincaccountApiAccess.IsActiveZinc(ApiURL, token, Viewmodel);
            return 0;
        }

        public int IsDefault(int id, bool IsDefault)
        {
            string token = Request.Cookies["Token"];
            ZincAccountsViewModel Viewmodel = new ZincAccountsViewModel();
            Viewmodel.IsDefault = IsDefault;
            Viewmodel.ZincAccountsId = id;
            _zincaccountApiAccess.IsDefaultZinc(ApiURL, token, Viewmodel);
            return 0;
        }

    }
}