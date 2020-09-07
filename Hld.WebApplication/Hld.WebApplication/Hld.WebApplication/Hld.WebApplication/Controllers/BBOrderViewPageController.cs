using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class BBOrderViewPageController : Controller
    {

        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";

        CreditCardApiAccess creditCardApiAccess = new CreditCardApiAccess();
        BBOrderViewPageApiAccess _bBorderApiAccess = null;
        ZincApiAccess _zincApiAccess = null;

        string s3BucketURL = "";
        string s3BucketURL_large = "";
        public BBOrderViewPageController(IConfiguration configuration)
        {
            _configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            _zincApiAccess = new ZincApiAccess();
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _bBorderApiAccess = new BBOrderViewPageApiAccess();

        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult OrderViewPage(string SCorderid = "6413902")
        {
            BestBuyOrdersViewPageModel model = new BestBuyOrdersViewPageModel();
            token = Request.Cookies["Token"];
            decimal price = _bBorderApiAccess.GetCountCad(ApiURL, token);
            model = _bBorderApiAccess.GetOrderViewPage(ApiURL, token, SCorderid);
            EncDecChannel _encDecChannel = new EncDecChannel();
            if (model.CreditCardId > 0)
            {
                var CreditCardDetail = creditCardApiAccess.GetCreditCardDetailById(ApiURL, token, model.CreditCardId);
                var AccountDetail = creditCardApiAccess.GetAccountDetailById(ApiURL, token, model.ZincAccountId);
                model.CreditCardName = _encDecChannel.DecryptStringFromBytes_Aes(CreditCardDetail.name_on_card);
                model.ZincAccountName = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.UserName);

            }
            ViewBag.ImagesURL = s3BucketURL;
            ViewBag.currencyexchange = price;
            ViewBag.ImageUrL_Large = s3BucketURL_large;

            return View(model);
        }

        public IActionResult SendOrderToZincOrderView(string ASINList, string orderid, string SellerOrderID)
        {
            token = Request.Cookies["Token"];


            string rootObject = _zincApiAccess.GetCustomerDetailForSendOrderToZincForOrderView(ApiURL, token, ASINList, orderid, SellerOrderID);

            return Json(rootObject);
        }

        public IActionResult ConfirmationOfZincOrderView(string JsonData, decimal profitlose, decimal percentageprofit)
        {
            token = Request.Cookies["Token"];
            ViewBag.profitlose = profitlose;
            ViewBag.percentagepro = percentageprofit;

            SaveZincOrders.RootObject rootObject = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(JsonData);

            var Object = _zincApiAccess.GetActiveZincAccountsList(ApiURL, token);
            rootObject.CreditCardDetail = Object.CreditCardDetail;
            rootObject.ZincAccounts = Object.ZincAccounts;

            return PartialView("~/Views/BBOrderViewPage/ConfirmationOfZincForOrderView.cshtml", rootObject);
        }

    }
}