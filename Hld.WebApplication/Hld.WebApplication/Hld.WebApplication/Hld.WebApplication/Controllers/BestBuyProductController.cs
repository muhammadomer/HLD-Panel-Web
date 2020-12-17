using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.Models;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]

    public class BestBuyProductController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";

        CreditCardApiAccess creditCardApiAccess = new CreditCardApiAccess();
        BBProductApiAccess _bBProductApiAccess = null;
        ProductApiAccess _productApiAccess = null;
        ProductWarehouseQtyApiAccess productWarehouseQtyApiAccess = null;
        SellerCloudApiAccess sellerCloudApiAccess = null;

        string s3BucketURL = "";
        string s3BucketURL_large = "";
        public BestBuyProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _bBProductApiAccess = new BBProductApiAccess();
            _productApiAccess = new ProductApiAccess();
            sellerCloudApiAccess = new SellerCloudApiAccess();
            productWarehouseQtyApiAccess = new ProductWarehouseQtyApiAccess();
        }

        [HttpGet]
        public ActionResult Index()
        {
            token = Request.Cookies["Token"];
            return View(_bBProductApiAccess.GetAllBestBuyProducts(ApiURL, token));
        }
        [HttpGet]
        public ActionResult InsertProductToBBProduct(string SKU)
        {
            token = Request.Cookies["Token"];

            sellerCloudApiAccess.InsertProductDataToProductTable(ApiURL, token);
            return RedirectToAction("PropertyPage", "BestBuyProduct", new { ProductSKU = SKU });
        }


        public IActionResult ImportDataFromFile()
        {
            token = Request.Cookies["Token"];
            List<BBProductViewModel> ListbBProductJsonViewModels = new List<BBProductViewModel>();
            var client = new WebClient();
            var json = client.DownloadString(@"C:\Users\Amir Javaid\source\repos\Hld.WebApplication\Hld.WebApplication\wwwroot\NextRecord.json");
            var result = JsonConvert.DeserializeObject<RootObject>(json);

            for (int i = 0; i < result.offers.Count; i++)
            {
                BBProductViewModel _model = new BBProductViewModel();
                List<AllPrice> allPrice = result.offers[i].all_prices.ToList();
                foreach (var _allPrice in allPrice)
                {
                    _model.DiscountEndDate = _allPrice.discount_end_date;
                    _model.DiscountStartDate = _allPrice.discount_start_date;
                    _model.UnitDiscountPrice_SellingPrice = _allPrice.unit_discount_price;
                    _model.UnitOriginPrice_MSRP = _allPrice.unit_origin_price;
                }

                _model.CategoryCode = result.offers[i].category_code;
                _model.CategoryName = result.offers[i].category_label;

                _model.LogisticClass_Code = result.offers[i].logistic_class.code;

                List<ProductReference> listproductReferences = result.offers[i].product_references;
                foreach (var _listproductReferences in listproductReferences)
                {
                    _model.Reference_UPC = _listproductReferences.reference;
                }

                _model.Product_Sku = result.offers[i].product_sku;
                _model.Product_Title = result.offers[i].product_title;
                _model.Quantity_BBQ = result.offers[i].quantity;
                _model.ShopSKU_OfferSKU = result.offers[i].shop_sku;

                ListbBProductJsonViewModels.Add(_model);
            }

            foreach (var item in ListbBProductJsonViewModels)
            {
                _bBProductApiAccess.AddBestBuyProduct(ApiURL, token, item);
            }

            return View();
        }
        [Authorize(Policy = "Access to BB property page")]
        [HttpGet]
        public ActionResult PropertyPage(string ProductSKU, string BBSKU)
        {
            token = Request.Cookies["Token"];
            int productID = 0;
            // sellerCloudApiAccess.InsertProductDataToProductTable(ApiURL, token);
            BBProductViewModel model = new BBProductViewModel();
            sellerCloudApiAccess.InsertProductDataToProductTable(ApiURL, token);
            if (BBSKU != null)
            {

                productID = _productApiAccess.CheckProductId(ApiURL, token, BBSKU);

                if (productID == 0)
                {
                    Response.StatusCode = 404;
                    return View("~/Views/Product/404NotFound.cshtml", BBSKU);
                }

                token = Request.Cookies["Token"];
                model = _bBProductApiAccess.BBProductByBBSKU(ApiURL, token, BBSKU);
            }
            else
            {
                productID = _productApiAccess.GetProductIDBySKU(ApiURL, token, ProductSKU);
                if (productID == 0)
                {
                    Response.StatusCode = 404;
                    return View("~/Views/Product/404NotFound.cshtml", ProductSKU);
                }


                token = Request.Cookies["Token"];
                model = _bBProductApiAccess.GetBestBuyProductDetail_ProductID(ApiURL, token, ProductSKU);
            }

            if (model.ShopSKU_OfferSKU != null)
            {
                model.DiscountEndDate = model.DiscountEndDate.Value != DateTime.MinValue ? model.DiscountEndDate.Value : DateTime.Now;
                model.DiscountStartDate = model.DiscountStartDate.Value != DateTime.MinValue ? model.DiscountStartDate.Value : DateTime.Now;

            }
            return View(model);
        }

        public ActionResult UpdateBestBuyProduct(BBProductViewModel model)
        {
            token = Request.Cookies["Token"];
            bool status = false;


            status = _productApiAccess.UpdateProductDropshipAndQty(ApiURL, token, model);
            if (status)
            {

                BestBuyDropShipQtyMovementViewModel qtyViewModel = new BestBuyDropShipQtyMovementViewModel();
                qtyViewModel.ProductSku = model.ShopSKU_OfferSKU;
                qtyViewModel.DropShipQuantity = model.dropship_Qty;
                qtyViewModel.DropShipStatus = model.dropship_status;
                qtyViewModel.DropshipComments = model.DropshipComments;
                qtyViewModel.OrderDate = DateTime.Now;
                productWarehouseQtyApiAccess.SaveBestBuyQtyMovementForDropship(ApiURL, token, qtyViewModel);
                return RedirectToAction("PropertyPage", "BestBuyProduct", new { ProductSKU = model.ShopSKU_OfferSKU });
            }


            return View();
        }


        public ActionResult SearchOrders(string ZincStatus, string MarketPlaces, string OrderStatus)
        {

            return RedirectToAction("BestBuOrdersDetail", "BestBuyProduct", new { ZincStatus = ZincStatus.ToString() });
        }

        public AccountDetailFroZincViewModel CreditCardDetail(int CardId, int AccountId)
        {
            
            AccountDetailFroZincViewModel viewModel = new AccountDetailFroZincViewModel();
            EncDecChannel _encDecChannel = new EncDecChannel();
            token = Request.Cookies["Token"];
            var CreditCardDetail = creditCardApiAccess.GetCreditCardDetailById(ApiURL, token, CardId);
            var AccountDetail = creditCardApiAccess.GetAccountDetailById(ApiURL, token, AccountId);
            viewModel.CardName = _encDecChannel.DecryptStringFromBytes_Aes(CreditCardDetail.name_on_card);
            viewModel.AccountName = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.UserName);
            return viewModel;
        }

        public ZincAccountsViewModel ZincAccountDetail(int Id)
        {
            var AccountDetail = creditCardApiAccess.GetAccountDetailById(ApiURL, token, Id);
            return AccountDetail;
        }

        //,string search_marketPlace,string search_shipmentOrderStatus
        [Authorize(Policy = "Access to Orders")]
        [HttpGet]
        public ActionResult BestBuOrdersDetail(string order_id, string sellerCloudID, string CustomerName,string EmptyFirstTime, int? page, int ShippingBoxContain, string ZincStatus = "", string MarketPlaces = "", string OrderStatus = "", string sku = "", string orderDateTimeFrom = "", string orderDateTimeTo = "", string DSStatus = "", string PaymentStatus = "", string ShippingTags = "")
        {

            if (ZincStatus == null)
            {
                ZincStatus = "";
            }
            if (ZincStatus != "")
            {

                orderDateTimeTo = DateTime.Now.ToString("yyyy-MM-dd");
                orderDateTimeFrom = Convert.ToDateTime(orderDateTimeTo).AddDays(-30).ToString("yyyy-MM-dd");

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

            if (ShippingBoxContain==0) {
            
            }
            
            BestBuyOrderSearchTotalCountViewModel SearchViewModel = new BestBuyOrderSearchTotalCountViewModel();


            string _ZincStatus = getString(ZincStatus);
            SearchViewModel.ZincStatus = _ZincStatus;
            SearchViewModel.OrderStatus = OrderStatus;
            SearchViewModel.Sku = sku;
            SearchViewModel.DateFrom = orderDateTimeFrom;
            SearchViewModel.DateTo = orderDateTimeTo;
            SearchViewModel.DSStatus = DSStatus;
            SearchViewModel.PaymentStatus = PaymentStatus;
            SearchViewModel.ShippingTags = ShippingTags;
            SearchViewModel.ShippingBoxContain = ShippingBoxContain;

            SearchViewModel.Sort = "desc";
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            token = Request.Cookies["Token"];
            BestBuyOrdersViewModel viewModel = new BestBuyOrdersViewModel();
            viewModel.searchViewModel = SearchViewModel;
            viewModel.searchViewModel.ZincStatus = ZincStatus;

            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                ViewBag.TotalCount =null;
                return View(viewModel);
            }
            if (!string.IsNullOrEmpty(order_id) && order_id != "undefined")
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "order_id", order_id);

                ViewBag.TotalCount = model.count;
            }
            else if (!string.IsNullOrEmpty(sellerCloudID) && sellerCloudID != "undefined")
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "sellerCloudID", sellerCloudID);
                ViewBag.TotalCount = model.count;
            }
            else if (!string.IsNullOrEmpty(CustomerName) && CustomerName != "undefined")
            {
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "CustomerName", CustomerName);
                ViewBag.TotalCount = model.count;
            }

            // for searching order in bestBuy Orders Page

            else if (!string.IsNullOrEmpty(ZincStatus) && ZincStatus != "undefined" || (!string.IsNullOrEmpty(sku) && sku != "undefined") || (!string.IsNullOrEmpty(OrderStatus) && OrderStatus != "undefined") || (!string.IsNullOrEmpty(orderDateTimeFrom) && orderDateTimeFrom != "undefined") || (!string.IsNullOrEmpty(orderDateTimeTo) && orderDateTimeTo != "undefined") || (!string.IsNullOrEmpty(DSStatus) && DSStatus != "undefined") || (!string.IsNullOrEmpty(PaymentStatus) && PaymentStatus != "undefined") || (!string.IsNullOrEmpty(ShippingTags) && ShippingTags != "undefined") || ShippingBoxContain !=0 )
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

        [HttpPost]
        public ActionResult BBorderPartialView(string order_id, string sellerCloudID, string CustomerName, string EmptyFirstTime, int? page,int ShippingBoxContain, string sort, string ZincStatus, string OrderStatus, string sku, string orderDateTimeFrom, string orderDateTimeTo, string DSStatus, string PaymentStatus = "", string ShippingTags = "")
        {
            if (ZincStatus == null)
            {
                ZincStatus = "";
            }
            if (ZincStatus != "" && ZincStatus != "undefined")
            {
                orderDateTimeTo = DateTime.Now.ToString("yyyy-MM-dd");
                orderDateTimeFrom = Convert.ToDateTime(orderDateTimeTo).AddDays(-30).ToString("yyyy-MM-dd");

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

            if (ShippingBoxContain == 0)
            {
                
            }
            if (string.IsNullOrEmpty(sort))
            {
                sort = "desc";
            }

                IPagedList<BestBuyOrdersViewModel> data = LoadOrderData(order_id, sellerCloudID, CustomerName, page, ShippingBoxContain, sort, ZincStatus, OrderStatus, sku, orderDateTimeFrom, orderDateTimeTo, DSStatus, PaymentStatus, ShippingTags);
                ViewBag.S3BucketURL = s3BucketURL;
                ViewBag.S3BucketURL_large = s3BucketURL_large;
                 
                return PartialView("~/Views/BestBuyProduct/_BBorderPartialView.cshtml", data);
            

        }


        private IPagedList<BestBuyOrdersViewModel> LoadOrderData(string order_id, string sellerCloudID, string CustomerName, int? page, int ShippingBoxContain, string sort, string ZincStatus, string orderStatus, string sku, string orderDateFrom, string orderDateTo, string DSStatus, string PaymentStatus, string ShippingTags)
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

                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilter(ApiURL, token, "order_id", order_id, startlimit, endLimit, sort);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                var totalQuantity = _viewModel.Sum(e => e.TotalQuantity);
                ViewBag.TotalProductsInOrders = totalQuantity;
                ViewBag.TotalOrders = _viewModel.Count;



            }
            else if (!string.IsNullOrEmpty(sellerCloudID) && sellerCloudID != "undefined")
            {
                _viewModel = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilter(ApiURL, token, "sellerCloudID", sellerCloudID, startlimit, endLimit, sort);
                data = new StaticPagedList<BestBuyOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                TotalCountWithBestBuyOrderViewModel model = _bBProductApiAccess.GetAllBestBuyOrdersDetailGlobalFilterTotalCount(ApiURL, token, "sellerCloudID", sellerCloudID);

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
            else if (!string.IsNullOrEmpty(ZincStatus) && ZincStatus != "undefined" || (!string.IsNullOrEmpty(sku) && sku != "undefined") || (!string.IsNullOrEmpty(orderStatus) && orderStatus != "undefined") || (!string.IsNullOrEmpty(orderDateFrom) && orderDateFrom != "undefined") || (!string.IsNullOrEmpty(orderDateTo) && orderDateTo != "undefined") || (!string.IsNullOrEmpty(DSStatus) && DSStatus != "undefined") || (!string.IsNullOrEmpty(PaymentStatus) && PaymentStatus != "undefined") || (!string.IsNullOrEmpty(ShippingTags) && ShippingTags != "undefined")|| ShippingBoxContain != 0)
            {
                modifiedZincStatus = getString(ZincStatus);
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
                SearchViewModel.ShippingBoxContain = ShippingBoxContain;
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

      
        public IActionResult GetWatchListSummary()
        {
            string token = Request.Cookies["Token"];
            ViewBag.TotalCount = _bBProductApiAccess.GetWatchListSummaryCount(ApiURL, token);

            return View();
        }

        public ActionResult bestBuyUpdateLogs(int page = 1)
        {
            IPagedList<BestBuyUpdateViewModel> data = null;
            string token = Request.Cookies["Token"];
            int pageNumber = page;
            int offset = 0;
            int pageSize = 25;


            offset = (pageNumber - 1) * 25;

            List<BestBuyUpdateViewModel> listmodel = new List<BestBuyUpdateViewModel>();
            listmodel = _bBProductApiAccess.bestBuyUpdateLogs(ApiURL, token, offset);
            ViewBag.Records = listmodel;
            data = new StaticPagedList<BestBuyUpdateViewModel>(listmodel, pageNumber, pageSize, listmodel.Count);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/BestBuyProduct/bestBuyUpdateLogs.cshtml", data);

        }



        public IActionResult SummaryList(int? page)
        {
            token = Request.Cookies["Token"];
            IPagedList<BestBuyUpdateViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }            //return PartialView("~/Views/BestBuyUpdateLogs/BustBuuPartialView.cshtml", data);

            List<BestBuyUpdateViewModel> listmodel = new List<BestBuyUpdateViewModel>();
            listmodel = _bBProductApiAccess.bestBuyUpdateLogs(ApiURL, token, endLimit);
            data = new StaticPagedList<BestBuyUpdateViewModel>(listmodel, pageNumber, pageSize, listmodel.Count);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;

            data = new StaticPagedList<BestBuyUpdateViewModel>(listmodel, pageNumber, pageSize, listmodel.Count);
            return PartialView("~/Views/BestBuyProduct/BestBuyUpdatelistPartialView.cshtml", data);
        }

        public IActionResult SummaryCount()
        {
            string token = Request.Cookies["Token"];
            int listmodel = 0;
            listmodel = _bBProductApiAccess.GetWatchListSummaryCount(ApiURL, token);
            ViewBag.logsRecords = listmodel;
            return View();
        }

        [HttpGet]
        public GetExplainAmountViewModel GetExplainAmount(string sellercloudId, string productSku)
        {
            token = Request.Cookies["Token"];
            GetExplainAmountViewModel model = new GetExplainAmountViewModel();
            model = sellerCloudApiAccess.GetExplainAmount(ApiURL, token, sellercloudId, productSku);           
            return model;
        }

    }


}