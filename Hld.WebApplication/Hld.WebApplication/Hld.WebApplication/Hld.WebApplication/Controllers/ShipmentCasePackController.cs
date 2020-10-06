using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Auth.AccessControlPolicy;
using Amazon.S3;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ShipmentCasePackController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        //private static string _Ds_Qty_CommentsSubdirectory = "Ds_Qty_Comments";
        //private static string _ImportMissingSkuSubdirectory = "MissingSKUFromSellerCloud";
        GetChannelCredViewModel _Selller = null;
        EncDecChannel _encDecChannel = null;
        ShipmentProductApiAccess shipmentProductApi = null;
        string ApiURL = "";
        string token = "";
        int POMasterID = 0;
        string SCRestURL = "";
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        OrderNotesAPiAccess _OrderApiAccess = null;
        ShipmentCasePackApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        UploadFilesToS3ForJobsAPIAccess ApiAccess = null;
        private readonly IConfiguration _configuration;
        public ShipmentCasePackController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ShipmentController> _logger)
        {
            _hostingEnvironment = environment;
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ApiAccess = new UploadFilesToS3ForJobsAPIAccess();
            this.logger = _logger;
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            shipmentProductApi = new ShipmentProductApiAccess();
            _encDecChannel = new EncDecChannel();
            _OrderApiAccess = new OrderNotesAPiAccess();
            _ApiAccess = new ShipmentCasePackApiAccess();
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        public IActionResult Index(string ShipmentId)
        {
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            ShipmentCasePackHeaderViewModel viewModel = new ShipmentCasePackHeaderViewModel();

            var list = _ApiAccess.GetShipmentCasePackProducts(ApiURL, token, ShipmentId);
            viewModel = _ApiAccess.GetShipmentCasePackProductHeader(ApiURL, token, ShipmentId);
            viewModel.ShipmentId = ShipmentId;
            viewModel.VendorId = POMasterID;
            var list4 = _ApiAccess.GetShipmentViewCasePackHeader(ApiURL, token, ShipmentId);
            viewModel.list = list;

            return View(viewModel);
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        public int Save(ShipmentCasePackProductViewModel data)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            data.VendorId = POMasterID;
            int Id = 0;
            if (data.idShipmentProducts == 0)
            {
                Id = _ApiAccess.Create(ApiURL, token, data);
            }
            else
            {
                Id = _ApiAccess.Update(ApiURL, token, data);
            }
            return Id;
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        public CasePackViewModel GetTemplate(string SKU)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            var Item = _ApiAccess.GetTemplate(ApiURL, token, POMasterID, SKU);
            return Item;
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        public IActionResult SaveTemplate()
        {
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View();
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        [HttpPost]
        public IActionResult SaveTemplate(CasePackViewModel data)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (data.VendorId == 0)
                    {
                        POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
                        data.VendorId = POMasterID;
                    }
                    token = Request.Cookies["Token"];

                    int Id = _ApiAccess.SaveShipmentSKUCasePackTemplate(ApiURL, token, data);
                    return RedirectToAction("GetTemplateList", "ShipmentCasePack");
                }
                catch (Exception exp)
                {
                    return View();
                }
            }
            else
                return View(data);

        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        public IActionResult GetTemplateList(CasePackSearchViewModel viewModel)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            if (viewModel.VendorId == 0)
            {
                viewModel.VendorId = POMasterID;
            }
            else
            {
            }

            token = Request.Cookies["Token"];
            List<CasePackViewModel> list = new List<CasePackViewModel>();
            var count = _ApiAccess.GetTemplateCounter(ApiURL, token, viewModel.VendorId, viewModel.SKU, viewModel.Title);
            ViewBag.TotalCount = count;
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View(viewModel);
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        [HttpPost]
        public IActionResult GetTemplateListPartial(int? page, int VendorId = 0, string SKU = "", string Title = "")
        {
            token = Request.Cookies["Token"];
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<CasePackViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int offset = 0;
            int startlimit = pageSize;

            if (page.HasValue)
            {
                offset = (pageNumber - 1) * pageSize;
            }

            if (VendorId != 0)
            {
                POMasterID = VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }

            List<CasePackViewModel> _viewModel = null;
            _viewModel = _ApiAccess.GetTemplateList(ApiURL, token, POMasterID, SKU, Title, startlimit, offset);
            data = new StaticPagedList<CasePackViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/ShipmentCasePack/TemplatelistPartialView.cshtml", data);
        }
        [Authorize(Policy = "Access to Create & Edit Shipment")]
        [HttpPost]
        public int Delete(int Id)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            var Item = _ApiAccess.Delete(ApiURL, token, Id);
            return Item;
        }

        [Authorize(Policy = "Access to Create & Edit Shipment")]
        [HttpPost]
        public int DeleteCasePack(int Id)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            var Item = _ApiAccess.DeleteCasePack(ApiURL, token, Id);
            return Item;
        }

        public IActionResult ViewShipment(string ShipmentId)
        {
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);

            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            SearchViewModelShipmentCasePack viewModel = new SearchViewModelShipmentCasePack();
            //var list = _ApiAccess.GetShipmentCasePackProducts(ApiURL, token, ShipmentId);
            viewModel.Item = _ApiAccess.GetShipmentViewCasePackHeader(ApiURL, token, ShipmentId);
            viewModel.ShipmentId = ShipmentId;
            var list = _ApiAccess.GetShipmentViewProductCasPackList(ApiURL, token, ShipmentId);
            viewModel.list = list;
            foreach (var item in viewModel.list)
            {
                if ((string.IsNullOrEmpty(item.LocationNotes) || item.LocationNotes == "undefined") && item.QtyPerCase == 0)
                {
                    ShipmentProductInventroyViewModel Obj = shipmentProductApi.GetSCInventoryBySKU(SCRestURL, authenticate.access_token, item.SKU);
                    item.LocationNotes = Obj.General.LocationNotes;
                    item.ShadowOf = Obj.General.ShadowOf;
                    item.QtyPerCase = Obj.Inventory.QtyPerCase;
                    item.PhysicalInventory = Obj.Inventory.PhysicalInventory;

                    ShipmentProductListViewModel Obj1 = new ShipmentProductListViewModel();
                    Obj1.SKU = item.SKU;
                    Obj1.LocationNotes = Obj.General.LocationNotes;
                    Obj1.ShadowOf = Obj.General.ShadowOf;
                    Obj1.QtyPerCase = Obj.Inventory.QtyPerCase;
                    Obj1.PhysicalInventory = Obj.Inventory.PhysicalInventory;
                    shipmentProductApi.UpdateShipmentProductInventory(ApiURL, token, Obj1);
                }
            }


            return View(viewModel);
        }

        [HttpPost]
        public string UpdateReceivedQty(List<ShipmentProductListViewModel> data)
        {
            string Message = "";
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            string BoxId = "";

            foreach (var item in data)
            {
                int Id = 0;
                ShipmentViewModel shipmentViewModel = new ShipmentViewModel();
                shipmentViewModel.ShipmentId = item.ShipmentId;
                shipmentViewModel.Status = 4;
                Id = shipmentProductApi.GetPOIID(ApiURL, token, item.idShipmentProducts);

                List<Items> list = new List<Items>();
                var Obj = new Items()
                {
                    ID = Id,
                    QtyToReceive = item.ReceivedQty,
                };
                BoxId = item.BoxId;
                list.Add(Obj);
                POItemRecivedViewModel Item = new POItemRecivedViewModel()
                {
                    VendorOrderId = "",
                    ReceiveInvoiceNumber = item.ShipmentId,
                    Items = list
                };


                Message = shipmentProductApi.SellerCloudUpdateReceivedQty(SCRestURL, authenticate.access_token, Item, item.POId);
                if (Message == "Ok")
                {
                    shipmentProductApi.UpdateShipmentStatus(ApiURL, token, shipmentViewModel);
                    Id = shipmentProductApi.UpdateReceivedQty(ApiURL, token, item);
                }
            }
            return Message;
        }
    }
}