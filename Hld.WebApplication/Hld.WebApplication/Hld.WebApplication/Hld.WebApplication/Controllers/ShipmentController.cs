using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ShipmentController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        //private static string _Ds_Qty_CommentsSubdirectory = "Ds_Qty_Comments";
        //private static string _ImportMissingSkuSubdirectory = "MissingSKUFromSellerCloud";

        string ApiURL = "";
        string token = "";
        int POMasterID = 0;
        string s3BucketURL = "";
        string s3BucketURL_large = "";

        ShipmentApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        UploadFilesToS3ForJobsAPIAccess ApiAccess = null;
        private readonly IConfiguration _configuration;
        public ShipmentController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ShipmentController> _logger)
        {
            _hostingEnvironment = environment;
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ApiAccess = new UploadFilesToS3ForJobsAPIAccess();
            this.logger = _logger;
            _ApiAccess = new ShipmentApiAccess();
        }

        public IActionResult Create(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string ShipmentId = "", int VendorId = 0, string Vendor = "", string TrakingNumber = "", string ItemType = "", string Type = "")
        {
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
                PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }
            string Status = "";
            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                Status += "(0";
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Open")
                    {
                        Status += ",1";
                    }
                    if (item == "InProcess")
                    {
                        Status += ",2";
                    }
                    if (item == "Shiped")
                    {
                        Status += ",3";
                    }
                    if (item == "ReceiveInProcess")
                    {
                        Status += ",4";
                    }
                    if (item == "Received")
                    {
                        Status += ",5";
                    }
                }
                Status += ")";
            }

            if (VendorId != 0)
            {
                POMasterID = VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            ShipmentViewModel viewModel = new ShipmentViewModel();
            token = Request.Cookies["Token"];
            viewModel.VendorId = Convert.ToInt32(Request.Cookies["POMasterID"]);
            var count = _ApiAccess.GetCounter(ApiURL, token, CurrentDate, PreviousDate, POMasterID, ShipmentId, TrakingNumber, Status, Type);
            viewModel.TrakingNumber = TrakingNumber;
            viewModel.ShipmentId = ShipmentId;
            viewModel.Type = Type;
            ViewBag.TotalCount = count;
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult getlist(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, int? page, string ShipmentId = "", int VendorId = 0, string TrakingNumber = "", string ItemType = "", string Type = "")
        {

            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {
                CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
                PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }
            token = Request.Cookies["Token"];
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<ShipmentGetDataViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }
            string Status = "";
            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                Status += "(0";
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Open")
                    {
                        Status += ",1";
                    }
                    if (item == "InProcess")
                    {
                        Status += ",2";
                    }
                    if (item == "Shiped")
                    {
                        Status += ",3";
                    }
                    if (item == "ReceiveInProcess")
                    {
                        Status += ",4";
                    }
                    if (item == "Received")
                    {
                        Status += ",5";
                    }
                }
                Status += ")";
            }

            if (VendorId != 0)
            {
                POMasterID = VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            List<ShipmentGetDataViewModel> _viewModel = null;
            _viewModel = _ApiAccess.GetShipmentList(ApiURL, token, CurrentDate, PreviousDate, POMasterID, startlimit, endLimit, ShipmentId, TrakingNumber, Status, Type);
            data = new StaticPagedList<ShipmentGetDataViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/Shipment/ShipmentPartialView.cshtml", data);
        }

        [HttpPost]
        public IActionResult Create(ShipmentViewModel viewModel)
        {
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            viewModel.CreatedOn = DateTime.Now;
            viewModel.VendorId = Convert.ToInt32(Request.Cookies["POMasterID"]);
            bool status = _ApiAccess.Create(ApiURL, token, viewModel);
            var count = _ApiAccess.GetCounter(ApiURL, token, CurrentDate, PreviousDate, viewModel.VendorId);
            ViewBag.TotalCount = count;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(ShipmentViewModel viewModel)
        {
            token = Request.Cookies["Token"];
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            bool status = _ApiAccess.Update(ApiURL, token, viewModel);
            viewModel.VendorId = Convert.ToInt32(Request.Cookies["POMasterID"]);
            var count = _ApiAccess.GetCounter(ApiURL, token, CurrentDate, PreviousDate, viewModel.VendorId);
            ViewBag.TotalCount = count;
            return View("Create", viewModel);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public int Delete(ShipmentViewModel data)
        {
            token = Request.Cookies["Token"];
            bool status = _ApiAccess.Delete(ApiURL, token, data.ShipmentId);
            return 0;
        }

        [HttpPost]
        public IActionResult CreateShipmentCourier(ShipmentCourierViewModel viewModel)
        {
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            viewModel.CreatedAt = DateTime.Now;
            int VendorId = Convert.ToInt32(Request.Cookies["POMasterID"]);
            bool status = _ApiAccess.CreateShipmentCourier(ApiURL, token, viewModel);
            var count = _ApiAccess.GetCounter(ApiURL, token, CurrentDate, PreviousDate, VendorId);
            ViewBag.TotalCount = count;
            ShipmentViewModel _viewModel = new ShipmentViewModel();
            return View("Create", _viewModel);
        }

        [HttpPost]
        public int DeleteShipmentCourier(ShipmentViewModel data)
        {
            token = Request.Cookies["Token"];
            bool status = _ApiAccess.DeleteShipmentCourier(ApiURL, token, data.ShipmentId);
            return 0;
        }

        public IActionResult ShipmentIndex(string ShipmentId, int POID, string SKU = "", string Title = "", string ItemType = "")
        {
            string OpenItem = "";
            string ReceivedItem = "";
            string OrderdItem = "";
            int count = 0;
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


                }
            }
            string token = Request.Cookies["Token"];
            SearchViewModelShipment viewModel = new SearchViewModelShipment();
            viewModel.SKU = SKU;
            viewModel.Title = Title;
            viewModel.POID = POID;


            count = _ApiAccess.ShipmentCount(ApiURL, token, ShipmentId, POID, SKU, Title, OpenItem, ReceivedItem, OrderdItem);
            //listmodel.List = _ApiAccess.ShipmentListData(ApiURL, token, ShipmentId);
            viewModel.Item = _ApiAccess.ShipmentViewDetail(ApiURL, token, ShipmentId);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            ViewBag.TotalCount = count;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ShipmentListData(int? page, string ShipmentId, int POID, string SKU = "", string Title = "", string ItemType = "")
        {
            string OpenItem = "";
            string ReceivedItem = "";
            string OrderdItem = "";


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
                }
            }
            token = Request.Cookies["Token"];


            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<ShipmentViewProducListViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;

            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;


            List<ShipmentViewProducListViewModel> _viewModel = null;
            _viewModel = _ApiAccess.ShipmentListData(ApiURL, token, ShipmentId, startlimit, endLimit, POID, SKU, Title, OpenItem, ReceivedItem, OrderdItem);
            List<ShipmentViewProducListViewModel> list = _viewModel.GroupBy(x => new { x.SKU, x.POId })
                .Select(p => p.FirstOrDefault())
                .Select(s => new ShipmentViewProducListViewModel
                {
                    CompressedImage = s.CompressedImage,
                    ImageName = s.ImageName,
                    SKU = s.SKU,
                    POId = s.POId,
                    CurrencyCode = s.CurrencyCode,
                    OpenQty = s.OpenQty,
                    OrderedQty = s.OrderedQty,
                    ReceivedQty = s.ReceivedQty,
                    Title = s.Title,
                    UnitPrice = s.UnitPrice,
                    UnitPriceUSD = s.UnitPriceUSD,
                }).Distinct().ToList();
            foreach (var item in list)
            {
                var Boxes = _viewModel.Where(s => s.SKU == item.SKU && s.POId == item.POId).Select(x => new Boxes
                {
                    BoxId = x.BoxId,
                    BoxNo = x.BoxNo,
                    SKUShipedQty = x.ShipedQty,
                }).ToList();
                item.ShipedQty = _viewModel.Where(s => s.SKU == item.SKU && s.POId == item.POId).Sum(s => s.ShipedQty);
                item.ReceivedQty = _viewModel.Where(s => s.SKU == item.SKU && s.POId == item.POId).Sum(s => s.ReceivedQty);

                item.Boxes = Boxes;
            }

            data = new StaticPagedList<ShipmentViewProducListViewModel>(list, pageNumber, pageSize, list.Count);
            return PartialView("~/Views/Shipment/ShipmentViewPartialViewModel.cshtml", data);
        }

        [HttpPost]
        public JsonResult GetAllSKUForAutoComplete(string Prefix, string ShipmentId)
        {
            token = Request.Cookies["Token"];
            var model = _ApiAccess.GetAllSKUByName(ApiURL, token, Prefix, ShipmentId);
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
        public List<ShipmentProductListViewModel> SKUWithShipedQty(string BoxId)
        {
            ShipmentProductApiAccess shipmentProductApiAccess = new ShipmentProductApiAccess();
            token = Request.Cookies["Token"];
            var item = shipmentProductApiAccess.GetHeaderWithList(ApiURL, token, BoxId);

            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return item.list;
        }

        public IActionResult ShipmentHistoryList(ShipmentHistorySearchViewModel viewModel)
        {
            token = Request.Cookies["Token"];
            //List<ASINDetailViewModel> list = new List<ASINDetailViewModel>();
            string DateTo = DateTime.Now.ToString("yyyy-MM-dd"); ;
            string DateFrom = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"); ;
            int count = 0;
            GetShipedAndRecQtyViewModel model = new GetShipedAndRecQtyViewModel();
            if ("0001-01-01" != viewModel.orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                DateTo = viewModel.orderDateTimeTo.ToString("yyyy-MM-dd");
                DateFrom = viewModel.orderDateTimeFrom.ToString("yyyy-MM-dd");
            }
            if (viewModel.VendorId != 0)
            {
                POMasterID = viewModel.VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            string Status = "";
            if (!string.IsNullOrEmpty(viewModel.ItemType) && viewModel.ItemType != "undefined")
            {
                Status += "(0";
                string[] Val = viewModel.ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Open")
                    {
                        Status += ",1";
                    }
                    if (item == "InProcess")
                    {
                        Status += ",2";
                    }
                    if (item == "Shiped")
                    {
                        Status += ",3";
                    }
                    if (item == "ReceiveInProcess")
                    {
                        Status += ",4";
                    }
                    if (item == "Received")
                    {
                        Status += ",5";
                    }
                }
                Status += ")";
            }
            if ((string.IsNullOrEmpty(viewModel.EmptyFirstTime) || viewModel.EmptyFirstTime == "undefined"))
            {

            }
            else
            {
                model = _ApiAccess.GetShipmentHistoryCount(ApiURL, token, DateTo, DateFrom, POMasterID, viewModel.ShipmentId, viewModel.SKU, viewModel.Title, Status);
                ViewBag.TotalCount = model.TotalCount;
                TempData["ShipQty"] = model.ShipedQty;
                TempData["RecQty"] = model.RecivedQty;
                ViewBag.showlist = ViewBag.showlist;



            }
            //  model = _ApiAccess.GetShipmentHistoryCount(ApiURL, token, DateTo, DateFrom, POMasterID, viewModel.ShipmentId, viewModel.SKU, viewModel.Title, Status);
            //ViewBag.TotalCount = model.TotalCount;
            //TempData["ShipQty"] = model.ShipedQty;
            //TempData["RecQty"] = model.RecivedQty;
            //ViewBag.S3BucketURL = s3BucketURL;
            //ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ShipmentHistoryListPartialView(int? page, DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string SKU, string Title, int VendorId, string EmptyFirstTime, string ShipmentId, string ItemType = "")
        {
            token = Request.Cookies["Token"];
            IPagedList<ShipmentHistoryViewModel> data = null;
            List<ShipmentHistoryViewModel> _viewModel = null;

            string Status = "";
            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                Status += "(0";
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Open")
                    {
                        Status += ",1";
                    }
                    if (item == "InProcess")
                    {
                        Status += ",2";
                    }
                    if (item == "Shiped")
                    {
                        Status += ",3";
                    }
                    if (item == "ReceiveInProcess")
                    {
                        Status += ",4";
                    }
                    if (item == "Received")
                    {
                        Status += ",5";
                    }
                }
                Status += ")";
            }

            string DateTo = DateTime.Now.ToString("yyyy-MM-dd"); ;
            string DateFrom = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"); ;

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                DateTo = orderDateTimeTo.ToString("yyyy-MM-dd");
                DateFrom = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }

            if (VendorId != 0)
            {
                POMasterID = VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }

            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int offset = 0;
            int limit = pageSize;

            if (page.HasValue)
            {
                offset = (pageNumber - 1) * pageSize;
            }
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                _viewModel = _ApiAccess.GetShipmentHistoryList(ApiURL, token, DateTo, DateFrom, POMasterID, ShipmentId, SKU, Title, 0, 0, Status);
                data = new StaticPagedList<ShipmentHistoryViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                return PartialView("~/Views/Shipment/ShipmentHistoryPartialView.cshtml", data);
            }
            _viewModel = _ApiAccess.GetShipmentHistoryList(ApiURL, token, DateTo, DateFrom, POMasterID, ShipmentId, SKU, Title, limit, offset, Status);


            List<ShipmentHistoryViewModel> list = _viewModel.GroupBy(x => new { x.SKU, x.ShipmentId })
                .Select(p => p.FirstOrDefault())
                .Select(s => new ShipmentHistoryViewModel
                {
                    CompressedImage = s.CompressedImage,
                    ImageName = s.ImageName,
                    SKU = s.SKU,
                    Title = s.Title,
                    Vendor = s.Vendor,
                    VendorId = s.VendorId,
                    ShipmentId = s.ShipmentId,
                    Type = s.Type,
                    Status = s.Status,
                    ReceivedDate = s.ReceivedDate,
                    ShippedDate = s.ShippedDate,
                    ShipedQty = s.ShipedQty,
                    ReceivedQty = s.ReceivedQty,
                    CreatedOn = s.CreatedOn,
                    TrakingNumber = s.TrakingNumber,
                    TrakingURL = s.TrakingURL,
                    CourierCode = s.CourierCode,
                    ExpectedDelivery = s.ExpectedDelivery,

                }).Distinct().ToList();
            foreach (var item in list)
            {
                var POs = _viewModel.Where(s => s.SKU == item.SKU && s.ShipmentId == item.ShipmentId).Select(x => new POIDs
                {
                    POId = x.POId,
                    ShipedQty = x.ShipedQty,
                }).ToList();
                item.ShipedQty = _viewModel.Where(s => s.SKU == item.SKU && s.ShipmentId == item.ShipmentId).Sum(s => s.ShipedQty);
                //item.ReceivedQty = _viewModel.Where(s => s.SKU == item.SKU && s.POId == item.POId).Sum(s => s.ReceivedQty);
                item.POs = POs;
            }
            data = new StaticPagedList<ShipmentHistoryViewModel>(list, pageNumber, pageSize, list.Count);
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            ViewBag.showlist = list.Count;
            return PartialView("~/Views/Shipment/ShipmentHistoryPartialView.cshtml", data);
        }

        public List<ShipmentHistoryViewModel> ShipmentDetailBySku(string SKU, int POID)
        {
            token = Request.Cookies["Token"];
            List<ShipmentHistoryViewModel> _viewModel = _ApiAccess.GetShipmentHistoryBYSKU(ApiURL, token, POID, SKU);
            return _viewModel;
        }
        public ShipmentCourierInfoViewModel GetShipmentCourierInfo(string shipmentId)
        {

            ShipmentCourierInfoViewModel _viewModel = new ShipmentCourierInfoViewModel();


            token = Request.Cookies["Token"];
            _viewModel = _ApiAccess.GetShipmentCourierInfo(ApiURL, token, shipmentId);
            return _viewModel;
        }

        [HttpPost]
        public IActionResult UpdateShipmentCourierInfo(ShipmentCourierInfoViewModel viewModel)
        {
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            bool status = _ApiAccess.UpdateShipmentCourierInfo(ApiURL, token, viewModel);
            return RedirectToAction("Create", "Shipment");
        }
       
        [HttpGet]
        public ActionResult AddDate(string id)
        {
            Expected_Delivery_Shipped_POViewModel ViewModel = new Expected_Delivery_Shipped_POViewModel();
            ViewModel.ShipmentId = id;
            
            try
            {
                if (ViewModel.ExpectedDelivery!=null)
                {
                    string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                    string token = Request.Cookies["Token"];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/GetById?id=" + id);
                    request.Method = "GET";
                    request.Accept = "application/json;";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + token;

                    var response = (HttpWebResponse)request.GetResponse();
                    string strResponse = "";
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                    ViewModel = JsonConvert.DeserializeObject<Expected_Delivery_Shipped_POViewModel>(strResponse);
                    return PartialView("~/Views/Shipment/ShowDate.cshtml", ViewModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
           
            return PartialView("~/Views/Shipment/ShowDate.cshtml", ViewModel);
        }
        [HttpPost]
        public ActionResult AddDate(Expected_Delivery_Shipped_POViewModel ViewModel)
        {
            if (ViewModel.ShipmentId == "")
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Brand/save");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                return Json(new { success = true, message = "save successfully" });
            }
            else
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/UpdateExpectedDelivery");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                var Status = strResponse;
                return Json(new { success = true, message = "Update successfully" });
            }
        }
        [HttpGet]
        public ActionResult SaveBBtrackingCodes()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveBBtrackingCodes(BBtrackingCodesViewModel ViewModel)
        {
            if (ViewModel.TrackingNumberCode!=null)
            {
                try
                {
                    string token = Request.Cookies["Token"];
                    if (ViewModel.IdBBtrackingCodes > 0)
                    {
                        _ApiAccess.SaveBBtrackingCodes(ApiURL, token, ViewModel);
                    }
                    else
                    {
                        _ApiAccess.SaveBBtrackingCodes(ApiURL, token, ViewModel);

                    }
                    //return RedirectToAction("BBtrackingCodesList", "Shipment");
                    return RedirectToAction("BBtrackingRuleList", "Shipment");

                }

                catch
                {
                    return View();
                }
            }
            else
                return View(ViewModel);
        }

        public IActionResult BBtrackingCodesList()
        {
            string token = Request.Cookies["Token"];
            List<BBtrackingCodesViewModel> listmodel = new List<BBtrackingCodesViewModel>();
            listmodel = _ApiAccess.BBtrackingCodesList(ApiURL, token);
            return View(listmodel);
        }
        public IActionResult BBtrackingRulesList(int page = 0)
        {
            IPagedList<BBtrackingCodesViewModel> data = null;
            string token = Request.Cookies["Token"];
            int pageNumber = page;
            int offset = 0;
            int pageSize = 25;


            offset = (pageNumber - 1) * 25;

            List<BBtrackingCodesViewModel> listmodel = new List<BBtrackingCodesViewModel>();           
            listmodel = _ApiAccess.BBtrackingRulesList(ApiURL, token, offset);
            data = new StaticPagedList<BBtrackingCodesViewModel>(listmodel, pageNumber, pageSize, listmodel.Count);
            return PartialView("~/Views/Shipment/BBtrackingCodesList.cshtml", data);
            //return View(listmodel);
        }
        public IActionResult BBtrackingRuleList()
        {
            string token = Request.Cookies["Token"];
            int listmodel = 0;
            listmodel = _ApiAccess.GetTrackingNumberCount(ApiURL, token);
            ViewBag.Records = listmodel;
            return View();
        }


        public ActionResult Edit(int id)
        {

            string token = Request.Cookies["Token"];
            BBtrackingCodesViewModel ViewModel = new BBtrackingCodesViewModel();
            ViewModel = _ApiAccess.EditBBtrackingCodesById(ApiURL, token, id);

            return View("SaveBBtrackingCodes", ViewModel);

        }
        [HttpPost]
        public JsonResult CheckTrackingNumberExists(string name)
        {
            bool status = false;
            try
            {

                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Shipment/CheckTrackingNumberExists/" + name + "");
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = Convert.ToBoolean(JObject.Parse(strResponse)["status"].ToString());

            }
            catch (Exception)
            {
                throw;
            }
            return Json(new { success = status });
        }

    }

}