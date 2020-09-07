using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ShipmentProductController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string SCRestURL = "";
        int POMasterID = 0;
        ShipmentProductApiAccess _ApiAccess = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        OrderNotesAPiAccess _OrderApiAccess = null;
        ShipmentBoxApiAccess _BoxApiAccess;
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        public ShipmentProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _ApiAccess = new ShipmentProductApiAccess();
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            _encDecChannel = new EncDecChannel();
            _OrderApiAccess = new OrderNotesAPiAccess();
            _BoxApiAccess = new ShipmentBoxApiAccess();
        }

        public IActionResult Index(string BoxId)
        {
            token = Request.Cookies["Token"];
            var item = _ApiAccess.GetHeaderWithList(ApiURL, token, BoxId);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;

            return View(item);
        }

        [HttpPost]
        public int CreateBox(ShipmentProductViewModel data)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            //data.ShipmentId = "12";
            //data.BoxId = "15";
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

        [HttpPut]
        public int Update(ShipmentProductViewModel data)
        {
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            //data.ShipmentId = "12";
            //data.BoxId = "15";
            data.VendorId = POMasterID;
            int Id = _ApiAccess.Create(ApiURL, token, data);
            return Id;
        }

        public int Delete(List<ShipmentProductViewModel> data)
        {
            token = Request.Cookies["Token"];
            foreach (var item in data)
            {
                bool status = _ApiAccess.Delete(ApiURL, token, item.idShipmentProducts);
            }
            return 0;
        }


        [HttpGet]
        public IActionResult ShipementProductByBarcode(string BoxId)
        {
            ShipementProductSearchViewModel viewModel = new ShipementProductSearchViewModel();
            token = Request.Cookies["Token"];
            viewModel.BoxId = BoxId; /*"200520-Rizwan Sohail-1-1";*/
            var count = 0;
            if (BoxId != null)
            {
                count = _ApiAccess.GetCounterByBarcode(ApiURL, token, viewModel.BoxId);
            }
            viewModel.model = _BoxApiAccess.GetBoxDeatilById(ApiURL, token, viewModel.BoxId);
            ViewBag.TotalCount = count;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ShipementProductByBarcodeDumy(string BoxId, string ShipmentId)
        {
            ShipementProductSearchViewModel viewModel = new ShipementProductSearchViewModel();
            token = Request.Cookies["Token"];
            viewModel.BoxId = BoxId; /*"200520-Rizwan Sohail-1-1";*/
            ShipmentBoxDetailViewModel model = new ShipmentBoxDetailViewModel();
            model.ShipmentId = ShipmentId;
            viewModel.model = model;
            var count = 0;
            if (BoxId != null)
            {
                count = _ApiAccess.GetCounterByBarcode(ApiURL, token, viewModel.BoxId);
                viewModel.model = _BoxApiAccess.GetBoxDeatilById(ApiURL, token, viewModel.BoxId);
            }
            ViewBag.TotalCount = count;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ShipementProductByBarcodeList(int? page, string BoxId)
        {
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);

            if (page.HasValue)
                ViewBag.pageNumber = page;
            IPagedList<ShipmentProductListViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
                endLimit = (pageNumber - 1) * pageSize;
            List<ShipmentProductListViewModel> _viewModel = null;
            _viewModel = _ApiAccess.GetListByBarcode(ApiURL, token, BoxId, startlimit, endLimit);
            foreach (var item in _viewModel)
            {
                if ((string.IsNullOrEmpty(item.LocationNotes) || item.LocationNotes == "undefined") && item.QtyPerCase == 0)
                {
                    ShipmentProductInventroyViewModel Obj = _ApiAccess.GetSCInventoryBySKU(SCRestURL, authenticate.access_token, item.SKU);
                    item.LocationNotes = Obj.General.LocationNotes;
                    item.ShadowOf = Obj.General.ShadowOf;
                    item.QtyPerCase = Obj.Inventory.QtyPerCase;
                    item.PhysicalInventory = Obj.Inventory.PhysicalInventory;
                    _ApiAccess.UpdateShipmentProductInventory(ApiURL, token, item);
                }
            }

            data = new StaticPagedList<ShipmentProductListViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/ShipmentProduct/ShipmentProductPartialView.cshtml", data);
        }


        public int UpdateShipmentStatus(ShipmentViewModel data)
        {
            token = Request.Cookies["Token"];
            data.Status = 5;
            _ApiAccess.UpdateShipmentStatus(ApiURL, token, data);
            return 0;
        }

        public int ShipmentasReceived(ShipmentViewModel data)
        {
            token = Request.Cookies["Token"];
            data.Status = 5;
            _ApiAccess.SetShipmentasReceived(ApiURL, token, data);
            return 0;
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
                Id = _ApiAccess.GetPOIID(ApiURL, token, item.idShipmentProducts);

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
                    ReceiveInvoiceNumber = BoxId,
                    Items = list
                };

                Message = _ApiAccess.SellerCloudUpdateReceivedQty(SCRestURL, authenticate.access_token, Item, item.POId);
                if (Message == "Ok")
                {
                    _ApiAccess.UpdateShipmentStatus(ApiURL, token, shipmentViewModel);
                    Id = _ApiAccess.UpdateReceivedQty(ApiURL, token, item);
                }
            }
            return Message;
        }

        public List<ShipmentBoxListViewModel> Boxes(string ShipmentId)
        {
            ShipmentBoxApiAccess shipmentBoxApiAccess = new ShipmentBoxApiAccess();
            token = Request.Cookies["Token"];
            ShipmentHeaderViewModel viewModel = new ShipmentHeaderViewModel();
            viewModel = shipmentBoxApiAccess.GetListbyShipemntId(ApiURL, token, ShipmentId);
            List<ShipmentBoxListViewModel> list = viewModel.List;
            return list;
        }





    }
}