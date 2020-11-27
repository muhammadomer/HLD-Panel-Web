using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hld.WebApplication.Helper;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ProductWarehouseQtyController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string SCRestURL = "";
        SellerCloudController ctrl = null;
        public static IHostingEnvironment _environment;
        ProductWarehouseQtyApiAccess productWarehouseQtyApiAccess = null;
        ProductStatusApiAccess productStatusApiAccess = null;
        ProductApiAccess productApiAccess = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        OrderNotesAPiAccess _OrderApiAccess = null;

        public IActionResult Index()
        {
            return View();
        }

        public ProductWarehouseQtyController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            _environment = environment;
            productWarehouseQtyApiAccess = new ProductWarehouseQtyApiAccess();
            _encDecChannel = new EncDecChannel();
            productStatusApiAccess = new ProductStatusApiAccess();
            productApiAccess = new ProductApiAccess();
            ctrl = new SellerCloudController(_configuration, _environment);
            ctrl.ControllerContext = ControllerContext;
            _OrderApiAccess = new OrderNotesAPiAccess();
        }

        public IActionResult GetProductWarehouseQty(string ProductSku)
        {
            token = Request.Cookies["Token"];
            List<ProductStatusViewModel> productStatusList = productStatusApiAccess.GetProductStatusList(ApiURL, token);

            ProductWarehouseQtyViewModel model = new ProductWarehouseQtyViewModel();
            model.ProductSku = ProductSku;
            model.ProductStatus = productStatusList;
            return View(model);
        }

        public async Task<IActionResult> GetProductWarehouseData(string productSku)
        {
            token = Request.Cookies["Token"];
            List<ProductWarehouseQtyViewModel> data = await ctrl.GetProductWarehouseQuantity(productSku);
            productWarehouseQtyApiAccess.SaveProductWarehouseQty(ApiURL, token, data);
            return PartialView("~/Views/ProductWarehouseQty/GetProductWarehouseData.cshtml", data);
        }

        public IActionResult GetProductWarehouseQtyDataFromDatabase(string productSku)
        {
            token = Request.Cookies["Token"];
            ProductWarehouseQtyViewModel model = new ProductWarehouseQtyViewModel();
            model.ProductSku = productSku;
            List<ProductWarehouseQtyViewModel> data = productApiAccess.GetWareHousesQtyList(ApiURL, token, productSku);
            data = data.Where(s => s.AvailableQty != 0).ToList();
            return PartialView("~/Views/ProductWarehouseQty/GetProductWarehouseData.cshtml", data);
        }


        public IActionResult UpdateProductStatus(string sku, string id)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            status = productApiAccess.UpdateProductStatus(ApiURL, token, sku, id);
            return Json(new { Productstatus = status, message = "Update Successfully" });
        }
        [HttpGet]
        public IActionResult UpdateWarehouseQuantityFromSellerCloud(string sku)
        {
            bool status = false;
            List<ProductWarehouseQtyViewModel> QuantityList = new List<ProductWarehouseQtyViewModel>();
            try
            {
                token = Request.Cookies["Token"];
               
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
                int[] warehouseId = { 358, 359,368, 373, 360 };
                foreach (var item in warehouseId.ToList())
                {
                    ProductWarehouseQuantityViewModel Quantity = productWarehouseQtyApiAccess.GetProductWarehouseFormSC(SCRestURL, authenticate.access_token, sku, item);
                   
                    ProductWarehouseQtyViewModel productWarehouse = new ProductWarehouseQtyViewModel();
                    productWarehouse.AvailableQty = Quantity.AvailableQty;
                    productWarehouse.ProductSku = Quantity.ProductID;
                    productWarehouse.WarehouseID = Quantity.WarehouseID == 364 ? 1 : Quantity.WarehouseID == 365 ? 2 : Quantity.WarehouseID == 368 ? 3 : Quantity.WarehouseID == 359 ? 4 : Quantity.WarehouseID == 358 ? 5 : Quantity.WarehouseID == 367 ? 6 : Quantity.WarehouseID == 376 ? 7 : Quantity.WarehouseID == 378 ? 8 : Quantity.WarehouseID == 372 ? 9 : Quantity.WarehouseID == 373 ? 10 : Quantity.WarehouseID == 360 ? 11 : Quantity.WarehouseID == 369 ? 12 : Quantity.WarehouseID == 366 ? 13 : 13;

                    QuantityList.Add(productWarehouse);
                }
                status = productWarehouseQtyApiAccess.SaveProductWarehouseQty(ApiURL, token, QuantityList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //List<ProductWarehouseQtyViewModel> listmodel = productWarehouseQtyApiAccess.GetProductWarehouseQtyFromDatabase(ApiURL, token, new ProductWarehouseQtyViewModel() { ProductSku = sku });
            List<ProductWarehouseQtyViewModel> listmodel = productApiAccess.GetWareHousesQtyList(ApiURL, token, sku);
            ViewBag.ProductSku = sku;

            return PartialView("~/Views/Product/_PoroductWarehouseQty.cshtml", listmodel);
        }
        public List<ProductWarehouseQtyViewModel> UpdateWarehouseQuantityFromSellerCloudForOrderLIst(string sku)
        {
            bool status = false;
            List<ProductWarehouseQtyViewModel> QuantityList = new List<ProductWarehouseQtyViewModel>();
            try
            {
                token = Request.Cookies["Token"];

                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
                int[] warehouseId = { 358, 359,368, 373, 360 };
                foreach (var item in warehouseId.ToList())
                {
                    ProductWarehouseQuantityViewModel Quantity = productWarehouseQtyApiAccess.GetProductWarehouseFormSC(SCRestURL, authenticate.access_token, sku, item);
                    ProductWarehouseQtyViewModel productWarehouse = new ProductWarehouseQtyViewModel();
                    productWarehouse.AvailableQty = Quantity.AvailableQty;
                    productWarehouse.ProductSku = Quantity.ProductID;
                    productWarehouse.WarehouseID = Quantity.WarehouseID == 364 ? 1 : Quantity.WarehouseID == 365 ? 2 : Quantity.WarehouseID == 368 ? 3 : Quantity.WarehouseID == 359 ? 4 : Quantity.WarehouseID == 358 ? 5 : Quantity.WarehouseID == 367 ? 6 : Quantity.WarehouseID == 376 ? 7 : Quantity.WarehouseID == 378 ? 8 : Quantity.WarehouseID == 372 ? 9 : Quantity.WarehouseID == 373 ? 10 : Quantity.WarehouseID == 360 ? 11 : Quantity.WarehouseID == 369 ? 12 : Quantity.WarehouseID == 366 ? 13 : 13;

                    QuantityList.Add(productWarehouse);

                }
                status = productWarehouseQtyApiAccess.SaveProductWarehouseQty(ApiURL, token, QuantityList);


            }
            catch (Exception ex)
            {

                throw;
            }
            //List<ProductWarehouseQtyViewModel> listmodel = productWarehouseQtyApiAccess.GetProductWarehouseQtyFromDatabase(ApiURL, token, new ProductWarehouseQtyViewModel() { ProductSku = sku });

            //listmodel = productWarehouseQtyApiAccess.GetProductWarehouseQtyFromDatabaseInvent(ApiURL, token, sku);
            List<ProductWarehouseQtyViewModel> listmodel = productApiAccess.GetWareHousesQtyList(ApiURL, token, sku);
            //foreach (var item in listmodel)
            //{
            //    item.ProductSku = sku;
            //}

            return  listmodel;
        }

        public List<ProductWarehouseQtyViewModel> UpdateWarehouseQuantityFromSellerCloudForOrderLIstForSingleWH(string sku)
        {
            bool status = false;
            List<ProductWarehouseQtyViewModel> QuantityList = new List<ProductWarehouseQtyViewModel>();
            try
            {
                token = Request.Cookies["Token"];

                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
                    ProductWarehouseQuantityViewModel Quantity = productWarehouseQtyApiAccess.GetProductWarehouseFormSC(SCRestURL, authenticate.access_token, sku, 358);
                    ProductWarehouseQtyViewModel productWarehouse = new ProductWarehouseQtyViewModel();
                    productWarehouse.AvailableQty = Quantity.AvailableQty;
                    productWarehouse.ProductSku = Quantity.ProductID;
                    productWarehouse.WarehouseID = Quantity.WarehouseID == 364 ? 1 : Quantity.WarehouseID == 365 ? 2 : Quantity.WarehouseID == 368 ? 3 : Quantity.WarehouseID == 359 ? 4 : Quantity.WarehouseID == 358 ? 5 : Quantity.WarehouseID == 367 ? 6 : Quantity.WarehouseID == 376 ? 7 : Quantity.WarehouseID == 378 ? 8 : Quantity.WarehouseID == 372 ? 9 : Quantity.WarehouseID == 373 ? 10 : Quantity.WarehouseID == 360 ? 11 : Quantity.WarehouseID == 369 ? 12 : Quantity.WarehouseID == 366 ? 13 : 13;
                    QuantityList.Add(productWarehouse);
                    status = productWarehouseQtyApiAccess.SaveProductWarehouseQty(ApiURL, token, QuantityList);
            }
            catch (Exception ex)
            {
                throw;
            }
            List<ProductWarehouseQtyViewModel> listmodel = productApiAccess.GetWareHousesQtyList(ApiURL, token, sku);
            return listmodel;
        }
    }
}