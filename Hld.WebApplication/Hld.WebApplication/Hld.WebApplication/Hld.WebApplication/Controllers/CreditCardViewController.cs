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
    public class CreditCardViewController : Controller
    {
        private readonly IConfiguration _configuration;
        CreditCardApiAccess _creditcardApiAccess = new CreditCardApiAccess();
        EncDecChannel encDecChannel = new EncDecChannel();
        string ApiURL = null;

        public CreditCardViewController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(CreditCardDetailViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    viewmodel.name_on_cardShort = viewmodel.name_on_card;                    viewmodel.numberShort = viewmodel.number.Trim().Substring(viewmodel.number.Length - 4);                    viewmodel.security_codeShort = viewmodel.security_code.Trim().Substring(viewmodel.security_code.Length - 2);                    viewmodel.name_on_card = encDecChannel.EncryptStringToBytes_Aes(viewmodel.name_on_card.Trim());                    viewmodel.number = encDecChannel.EncryptStringToBytes_Aes(viewmodel.number.Trim());                    viewmodel.security_code = encDecChannel.EncryptStringToBytes_Aes(viewmodel.security_code.Trim());

                    string token = Request.Cookies["Token"];

                    _creditcardApiAccess.SaveCreditCardAddress(ApiURL, token, viewmodel);

                    return RedirectToAction("Listinfo", "CreditCardView");
                }
                catch
                {
                    return View();
                }
            }
            else
                return View(viewmodel);

        }

        public IActionResult Listinfo()
        {
            string token = Request.Cookies["Token"];
            List<CreditCardDetailViewModel> listmodel = new List<CreditCardDetailViewModel>();
            listmodel = _creditcardApiAccess.CreditCardAddressList(ApiURL, token);
            return View(listmodel);
        }
        public int IsActive(int id, bool IsActive)        {            string token = Request.Cookies["Token"];            CreditCardDetailViewModel Viewmodel = new CreditCardDetailViewModel();            Viewmodel.IsActive = IsActive;            Viewmodel.CreditCardDetailId = id;            _creditcardApiAccess.IsActive(ApiURL, token, Viewmodel);            return 0;        }

        public int IsDefault(int id, bool IsDefault)        {            string token = Request.Cookies["Token"];            CreditCardDetailViewModel Viewmodel = new CreditCardDetailViewModel();            Viewmodel.IsDefault = IsDefault;            Viewmodel.CreditCardDetailId = id;            _creditcardApiAccess.IsDefault(ApiURL, token, Viewmodel);            return 0;        }

    }
}