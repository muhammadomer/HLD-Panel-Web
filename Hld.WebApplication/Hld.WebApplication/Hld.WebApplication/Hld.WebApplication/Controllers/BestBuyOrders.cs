using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    public class BestBuyOrders : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";

        CreditCardApiAccess creditCardApiAccess = new CreditCardApiAccess();
        BBOrderApiAccessNew _bBProductApiAccess = null;
        ProductApiAccess _productApiAccess = null;
        ProductWarehouseQtyApiAccess productWarehouseQtyApiAccess = null;
        BBOrderViewPageApiAccess _bBorderApiAccess = null;
        SellerCloudApiAccess sellerCloudApiAccess = null;

        string s3BucketURL = "";
        string s3BucketURL_large = "";
        public BestBuyOrders(IConfiguration configuration)
        {
            _configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _bBProductApiAccess = new BBOrderApiAccessNew();
            _productApiAccess = new ProductApiAccess();
            sellerCloudApiAccess = new SellerCloudApiAccess();
            productWarehouseQtyApiAccess = new ProductWarehouseQtyApiAccess();
            _bBorderApiAccess = new BBOrderViewPageApiAccess();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BestbuyOrderDetails(string order_id, string sellerCloudID, string CustomerName, string EmptyFirstTime, int? page, int ShippingBoxContain, int WHQStatus, string ZincStatus = "", string MarketPlaces = "", string OrderStatus = "", string sku = "", string orderDateTimeFrom = "", string orderDateTimeTo = "", string DSStatus = "", string PaymentStatus = "", string ShippingTags = "",string BBOrderStatus="")
        {
            if (ZincStatus == null)
            {
                ZincStatus = "";
            }
            if (ZincStatus != "")
            {

                //orderDateTimeTo = DateTime.Now.ToString("yyyy-MM-dd");
                //orderDateTimeFrom = Convert.ToDateTime(orderDateTimeTo).AddDays(-30).ToString("yyyy-MM-dd");

            }
            if (orderDateTimeFrom == null)
            {
                orderDateTimeFrom = "";

            }
            if (orderDateTimeTo == null)
            {
                orderDateTimeTo = "";

            }
            if (sku == null)
            {
                sku = "";
            }
            if (OrderStatus == null)
            {
                OrderStatus = "";
            }
            if (DSStatus == null)
            {
                DSStatus = "";
            }
            if (PaymentStatus == null)
            {
                PaymentStatus = "";
            }
            if (ShippingTags == null)
            {
                ShippingTags = "";
            }
            if (BBOrderStatus == null)
            {
                BBOrderStatus = "";
            }

            if (ShippingBoxContain == 0)
            {

            }
            if (WHQStatus == 0)
            {

            }

            BestBuyOrderSearchTotalCountViewModel SearchViewModel = new BestBuyOrderSearchTotalCountViewModel();


            string _ZincStatus =ZincStatus;
            SearchViewModel.ZincStatus = _ZincStatus;
            SearchViewModel.OrderStatus = OrderStatus;
            SearchViewModel.Sku = sku;
            SearchViewModel.DateFrom = orderDateTimeFrom;
            SearchViewModel.DateTo = orderDateTimeTo;
            SearchViewModel.DSStatus = DSStatus;
            SearchViewModel.PaymentStatus = PaymentStatus;
            SearchViewModel.ShippingTags = ShippingTags;
            SearchViewModel.BBOrderStatus = BBOrderStatus;
            SearchViewModel.ShippingBoxContain = ShippingBoxContain;
            SearchViewModel.WHQStatus = WHQStatus;

            SearchViewModel.Sort = "desc";
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            token = Request.Cookies["Token"];
            BestBuyOrdersViewModel viewModel = new BestBuyOrdersViewModel();
            viewModel.searchViewModel = SearchViewModel;
           

            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                ViewBag.TotalCount = null;
                return View(viewModel);
            }
            if (!string.IsNullOrEmpty(order_id) && order_id != "undefined")
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "SCOrders.order_id", order_id);

                ViewBag.TotalCount = model.count;
            }
            else if (!string.IsNullOrEmpty(sellerCloudID) && sellerCloudID != "undefined")
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "SCOrders.seller_cloud_order_id", sellerCloudID);
                ViewBag.TotalCount = model.count;
            }
            else if (!string.IsNullOrEmpty(CustomerName) && CustomerName != "undefined")
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "CustomerName", CustomerName);
                ViewBag.TotalCount = model.count;
            }

            // for searching order in bestBuy Orders Page

            else if (!string.IsNullOrEmpty(ZincStatus) && ZincStatus != "undefined" || (!string.IsNullOrEmpty(sku) && sku != "undefined") || (!string.IsNullOrEmpty(OrderStatus) && OrderStatus != "undefined") || (!string.IsNullOrEmpty(orderDateTimeFrom) && orderDateTimeFrom != "undefined") || (!string.IsNullOrEmpty(orderDateTimeTo) && orderDateTimeTo != "undefined") || (!string.IsNullOrEmpty(DSStatus) && DSStatus != "undefined") || (!string.IsNullOrEmpty(PaymentStatus) && PaymentStatus != "undefined") || (!string.IsNullOrEmpty(ShippingTags) && ShippingTags != "undefined") || ShippingBoxContain != 0|| WHQStatus != 0 || (!string.IsNullOrEmpty(BBOrderStatus) && BBOrderStatus != "undefined"))
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailSearchingTotalCount(ApiURL, token, SearchViewModel);
                ViewBag.TotalCount = model.count;
                // return RedirectToAction("BestBuOrdersDetail", new { ZincStatus = ZincStatus.ToString() });
                //return RedirectToAction("BestBuOrdersDetail", "BestBuyProduct", new { ZincStatus = _ZincStatus });

            }
            else
            {
                //  TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "NoFilter", "NoValue");
                // ViewBag.TotalCount = model.count;
                ViewBag.TotalCount = 5000;

            }

            return View(viewModel);
        }
        public string getString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {


                //   '\'apple\',\'banana\''
                string finalString = "";
                string[] splitString = text.Split(',');

                int end = splitString.Length - 1;
                StringBuilder stringBuilder = new StringBuilder();

                if (!(splitString.Length > 1))
                {
                    stringBuilder.Append(string.Concat("\'", splitString[0] + "\'"));
                }
                else
                {
                    for (int i = 0; i < splitString.Length; i++)
                    {
                        stringBuilder.Append(string.Concat("\'", splitString[i] + "\',"));
                    }
                }

                int index = stringBuilder.ToString().LastIndexOf(',');
                if (index != -1)
                {
                    finalString = stringBuilder.ToString().Remove(index);
                }
                else
                {
                    finalString = stringBuilder.ToString();
                }

                return finalString;
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        public ActionResult BBorderPartialView(string order_id, string sellerCloudID, string CustomerName, string EmptyFirstTime, int? page, int ShippingBoxContain, int WHQStatus, string sort, string ZincStatus, string OrderStatus, string sku, string orderDateTimeFrom, string orderDateTimeTo, string DSStatus, string PaymentStatus = "", string ShippingTags = "",string BBOrderStatus="")
        {
            if (ZincStatus == null)
            {
                ZincStatus = "";
            }
            if (ZincStatus != "" && ZincStatus != "undefined")
            {
                //orderDateTimeTo = DateTime.Now.ToString("yyyy-MM-dd");
                //orderDateTimeFrom = Convert.ToDateTime(orderDateTimeTo).AddDays(-30).ToString("yyyy-MM-dd");


            }
            if (orderDateTimeFrom == null)
            {
                orderDateTimeFrom = "";
            }
            if (orderDateTimeTo == null)
            {
                orderDateTimeTo = "";
            }
            if (sku == null)
            {
                sku = "";
            }
            if (OrderStatus == null)
            {
                OrderStatus = "";
            }
            if (PaymentStatus == null)
            {
                PaymentStatus = "";
            }
            if (ShippingTags == null)
            {
                ShippingTags = "";
            }
            if (BBOrderStatus == null)
            {
                    BBOrderStatus = "";
            }
            if (ShippingBoxContain == 0)
            {

            }
            if (WHQStatus == 0)
            {

            }
            if (string.IsNullOrEmpty(sort))
            {
                sort = "desc";
            }

            IPagedList<BestBuyOrdersViewModel> data = LoadOrderData(order_id, sellerCloudID, CustomerName, page, ShippingBoxContain,WHQStatus, sort, ZincStatus, OrderStatus, sku, orderDateTimeFrom, orderDateTimeTo, DSStatus, PaymentStatus, ShippingTags, BBOrderStatus);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;

            return PartialView("~/Views/BestBuyOrders/_BBorderPartialView.cshtml", data);


        }


        private IPagedList<BestBuyOrdersViewModel> LoadOrderData(string order_id, string sellerCloudID, string CustomerName, int? page, int ShippingBoxContain,int WHQStatus, string sort, string ZincStatus, string orderStatus, string sku, string orderDateFrom, string orderDateTo, string DSStatus, string PaymentStatus, string ShippingTags,string BBOrderStatus)
        {
            string modifiedZincStatus = "";
            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<BestBuyOrdersViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;

            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * 25;
            }


            token = Request.Cookies["Token"];
            List<BestBuyOrdersViewModel> _viewModel = null;
            if (!string.IsNullOrEmpty(order_id) && order_id != "undefined")
            {

                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilter(ApiURL, token, "SCOrders.order_id", order_id, startlimit, endLimit, sort);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                var totalQuantity = _viewModel.Sum(e => e.TotalQuantity);
                ViewBag.TotalProductsInOrders = totalQuantity;
                ViewBag.TotalOrders = _viewModel.Count;



            }
            else if (!string.IsNullOrEmpty(sellerCloudID) && sellerCloudID != "undefined")
            {
                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilter(ApiURL, token, "SCOrders.seller_cloud_order_id", sellerCloudID, startlimit, endLimit, sort);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "SCOrders.seller_cloud_order_id", sellerCloudID);

                ViewBag.TotalOrders = _viewModel.Count;
                var totalQuantity = _viewModel.Sum(e => e.TotalQuantity);
                ViewBag.TotalProductsInOrders = totalQuantity;
            }
            else if (!string.IsNullOrEmpty(CustomerName) && CustomerName != "undefined")
            {
                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilter(ApiURL, token, "CustomerName", CustomerName, startlimit, endLimit, sort);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);

                ViewBag.TotalOrders = _viewModel.Count;
                var totalQuantity = _viewModel.Sum(e => e.TotalQuantity);
                ViewBag.TotalProductsInOrders = totalQuantity;
            }
            else if (!string.IsNullOrEmpty(ZincStatus) && ZincStatus != "undefined" || (!string.IsNullOrEmpty(sku) && sku != "undefined") || (!string.IsNullOrEmpty(orderStatus) && orderStatus != "undefined") || (!string.IsNullOrEmpty(orderDateFrom) && orderDateFrom != "undefined") || (!string.IsNullOrEmpty(orderDateTo) && orderDateTo != "undefined") || (!string.IsNullOrEmpty(DSStatus) && DSStatus != "undefined") || (!string.IsNullOrEmpty(PaymentStatus) && PaymentStatus != "undefined") || (!string.IsNullOrEmpty(ShippingTags) && ShippingTags != "undefined") || ShippingBoxContain != 0|| WHQStatus!=0 || (!string.IsNullOrEmpty(BBOrderStatus) && BBOrderStatus != "undefined"))
            {
                modifiedZincStatus = ZincStatus;
                BestBuyOrderSearchTotalCountViewModel SearchViewModel = new BestBuyOrderSearchTotalCountViewModel();
                SearchViewModel.ZincStatus = modifiedZincStatus;

                SearchViewModel.StartIndex = startlimit;
                SearchViewModel.EndIndex = endLimit;

                SearchViewModel.OrderStatus = orderStatus;
                SearchViewModel.Sku = sku;
                SearchViewModel.DateFrom = orderDateFrom;
                SearchViewModel.DateTo = orderDateTo;
                SearchViewModel.DSStatus = DSStatus;
                SearchViewModel.PaymentStatus = PaymentStatus;
                SearchViewModel.ShippingTags = ShippingTags;
                SearchViewModel.BBOrderStatus = BBOrderStatus;
                SearchViewModel.ShippingBoxContain = ShippingBoxContain;
                SearchViewModel.WHQStatus = WHQStatus;
                SearchViewModel.Sort = "desc";
                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetailSearch(ApiURL, token, SearchViewModel);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);

                ViewBag.TotalOrders = _viewModel.Count;
                var totalQuantity = _viewModel.Sum(e => e.TotalQuantity);
                ViewBag.TotalProductsInOrders = totalQuantity;
            }
            else
            {
                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetail(ApiURL, token, startlimit, endLimit, sort);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);


                ViewBag.TotalOrders = _viewModel.Count;
                var totalQuantity = _viewModel.Sum(e => e.TotalQuantity);
                ViewBag.TotalProductsInOrders = totalQuantity;
                //TempData["OnOrder"] = _viewModel.Select(a=>a.OnOrder).ToList();
            }
            return data;
        }

        public IActionResult BBOrderViewPage(string SCorderid = "6413902")
        {
            BestBuyOrdersViewPageModel model = new BestBuyOrdersViewPageModel();
            token = Request.Cookies["Token"];
            decimal price = _bBorderApiAccess.GetCountCad(ApiURL, token);
            model = _bBProductApiAccess.GetOrderViewPage(ApiURL, token, SCorderid);
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
    }
}
