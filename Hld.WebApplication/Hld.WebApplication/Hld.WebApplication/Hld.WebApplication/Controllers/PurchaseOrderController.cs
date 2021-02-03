using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using BarcodeLib;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    [Authorize(Policy = "Access to PO")]
    public class PurchaseOrderController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string SCRestURL = "";
        int POMasterID = 0;
        PurchaseOrderApiAccess _ApiAccess = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        OrderNotesAPiAccess _OrderApiAccess = null;

        string s3BucketURL = "";
        string s3BucketURL_large = "";
        public PurchaseOrderController(IConfiguration configuration)
        {
            _configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _ApiAccess = new PurchaseOrderApiAccess();
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            _encDecChannel = new EncDecChannel();
            _OrderApiAccess = new OrderNotesAPiAccess();
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "Access to Import PO")]
        [HttpGet]
        public IActionResult ImportPurchaseOrders()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ImportPurchaseOrders(PurchaseModel data)
        {

            token = Request.Cookies["Token"];
            List<CheckMissingOrderViewModel> OrderList = new List<CheckMissingOrderViewModel>();
            List<string> missingSkuList = new List<string>();
            string newlist = data.Order;
            var lines = newlist.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Trim() != string.Empty)
                {
                    OrderList.Add(new CheckMissingOrderViewModel
                    {
                        OrderID = Convert.ToInt32(line.Trim())
                    });


                }
            }
            MissingOrderReturnViewModel detail = _ApiAccess.CheckPOOrderINDB(ApiURL, token, OrderList);
            var sessionMissingSku = JsonConvert.SerializeObject(detail.MissingOrder);
            HttpContext.Session.SetString("MissingPOOrders", sessionMissingSku);
            ViewBag.MissingOrderCount = detail.MissingOrderCount;
            ViewBag.ExistingOrderCount = detail.ExistingOrderCount;
            ViewBag.ExistingOrder = detail.ExistingOrder;
            ViewBag.MissingOrder = detail.MissingOrder;

            ViewData["existingPOOrder"] = detail.ExistingOrder;

            return View(data);


        }

        public IActionResult ImportPurchaseOrderFromSellerCloud()
        {

            token = Request.Cookies["Token"];

            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            var key = HttpContext.Session.GetString("MissingPOOrders");
            List<int> addedList = new List<int>();
            List<int> ErrorList = new List<int>();


            List<int> missingOrderList = JsonConvert.DeserializeObject<List<int>>(key);


            foreach (var line in missingOrderList)
            {
                PurchaseOrderViewModel.PurchaseOrderData purchaseOrderData = new PurchaseOrderViewModel.PurchaseOrderData();
                PurchaseOrderDataViewModel purchaseOrderDataViewModel = new PurchaseOrderDataViewModel();
                List<PurchaseOrderItemsDataViewModel> purchaseOrderItemsDataViewModel = new List<PurchaseOrderItemsDataViewModel>();

                purchaseOrderData = _ApiAccess.GetPurchaseOrderByIdFromSellerCloud(authenticate.access_token, SCRestURL, line.ToString(), ApiURL, token);
                purchaseOrderDataViewModel.CompanyId = purchaseOrderData.Purchase.CompanyId;
                purchaseOrderDataViewModel.CurrencyCode = purchaseOrderData.Purchase.CurrencyCode;
                purchaseOrderDataViewModel.OrderedOn = purchaseOrderData.Purchase.OrderedOn;
                purchaseOrderDataViewModel.POId = purchaseOrderData.Purchase.POId;
                purchaseOrderDataViewModel.DefaultWarehouseID = purchaseOrderData.Purchase.DefaultWarehouseID;
                purchaseOrderDataViewModel.VendorId = purchaseOrderData.Purchase.VendorId;
                purchaseOrderDataViewModel.POStatus = 1;
                foreach (var item in purchaseOrderData.Items)
                {
                    PurchaseOrderItemsDataViewModel ItemsDataViewModel = new PurchaseOrderItemsDataViewModel();
                    ItemsDataViewModel.ID = item.ID;
                    ItemsDataViewModel.ProductID = item.ProductID;
                    ItemsDataViewModel.PurchaseID = item.PurchaseID;
                    ItemsDataViewModel.QtyOnHand = item.QtyOnHand;
                    ItemsDataViewModel.QtyOrdered = item.QtyOrdered;
                    ItemsDataViewModel.QtyReceived = item.QtyReceived;
                    ItemsDataViewModel.UnitPrice = item.UnitPrice;
                    ItemsDataViewModel.SkuStatus = 1;
                    ItemsDataViewModel.ProductTitle = item.ProductName;
                    purchaseOrderItemsDataViewModel.Add(ItemsDataViewModel);
                }
                purchaseOrderDataViewModel.items = purchaseOrderItemsDataViewModel;
                bool status = _ApiAccess.SavepurchaseOrder(token, ApiURL, purchaseOrderDataViewModel);
                if (status == true)
                {
                    addedList.Add(line);
                }
                else
                {
                    ErrorList.Add(line);
                }

            }


            return Json(new { AddedOrder = addedList, ErrorOrder = ErrorList, });

        }

        [HttpGet]
        public IActionResult PurchaseOrders(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, int VendorID = 0, int POID = 0, string Vendor = "", string ItemType = "")
        {
            int count = 0;
            string OpenItem = "";
            string ReceivedItem = "";
            string OrderdItem = "";
            string NotShipped = "";

            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
                PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "OpenItem")
                    {
                        OpenItem = "HasValue";
                    }
                    if (item == "ReceivedItem")
                    {
                        ReceivedItem = "HasValue";
                    }
                    if (item == "OrderdItem")
                    {
                        OrderdItem = "HasValue";
                    }
                    if (item == "NotShipped")
                    {
                        NotShipped = "HasValue";
                    }
                }
            }

            token = Request.Cookies["Token"];
            PurchaseOrdersViewModel viewModel = new PurchaseOrdersViewModel();
            viewModel.Vendor = Vendor;


            viewModel.POId = POID;
            if (VendorID != 0)
            {
                POMasterID = VendorID;

            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }
            viewModel.VendorId = POMasterID;

            count = _ApiAccess.GetAllPurchaseOrdersCount(ApiURL, token, CurrentDate, PreviousDate, POMasterID, POID, OpenItem, ReceivedItem, OrderdItem, NotShipped);

            ViewBag.TotalCount = count;
            GetSummaryForPOViewModel model = new GetSummaryForPOViewModel();

            model = _ApiAccess.GetAllPurchaseOrdersForPO(CurrentDate, PreviousDate, ApiURL, token, POMasterID, POID, OpenItem, ReceivedItem, OrderdItem, NotShipped);
            TempData["recQty"] = model.recQty;
            TempData["ordQty"] = model.ordQty;
            TempData["OpenQty"] = model.OpenQty;
            TempData["ReceviedAmount"] = model.ReceviedAmount;
            TempData["orderamount"] = model.orderamount;
            TempData["OpenAmount"] = model.OpenAmount;
            TempData["shiQty"] = model.shiQty;
            TempData["shipedamount"] = model.shipedamount;
            return View(viewModel);


        }
        [HttpPost]
        public IActionResult PurchaseOrdersList(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, int? page, int POID = 0, int Vendor = 0, string ItemType = "")
        {

            string OpenItem = "";
            string ReceivedItem = "";
            string OrderdItem = "";
            string NotShipped = "";

            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
                PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "OpenItem")
                    {
                        OpenItem = "HasValue";
                    }
                    if (item == "ReceivedItem")
                    {
                        ReceivedItem = "HasValue";
                    }
                    if (item == "OrderdItem")
                    {
                        OrderdItem = "HasValue";
                    }
                    if (item == "NotShipped")
                    {
                        NotShipped = "HasValue";
                    }
                }
            }
            token = Request.Cookies["Token"];
            if (Vendor != 0)
            {
                POMasterID = Vendor;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<PurchaseOrdersViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;

            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }


            List<PurchaseOrdersViewModel> _viewModel = null;


            ViewBag.recQty = Convert.ToInt32(TempData["recQty"]);
            ViewBag.ordQty = Convert.ToInt32(TempData["ordQty"]);
            ViewBag.OpenQty = Convert.ToInt32(TempData["OpenQty"]);
            ViewBag.ReceviedAmount = Convert.ToDecimal(TempData["ReceviedAmount"]);
            ViewBag.orderamount = Convert.ToDecimal(TempData["orderamount"]);
            ViewBag.OpenAmount = Convert.ToDecimal(TempData["OpenAmount"]);
            ViewBag.shiQty = Convert.ToInt32(TempData["shiQty"]);
            ViewBag.shipedamount = Convert.ToDecimal(TempData["shipedamount"]);
            _viewModel = _ApiAccess.GetAllPurchaseOrders(ApiURL, token, CurrentDate, PreviousDate, POMasterID, startlimit, endLimit, POID, OpenItem, ReceivedItem, OrderdItem,NotShipped);
            data = new StaticPagedList<PurchaseOrdersViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);

            return PartialView("~/Views/PurchaseOrder/_PurchaseOrderPartial.cshtml", data);
        }

        [HttpGet]
        public IActionResult PurchaseOrdersItems(int POId, string SKU = "", string Title = "", Boolean OpenQty = false)
        {
            token = Request.Cookies["Token"];
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            searchPOitemViewModel searchPOitemView = new searchPOitemViewModel();
            searchPOitemView.POId = POId;
            searchPOitemView.vendorID = POMasterID;
            searchPOitemView.SKU = SKU;
            searchPOitemView.Title = Title;
            searchPOitemView.OpenQty = OpenQty;
            PurchaseOrderModel count = _ApiAccess.GetAllPurchaseOrdersItems(ApiURL, token, searchPOitemView);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            count.SKU = SKU;
            count.Title = Title;
            count.OpenQty = OpenQty;
            if (count.ListItems == null)
            {
                Response.StatusCode = 404;
                return View("~/Views/PurchaseOrder/ErrorOrder.cshtml", POId.ToString());

            }
            return View(count);
        }


        [HttpGet]
        public bool SaveCurrencyExchange(int POId, decimal exchangeRate)
        {
            token = Request.Cookies["Token"];
            UpdatePOExchangeViewModel searchPOitemView = new UpdatePOExchangeViewModel();
            searchPOitemView.POId = POId;
            searchPOitemView.ExchangeRate = exchangeRate;
            bool count = _ApiAccess.SaveCurrencyExchange(token, ApiURL, searchPOitemView);

            return count;
        }
        [HttpGet]
        public IActionResult AddUpdateShipdate(int POID)
        {
            token = Request.Cookies["Token"];
            UpdatePOAcceptedViewModel updatePOAccepted = new UpdatePOAcceptedViewModel();

            updatePOAccepted = _ApiAccess.GetPOShiDatestoupdate(ApiURL, token, POID);
            return PartialView("~/Views/PurchaseOrder/AddUpdateShip.cshtml", updatePOAccepted);

        }

        [HttpGet]
        public int SaveCurrencyChange(int POId, int currencyCode)
        {
            token = Request.Cookies["Token"];
            int count = _ApiAccess.UpdateCurrency(ApiURL, token, POId, currencyCode);
            return count;
        }
        [HttpPost]
        public bool AddUpdateExpected(UpdatePOAcceptedViewModel updatePO)
        {
            bool count = false;
            token = Request.Cookies["Token"];

            if (updatePO.IsAccepted == 0)
            {
                count = _ApiAccess.SavePOAsAccepted(token, ApiURL, updatePO);
            }
            else
            {
                count = _ApiAccess.UpdatePOShipDate(token, ApiURL, updatePO);
            }


            return count;
        }

        [HttpGet]
        public IActionResult GetPOProduct(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, int POID = 0, int VendorID = 0, string Vendor = "", string SKU = "", string title = "", string EmptyFirstTime="", string ItemType = "")
        {
            string OpenItem = "";
            string ReceivedItem = "";
            string OrderdItem = "";
            string NotShipped = "";
            string ShippedButNotReceived = "";
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
                PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "OpenItem")
                    {
                        OpenItem = "HasValue";
                    }
                    if (item == "ReceivedItem")
                    {
                        ReceivedItem = "HasValue";
                    }
                    if (item == "OrderdItem")
                    {
                        OrderdItem = "HasValue";
                    }
                    if (item == "NotShipped")
                    {
                        NotShipped = "HasValue";
                    }
                    if (item == "ShippedButNotReceived")
                    {
                        ShippedButNotReceived = "HasValue";
                    }
                }
            }

            token = Request.Cookies["Token"];

            PurchaseOrderItemsViewModel viewModel = new PurchaseOrderItemsViewModel();
            viewModel.ProductID = SKU;
            viewModel.ProductTitle = title;
            viewModel.Vendor = Vendor;
            int? PO = POID;

            viewModel.PurchaseID = PO == 0 ? null : PO;
            if (VendorID != 0)
            {
                POMasterID = VendorID;
                viewModel.VendorID = VendorID;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);

            }
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {

            }
            else
            {

            
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            // var list = _ApiAccess.GetPOProductsList(ApiURL, token, ID, 10, 0, POID, SKU, title, OpenItem, ReceivedItem, OrderdItem);
            GetSummaryandCountPOViewModel getSummaryand = _ApiAccess.GetCounter(ApiURL, token, CurrentDate, PreviousDate, POMasterID, POID, SKU, title, OpenItem, ReceivedItem, OrderdItem, NotShipped, ShippedButNotReceived);

            ViewBag.TotalCount = getSummaryand.count;
            TempData["recQty"] = getSummaryand.recQty;
            TempData["ordQty"] = getSummaryand.ordQty;
            TempData["OpenQty"] = getSummaryand.OpenQty;
            TempData["ReceviedAmount"] = getSummaryand.ReceviedAmount;
            TempData["orderamount"] = getSummaryand.orderamount;
            TempData["OpenAmount"] = getSummaryand.OpenAmount;
            TempData["POcount"] = getSummaryand.POcount;
            TempData["count"] = getSummaryand.count;
            TempData["shiQty"] = getSummaryand.shiQty;
            TempData["shipedamount"] = getSummaryand.shipedamount;
            }
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult GetPOProductList(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, int? page, int Vendor = 0, int POID = 0, string SKU = "", string title = "", string ItemType = "")
        {
            string OpenItem = "";
            string ReceivedItem = "";
            string OrderdItem = "";
            string NotShipped = "";
            string ShippedButNotReceived = "";
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
                PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "OpenItem")
                    {
                        OpenItem = "HasValue";
                    }
                    if (item == "ReceivedItem")
                    {
                        ReceivedItem = "HasValue";
                    }
                    if (item == "OrderdItem")
                    {
                        OrderdItem = "HasValue";
                    }
                    if (item == "NotShipped")
                    {
                        NotShipped = "HasValue";
                    }
                    if (item == "NotShipped")
                    {
                        NotShipped = "HasValue";
                    }
                    if (item == "ShippedButNotReceived")
                    {
                        ShippedButNotReceived = "HasValue";
                    }
                }
            }
            token = Request.Cookies["Token"];
            if (Vendor != 0)
            {
                POMasterID = Vendor;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<PurchaseOrderItemsViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;

            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }

            List<PurchaseOrderItemsViewModel> _viewModel = null;
            
                ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            ViewBag.recQty = Convert.ToInt32(TempData["recQty"]);
            ViewBag.ordQty = Convert.ToInt32(TempData["ordQty"]);
            ViewBag.OpenQty = Convert.ToInt32(TempData["OpenQty"]);
            ViewBag.ReceviedAmount = Convert.ToDecimal(TempData["ReceviedAmount"]);
            ViewBag.orderamount = Convert.ToDecimal(TempData["orderamount"]);
            ViewBag.OpenAmount = Convert.ToDecimal(TempData["OpenAmount"]);
            ViewBag.POcount = Convert.ToInt32(TempData["POcount"]);
            ViewBag.count = Convert.ToInt32(TempData["count"]);
            ViewBag.shiQty = Convert.ToInt32(TempData["shiQty"]);
            ViewBag.shipedamount = Convert.ToDecimal(TempData["shipedamount"]);
          
            _viewModel = _ApiAccess.GetPOProductsList(ApiURL, token, CurrentDate, PreviousDate, POMasterID, startlimit, endLimit, POID, SKU, title, OpenItem, ReceivedItem, OrderdItem, NotShipped, ShippedButNotReceived);
            data = new StaticPagedList<PurchaseOrderItemsViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            return PartialView("~/Views/PurchaseOrder/_POProductsPartialView.cshtml", data);
        }
        public IActionResult UpdatepurchaseOrder(int POID)
        {
            PurchaseOrderViewModel.PurchaseOrderData purchaseOrderData = new PurchaseOrderViewModel.PurchaseOrderData();
            PurchaseOrderDataViewModel purchaseOrderDataViewModel = new PurchaseOrderDataViewModel();
            List<PurchaseOrderItemsDataViewModel> purchaseOrderItemsDataViewModel = new List<PurchaseOrderItemsDataViewModel>();
            token = Request.Cookies["Token"];

            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);


            purchaseOrderData = _ApiAccess.GetPurchaseOrderByIdFromSellerCloud(authenticate.access_token, SCRestURL, POID.ToString(), ApiURL, token);
            if (purchaseOrderData.Purchase == null)
            {
                return RedirectToAction("PurchaseOrders", "PurchaseOrder", new { POId = POID });
            }
            purchaseOrderDataViewModel.CompanyId = purchaseOrderData.Purchase.CompanyId;
            purchaseOrderDataViewModel.CurrencyCode = purchaseOrderData.Purchase.CurrencyCode;
            purchaseOrderDataViewModel.OrderedOn = purchaseOrderData.Purchase.OrderedOn;
            purchaseOrderDataViewModel.POId = purchaseOrderData.Purchase.POId;
            purchaseOrderDataViewModel.DefaultWarehouseID = purchaseOrderData.Purchase.DefaultWarehouseID;
            purchaseOrderDataViewModel.VendorId = purchaseOrderData.Purchase.VendorId;
            purchaseOrderDataViewModel.POStatus = 1;
            foreach (var item in purchaseOrderData.Items)
            {
                PurchaseOrderItemsDataViewModel ItemsDataViewModel = new PurchaseOrderItemsDataViewModel();
                ItemsDataViewModel.ID = item.ID;
                ItemsDataViewModel.ProductID = item.ProductID;
                ItemsDataViewModel.PurchaseID = item.PurchaseID;
                ItemsDataViewModel.QtyOnHand = item.QtyOnHand;
                ItemsDataViewModel.QtyOrdered = item.QtyOrdered;
                ItemsDataViewModel.QtyReceived = item.QtyReceived;
                ItemsDataViewModel.UnitPrice = item.UnitPrice;
                ItemsDataViewModel.SkuStatus = 1;
                ItemsDataViewModel.ProductTitle = item.ProductName;
                purchaseOrderItemsDataViewModel.Add(ItemsDataViewModel);
            }
            purchaseOrderDataViewModel.items = purchaseOrderItemsDataViewModel;
            bool status = _ApiAccess.UpdatepurchaseOrder(token, ApiURL, purchaseOrderDataViewModel);


            return RedirectToAction("PurchaseOrdersItems", "PurchaseOrder", new { POId = POID });
        }

        [HttpPost]
        public JsonResult GetProductDetailsForAPBySku(string Prefix, int POID)
        {
            token = Request.Cookies["Token"];
            var model = _ApiAccess.GetAllSKUByNameForPO(ApiURL, token, Prefix, POID);
            if (model == null)
            {
                return Json(model);
            }
            else
            {
                return Json(model);
            }

        }

        [HttpPost]
        public JsonResult GetAllSKUForAutoComplete(string Prefix)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);

            token = Request.Cookies["Token"];
            var model = _ApiAccess.GetAllSKUByName(ApiURL, token, Prefix, POMasterID);
            if (model == null)
            {
                return Json(model);
            }
            else
            {
                return Json(model);
            }
        }

        [HttpGet]
        public JsonResult GetPOIdBySku(string Prefix)
        {
            token = Request.Cookies["Token"];
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);

            var list = _ApiAccess.GetPOIdBySku(ApiURL, token, Prefix, POMasterID);
            if (list == null)
            {
                return Json(list);
            }
            else
            {
                return Json(list);
            }
        }


        public bool DeleteItem(int ID)
        {

            token = Request.Cookies["Token"];

            bool status = _ApiAccess.DeleteItem(token, ApiURL, ID);


            return status;
        }

        public FileStreamModel DownloadSKULabel(string SKU, string POID)
        {
            System.Drawing.Font font = new System.Drawing.Font("HELVETICA", 14f);

            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = false,
                Alignment = AlignmentPositions.CENTER,
                //Width = _Width,
                //Height = _Height,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
                LabelFont = font
            };

            string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";
            System.Drawing.Image img = barcode.Encode(TYPE.CODE128, SKU);
            System.Drawing.Image image = img; //Here I passed a bitmap
            string dateTime = DateTime.Today.ToString("yyyy-MM-dd");
            var pgSize = new iTextSharp.text.Rectangle(114, 86);
            Document doc = new Document(pgSize);

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Path.GetTempPath() + filename, FileMode.Create));
            doc.Open();
            PdfContentByte pdfContentByte = writer.DirectContent;
            doc.SetPageSize(pgSize);

            iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(image,
            System.Drawing.Imaging.ImageFormat.Jpeg);

            BaseFont heading_font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            pdfContentByte.BeginText();

            pdfContentByte.SetFontAndSize(heading_font, 6f);
            pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, dateTime, pgSize.Right - 30, pgSize.GetTop(Utilities.MillimetersToPoints(8)), 0);

            pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "PO# " + POID, pgSize.Left + 30, pgSize.GetTop(Utilities.MillimetersToPoints(8)), 0);

            pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, SKU, (pgSize.Left + pgSize.Right) / 2, pgSize.GetTop(Utilities.MillimetersToPoints(22)), 0);
            pdfContentByte.EndText();
            pdfContentByte.Stroke();


            pdfImage.SetAbsolutePosition(5, 30);

            pdfImage.ScaleAbsolute(105, 30);
            pdfContentByte.AddImage(pdfImage);
            //doc.Add(pdfImage);
            doc.Close();
            FileStreamModel _fileStream = new FileStreamModel();
            //using (FileStream _file = new FileStream(Path.GetTempPath() + filename, FileMode.Open))
            //{
            //    var memoryStream = new MemoryStream();
            //    _file.CopyTo(memoryStream);
            //    var dataBytes = memoryStream.ToArray();
            byte[] dataBytes = System.IO.File.ReadAllBytes(Path.GetTempPath() + filename);

            _fileStream.Content = dataBytes;
            _fileStream.ContentLength = dataBytes.Length;
            _fileStream.ContentType = "application/octet-stream";
            //}

            if ((System.IO.File.Exists(Path.GetTempPath() + filename))) { System.IO.File.Delete(Path.GetTempPath() + filename); }
            return _fileStream;

        }

        public bool UpdateTitle(string Sku, string Title)
        {

            bool status = false;
            token = Request.Cookies["Token"];


            status = _ApiAccess.UpdateTitle(ApiURL, token, Sku, Title);
            return status;
        }

        public bool UpdatePODescription(string ProductPONotes, string PO)
        {

            bool status = false;
            token = Request.Cookies["Token"];


            status = _ApiAccess.UpdatePODescription(ApiURL, token, ProductPONotes, PO);
            return status;
        }
    }
}
