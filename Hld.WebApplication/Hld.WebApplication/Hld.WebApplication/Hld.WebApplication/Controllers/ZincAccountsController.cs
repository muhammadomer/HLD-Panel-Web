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

        public IActionResult Index(ZincAccountsViewModel editmodel)
        {
            
            ZincAccountsViewModel ViewModel = new ZincAccountsViewModel();
            if (editmodel != null)
            {
                ViewModel = editmodel;
            }
            return View(ViewModel);
        }
        [HttpPost]
        public IActionResult IndexSave(ZincAccountsViewModel viewmodel)
        {
            
            if (ModelState.IsValid)
            {

                try
                {
                    
                  
                    viewmodel.UserNameShort = viewmodel.UserName;
                    viewmodel.AmzAccountNameShort = viewmodel.AmzAccountName;
                    viewmodel.PasswordShort = viewmodel.Password.Trim().Substring(viewmodel.Password.Length - 4);
                    viewmodel.KeyShort = viewmodel.Key.Trim().Substring(viewmodel.Key.Length - 2);

                    viewmodel.Password = encDecChannel.EncryptStringToBytes_Aes(viewmodel.Password.Trim());
                    viewmodel.Key = encDecChannel.EncryptStringToBytes_Aes(viewmodel.Key.Trim());
                    viewmodel.UserName = encDecChannel.EncryptStringToBytes_Aes(viewmodel.UserName.Trim());
                    viewmodel.AmzAccountName = encDecChannel.EncryptStringToBytes_Aes(viewmodel.AmzAccountName.Trim());


                    string token = Request.Cookies["Token"];

                    if (viewmodel.ZincAccountsId > 0) {
                        _zincaccountApiAccess.SaveZincAccount(ApiURL, token, viewmodel);
                    }
                   
                    else
                    {
                        _zincaccountApiAccess.SaveZincAccount(ApiURL, token, viewmodel);
                    }
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

        [HttpGet]
        public IActionResult ZincAccountDetailEdit(int id)
        {
            string token = Request.Cookies["Token"];
            ZincAccountsViewModel ViewModel = new ZincAccountsViewModel();
            ViewModel = _zincaccountApiAccess.ZincAccountDetailEdit(ApiURL, token, id);
          
            ViewModel.Password = encDecChannel.DecryptStringFromBytes_Aes(ViewModel.Password.Trim());
            ViewModel.Key = encDecChannel.DecryptStringFromBytes_Aes(ViewModel.Key.Trim());
            ViewModel.UserName = encDecChannel.DecryptStringFromBytes_Aes(ViewModel.UserName.Trim());
            ViewModel.AmzAccountName = encDecChannel.DecryptStringFromBytes_Aes(ViewModel.AmzAccountName.Trim());
            return RedirectToAction("Index", "ZincAccounts", ViewModel);
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