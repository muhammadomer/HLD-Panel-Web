using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]

    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        SellerCloudApiAccess sellerCloudApiAccess = null;
        BrandApiAccess brandApiAccess = null;
        ColorApiAccess colorApiAccess = null;
        ConditionApiAccess conditionApiAccess = null;
        CategoryApiAccess categoryApiAccess = null;
        ProductApiAccess ProductApiAccess = null;
        UploadFilesToS3 uploadFiles = null;
        ProductWarehouseQtyApiAccess productWarehouseQtyApiAccess = null;
        CurrencyExchangeApiAccess currencyExchangeApiAccess = null;
        string s3BucketURL = "";
        string s3BucketURL_large = "";

        public static IHostingEnvironment _environment;
        public ProductController(IConfiguration configuration, IHostingEnvironment environment)
        {

            _environment = environment;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            brandApiAccess = new BrandApiAccess();
            colorApiAccess = new ColorApiAccess();
            conditionApiAccess = new ConditionApiAccess();
            categoryApiAccess = new CategoryApiAccess();
            ProductApiAccess = new ProductApiAccess();
            sellerCloudApiAccess = new SellerCloudApiAccess();
            uploadFiles = new UploadFilesToS3(_environment, _configuration);
            currencyExchangeApiAccess = new CurrencyExchangeApiAccess();
            productWarehouseQtyApiAccess = new ProductWarehouseQtyApiAccess();
        }
        [Authorize(Policy = "Access to Inventory")]
        public IActionResult IndexMainView(ProductInventorySearchViewModel model, string input)
        {
            if (!string.IsNullOrEmpty(model.DSTag) && model.DSTag != "undefined")
            {
                string[] Val = model.DSTag.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        model.DSTag = "Yes";
                    }
                    if (item == "No")
                    {
                        model.DSTag = "No";
                    }
                }
            }
            else
            {
                model.DSTag = "ALL";
            }

            if (!string.IsNullOrEmpty(model.SearchFromSkuList))
            {
                var lines = model.SearchFromSkuList.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string newLines = string.Join(",", lines);
                model.SearchFromSkuList = getString(newLines);
                string skuList = JsonConvert.SerializeObject(model.SearchFromSkuList);
                HttpContext.Session.SetString("skuList", skuList);
                HttpContext.Session.SetString("dropshipsearch", model.dropshipstatusSearch);
                ViewBag.skuSearchList = "searchList";
            }
            else
            {
                model.SearchFromSkuList = "Nill";
                ViewBag.skuSearchList = "";
            }
            //start
            if (string.IsNullOrEmpty(model.dropshipstatusSearch))
            {
                model.dropshipstatusSearch = "ALL";
            }
            if (string.IsNullOrEmpty(model.Sku))
            {
                model.Sku = "Nill";
            }
            if (string.IsNullOrEmpty(model.asin))
            {
                model.asin = "Nill";
            }
            if (string.IsNullOrEmpty(model.Producttitle))
            {
                model.Producttitle = "Nill";
            }

            if (model.dropshipstatusSearch == "asin")
            {
                model.asin = model.Sku;
                model.Sku = "Nill";
            }

            if (model.dropshipstatusSearch == "ProductTitle")
            {
                model.Producttitle = model.Sku;
                model.Sku = "Nill";
            }
            if (model.dropshipstatusSearch == "asin" || model.dropshipstatusSearch == "sku" || model.dropshipstatusSearch == "ProductTitle")
            {
                model.dropshipstatusSearch = "All";
            }

            //end
            if (string.IsNullOrEmpty(model.dropshipstatus))
            {
                model.dropshipstatus = "ALL";
            }
            if (string.IsNullOrEmpty(model.Sku))
            {
                model.Sku = "Nill";
            }
            if (string.IsNullOrEmpty(model.asin))
            {
                model.asin = "Nill";
            }
            if (string.IsNullOrEmpty(model.Producttitle))
            {
                model.Producttitle = "Nill";
            }

            if (model.dropshipstatus == "asin")
            {
                model.asin = model.Sku;
                model.Sku = "Nill";
            }

            if (model.dropshipstatus == "ProductTitle")
            {
                model.Producttitle = model.Sku;
                model.Sku = "Nill";
            }
            if (model.dropshipstatus == "asin" || model.dropshipstatus == "sku" || model.dropshipstatus == "ProductTitle")
            {
                model.dropshipstatus = "All";
            }

            token = Request.Cookies["Token"];
            ViewBag.TotalCount = ProductApiAccess.GetAllProductsCount(ApiURL, token, model);
            ViewBag.Sku = model.Sku == null || model.Sku == "undefined" ? "Nill" : model.Sku.Trim();
            ViewBag.asin = model.asin == null || model.asin == "undefined" ? "Nill" : model.asin.Trim();
            if (model.Sku == "Nill")
            {
                model.Sku = "";
            }
            return View(model);
        }


        public IActionResult Index(int? page, string sort, string dropshipstatus, string dropshipstatusSearch, string Sku, string asin, string Producttitle, string DSTag, string TypeSearch)
        {
            //getting skulist from session data
            token = Request.Cookies["Token"];
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            var key = HttpContext.Session.GetString("skuList");
            var search = HttpContext.Session.GetString("dropshipsearch");





            if (string.IsNullOrEmpty(key) && (string.IsNullOrEmpty(dropshipstatusSearch) || dropshipstatusSearch == "undefined") && (string.IsNullOrEmpty(dropshipstatus) || dropshipstatus == "undefined") && (string.IsNullOrEmpty(Sku) || Sku == "undefined") && (string.IsNullOrEmpty(asin) || asin == "undefined") && string.IsNullOrEmpty(Producttitle))

            {

                List<ProductDisplayInventoryViewModel> viewModels = new List<ProductDisplayInventoryViewModel>();

                return PartialView("~/Views/Product/_Index.cshtml", viewModels);

            }

            if (!string.IsNullOrEmpty(key))
            {
                string skuList = JsonConvert.DeserializeObject<string>(key);
                ProductInventorySearchViewModel viewmodel = new ProductInventorySearchViewModel();
                viewmodel.dropshipstatus = "ALL";
                viewmodel.dropshipstatusSearch = search;
                viewmodel.Sku = "Nill";
                viewmodel.SearchFromSkuList = skuList;
                viewmodel.DSTag = "ALL";
                ViewBag.asin = "Nill";
                ViewBag.Producttitle = "Nill";
                List<ProductDisplayInventoryViewModel> productList = ProductApiAccess.GetAllProductsWihtoutPageLimit(ApiURL, token, viewmodel);
                HttpContext.Session.Remove("skuList");
                ViewBag.skuSearchList = "searchList";
                return PartialView("~/Views/Product/_Index.cshtml", productList);
            }
            else
            {
                if (!string.IsNullOrEmpty(DSTag) && DSTag != "undefined")
                {
                    string[] Val = DSTag.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "Yes")
                        {
                            DSTag = "Yes";
                        }
                        if (item == "No")
                        {
                            DSTag = "No";
                        }
                    }
                } 
                else
                {
                    DSTag = "ALL";
                }
                ViewBag.skuSearchList = "";
                //start
                if (dropshipstatusSearch == null || dropshipstatusSearch == "undefined")
                {
                    dropshipstatusSearch = "ALL";
                }
                if (Sku == null || Sku == "undefined")
                {
                    Sku = "Nill";
                }
                if (asin == null || asin == "undefined")
                {
                    asin = "Nill";
                }
                if (dropshipstatusSearch == "asin")
                {
                    asin = Sku;
                    Sku = "Nill";
                }
                if (Producttitle == null || Producttitle == "undefined")
                {
                    Producttitle = "Nill";
                }
                if (dropshipstatusSearch == "ProductTitle")
                {
                    Producttitle = Sku;
                    Sku = "Nill";
                }
                if (dropshipstatusSearch == "asin" || dropshipstatusSearch == "sku" || dropshipstatusSearch == "ProductTitle")
                {
                    dropshipstatusSearch = "All";
                }


                //end
                if (dropshipstatus == null || dropshipstatus == "undefined")
                {
                    dropshipstatus = "ALL";
                }
                if (Sku == null || Sku == "undefined")
                {
                    Sku = "Nill";
                }
                if (asin == null || asin == "undefined")
                {
                    asin = "Nill";
                }
                if (dropshipstatus == "asin")
                {
                    asin = Sku;
                    Sku = "Nill";
                }
                if (Producttitle == null || Producttitle == "undefined")
                {
                    Producttitle = "Nill";
                }
                if (dropshipstatus == "ProductTitle")
                {
                    Producttitle = Sku;
                    Sku = "Nill";
                }
                if (dropshipstatus == "asin" || dropshipstatus == "sku" || dropshipstatus == "ProductTitle")
                {
                    dropshipstatus = "All";
                }
                if (string.IsNullOrEmpty(sort))
                {
                    sort = "desc";
                }
                int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                int pageSize = 30;
                int endLimit = 0;
                int startlimit = pageSize;
                if (page.HasValue)
                {
                    endLimit = (pageNumber - 1) * pageSize;
                    ViewBag.pageNumber = page.Value;
                }
                List<ProductDisplayInventoryViewModel> viewModels = ProductApiAccess.GetAllProducts(ApiURL, token, startlimit, endLimit, sort, dropshipstatus, dropshipstatusSearch, Sku.Trim(), asin, Producttitle, DSTag, TypeSearch);
                return PartialView("~/Views/Product/_Index.cshtml", viewModels);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Index(string dropshipstatus, string dropshipstatusSearch, string Sku, string DSTag)
        {

            //getting skulist from session data
            bool status = false;
            token = Request.Cookies["Token"];
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            var key = HttpContext.Session.GetString("skuList");

            string folder = _environment.WebRootPath;
            string excelName = $"ProductData.xlsx";
            string downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
            FileInfo file = new FileInfo(Path.GetTempPath() + excelName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.GetTempPath() + excelName);
            }

            if (!string.IsNullOrEmpty(key))
            {
                string skuList = JsonConvert.DeserializeObject<string>(key);

                ProductInventorySearchViewModel viewmodel = new ProductInventorySearchViewModel();
                viewmodel.dropshipstatus = "ALL";
                viewmodel.dropshipstatusSearch = "ALL";
                viewmodel.Sku = "Nill";
                viewmodel.SearchFromSkuList = skuList;

                List<ProductDisplayInventoryViewModel> productList = ProductApiAccess.GetAllProductsWihtoutPageLimit(ApiURL, token, viewmodel);
                HttpContext.Session.Remove("skuList");
                ViewBag.skuSearchList = "searchList";

            }

            else
            {
                ViewBag.skuSearchList = "";
                if (dropshipstatus == null || dropshipstatus == "undefined")
                {
                    dropshipstatus = "ALL";
                }
                if (dropshipstatusSearch == null || dropshipstatusSearch == "undefined")
                {
                    dropshipstatusSearch = "ALL";
                }
                if (Sku == null || Sku == "undefined")
                {
                    Sku = "Nill";
                }

                List<ExportProductDataViewModel> viewModels = ProductApiAccess.GetAllProductsForExport(ApiURL, token, dropshipstatus, dropshipstatusSearch, Sku.Trim());
                await Task.Yield();
                using (var package = new ExcelPackage(file))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(viewModels, true);
                    package.Save();
                }


            }
            return Json(new { filePath = Path.GetTempPath(), fileName = excelName });
            // return downloadFile(Path.GetTempPath(), excelName, excelName); 
        }
        public FileResult downloadFile(string filePath, string fileName, string file)
        {

            IFileProvider provider = new PhysicalFileProvider(filePath);
            IFileInfo fileInfo = provider.GetFileInfo(fileName);
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, file);
        }
        [HttpPost]
        public IActionResult DeletProduct(string ProductSku)
        {
            string _message = "";
            bool _status = false;
            token = Request.Cookies["Token"];


            return RedirectToAction("ProductAddUpdate");

        }

        [Authorize(Policy = "Access to Add or Edit Product")]
        [HttpGet]
        public IActionResult ProductAddUpdate(string ProductSKU)
        {
            token = Request.Cookies["Token"];
            double currencyRate = currencyExchangeApiAccess.GetLatestCurrencyRate(ApiURL, token);
            ProductInsetUpdateViewModel model = ProductPageIndexMethod();
            if (!string.IsNullOrEmpty(ProductSKU))
            {

                int productID = 0;
                productID = ProductApiAccess.GetProductIDBySKU(ApiURL, token, ProductSKU);


                if (ProductSKU.Length > 0)
                {
                    ProductInsetUpdateViewModel viewModelUpdate = ProductApiAccess.GetProductDetail_ProductID(ApiURL, token, ProductSKU);

                    viewModelUpdate.Condition = model.Condition;
                    viewModelUpdate.CADPrice = Convert.ToDouble(viewModelUpdate.AvgCost) * currencyRate;
                    viewModelUpdate.CurrencyRate = currencyRate;
                    TempData["ProductId"] = productID;
                    return View(viewModelUpdate);
                }
            }
            model.CurrencyRate = currencyRate;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductAddUpdate(ProductInsetUpdateViewModel model, string action)
        {

            token = Request.Cookies["Token"];
            if (action == "Delete")
            {

                bool _status = false;
                DeleteProductViewModel deleteViewModel = new DeleteProductViewModel();
                deleteViewModel.PorductSku = model.ProductSKU;
                deleteViewModel.status = 0;
                _status = ProductApiAccess.DeleteProduct(ApiURL, token, deleteViewModel);
                if (_status == false)
                {
                    TempData["DeleteProductMessage"] = "Deleted successfully";
                }
                else
                {
                    TempData["DeleteProductMessage"] = "Can't be deleted,because it exists in SellerCloud Orders";
                }
                return RedirectToAction("ProductAddUpdate");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (model.ProductId > 0)
                    {
                        int ProductId = 0;
                        ProductId = ProductApiAccess.UpdateProduct(ApiURL, token, model);
                        if (ProductId > 0)
                        {

                            TempData["SaveProductMessage"] = "update";
                            return RedirectToAction("ProductAddUpdate");
                        }
                    }
                    else
                    {
                        model.ProductSKU = model.ProductSKU.ToUpper();
                        int ProductId = ProductApiAccess.AddProduct(ApiURL, token, model);
                        if (ProductId > 0)
                        {
                            await SaveImages(model.UploadImages, model.ProductSKU);
                            TempData["SaveProductMessage"] = "Save";
                            return RedirectToAction("ProductAddUpdate");
                        }
                        else
                        {
                            return RedirectToAction("ProductAddUpdate");
                        }
                    }
                }
            }

            //repopulate form if error is occured

            ProductInsetUpdateViewModel viewModel = ProductPageIndexMethod();

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult GetAllCategoriesForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
            List<CategoriesAutoCompleteViewModel> model = categoryApiAccess.GetListOfCategoriesForAutoComplete(Prefix, ApiURL, token);
            if (model.Count > 0)
            {
                return Json(model);
            }
            else
            {
                return Json(new CategoriesAutoCompleteViewModel());
            }

        }


        [HttpPost]
        public JsonResult GetAllBrandForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
            List<BrandViewModel> model = brandApiAccess.GetAllBrandsByName(ApiURL, token, Prefix);
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
        public JsonResult GetAlColorForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
            List<ColorAutoCompleteViewModel> model = colorApiAccess.GetAllColorsByName(ApiURL, token, Prefix);
            if (model == null)
            {
                return Json(model);
            }
            else
            {
                return Json(model);
            }

        }


        public ProductInsetUpdateViewModel ProductPageIndexMethod()
        {
            token = Request.Cookies["Token"];
            ProductInsetUpdateViewModel model = new ProductInsetUpdateViewModel();
            conditionApiAccess = new ConditionApiAccess();
            //getting condition list
            List<ConditionViewModel> ListconditionViewModels = conditionApiAccess.GetAllCondition(ApiURL, token);
            List<SelectListItem> _listConditionSelectListItems = new List<SelectListItem>();
            _listConditionSelectListItems.Insert(0, new SelectListItem() { Value = "0", Text = "Select Condition" });
            if (ListconditionViewModels.Count > 0)
            {

                if (ListconditionViewModels.Count > 0)
                {

                    foreach (var item in ListconditionViewModels)
                    {
                        SelectListItem selectListItem = new SelectListItem();

                        selectListItem.Value = item.ConditionId.ToString();
                        selectListItem.Text = item.ConditionName;
                        _listConditionSelectListItems.Add(selectListItem);
                    }
                }
            }

            model.Condition = _listConditionSelectListItems;
            return model;
        }

        [HttpGet]
        public IActionResult UploadImagesOfSku(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                token = Request.Cookies["Token"];
                List<ProductImagesViewModel> viewModel = ProductApiAccess.GetAllProductByProductID(ApiURL, token, ID);
                return PartialView("~/Views/Product/_UploadImagesOfSku.cshtml", viewModel);
            }
            else return PartialView("~/Views/Product/_UploadImagesOfSku.cshtml", new List<ProductImagesViewModel>());
        }


        [HttpPost]
        public async Task<IActionResult> UploadImagesOfSku(List<IFormFile> files)
        {
            token = Request.Cookies["Token"];
            if (files.Count > 0)
            {
                await SaveImages(files, Convert.ToString(HttpContext.Request.Form["ProductId"]));

            }
            return Json(new { message = "data is save successfully" });
        }

        [HttpGet]
        public IActionResult ShowImages(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {


                token = Request.Cookies["Token"];
                List<ProductImagesViewModel> viewModel = ProductApiAccess.GetAllProductByProductID(ApiURL, token, id);
                ViewBag.S3BucketURL = s3BucketURL;
                ViewBag.S3BucketURL_large = s3BucketURL_large;
                return PartialView("~/Views/Product/_ShowImages.cshtml", viewModel);
            }
            else return PartialView("~/Views/Product/_ShowImages.cshtml", new List<ProductImagesViewModel>());
        }

        private async Task<bool> SaveImages(List<IFormFile> files, string ProductSKU)
        {
            bool status = false;
            List<ImagesSaveToDatabaseWithURLViewMOdel> listImagesUrl = new List<ImagesSaveToDatabaseWithURLViewMOdel>();
            if (files != null)
            {
                if (files.Count > 0)
                {
                    foreach (var Imagefile in files)
                    {
                        string fileName = Imagefile.FileName + "-" + ProductSKU + Path.GetExtension(Imagefile.FileName);
                        Image img = Image.FromStream(Imagefile.OpenReadStream(), true, true);
                        using (Stream ms = new MemoryStream())
                        {
                            Imagefile.CopyTo(ms);
                            await uploadFiles.uploadToS3(ms, fileName);
                            await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);

                        }
                        ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                        databaseImagesURL.product_Sku = ProductSKU;
                        databaseImagesURL.FileName = fileName;

                        status = sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL);
                    }

                }
            }
            return status;
        }

        [HttpGet]
        public JsonResult RemoveCookie()
        {

            return Json(new { message = "ok", status = true });

        }
        [HttpPost]
        public JsonResult DeleteProductImage(int id)
        {
            token = Request.Cookies["Token"];
            bool status = ProductApiAccess.DeleteProductImage(ApiURL, token, id);
            if (status)
            {
                return Json(new { status = true, message = "Deleted Successfully" });
            }
            else
            {
                return Json(new { status = false, message = "Some Error Occured" });
            }

        }
        [HttpPost]
        public JsonResult GetAllSKUForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
            var model = ProductApiAccess.GetAllSKUByName(ApiURL, token, Prefix);
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
        public JsonResult GetProductDetailsForAPBySku(string Prefix)
        {
            token = Request.Cookies["Token"];
            var model = ProductApiAccess.GetProductDetailForAPBySKU(ApiURL, token, Prefix);
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
        public JsonResult CheckProductSKUExists(string name)
        {


            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/CheckExistsSKU/" + name + "");
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


        [HttpPost]
        public JsonResult CheckProductUPCExists(string name)
        {
            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/CheckExistsUPC/" + name + "");
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

        [HttpGet]
        public ActionResult ProductDetailView(string id)
        {
            if (id != null)
            {
                token = Request.Cookies["Token"];
                double currencyRate = currencyExchangeApiAccess.GetLatestCurrencyRate(ApiURL, token);
                ProductInsetUpdateViewModel viewModelUpdate = ProductApiAccess.GetProductDetail_ProductID(ApiURL, token, id);
                viewModelUpdate.CADPrice = Convert.ToDouble(viewModelUpdate.AvgCost) * currencyRate;
                return PartialView("~/Views/Product/_ProductDetailView.cshtml", viewModelUpdate);
            }
            return View();
        }


        [HttpGet]
        public ActionResult PoroductWarehouseQty(string sku)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                token = Request.Cookies["Token"];
          //      List<ProductWarehouseQtyViewModel> listmodel = productWarehouseQtyApiAccess.GetProductWarehouseQtyFromDatabase(ApiURL, token, new ProductWarehouseQtyViewModel() { ProductSku = sku });
                List<ProductWarehouseQtyViewModel> listmodel = ProductApiAccess.GetWareHousesQtyList(ApiURL, token, sku);
                ViewBag.ProductSku = sku;
                return PartialView("~/Views/Product/_PoroductWarehouseQty.cshtml", listmodel);
            }
            return View();
        }

        [HttpPost]
        public IActionResult ShowBulkSKUStatusFor_DropshipEnableDisable_Inventory(List<string> skuList)
        {
            var skuListJson = JsonConvert.SerializeObject(skuList);
            HttpContext.Session.SetString("selectedSkuListForEnableDisableSKU", skuListJson);
            SKUEnableDisableForInventoryPage model = new SKUEnableDisableForInventoryPage();
            model.totalSkuSelected = skuList.Count;
            return PartialView("~/Views/Product/_ShowBulkSKUStatusFor_DropshipEnableDisable_Inventory.cshtml", model);
        }

        public IActionResult SkuSelectList()
        {
            return PartialView("~/Views/Product/_skuSelectList.cshtml");
        }


        [HttpGet]
        public IActionResult SelectAllPagesWithoutPageLimit(string dropshipstatus, string dropshipstatusSearch, string Sku)
        {
            token = Request.Cookies["Token"];
            ProductInventorySearchViewModel viewmodel = new ProductInventorySearchViewModel();
            viewmodel.dropshipstatus = dropshipstatus;
            viewmodel.dropshipstatusSearch = dropshipstatusSearch;
            viewmodel.Sku = Sku;
            viewmodel.SearchFromSkuList = "Nill";
            List<ProductDisplayInventoryViewModel> viewModels = ProductApiAccess.GetAllProductsWihtoutPageLimit(ApiURL, token, viewmodel);

            List<string> skuList = viewModels.Select(e => e.ProductSKU).ToList();
            var skuListJson = JsonConvert.SerializeObject(skuList);
            HttpContext.Session.SetString("selectedSkuListForEnableDisableSKU", skuListJson);
            SKUEnableDisableForInventoryPage model = new SKUEnableDisableForInventoryPage();
            model.totalSkuSelected = skuList.Count;
            return PartialView("~/Views/Product/_ShowBulkSKUStatusFor_DropshipEnableDisable_Inventory.cshtml", model);
        }

        [HttpPost]
        public IActionResult Update_DS_EnableDisable_SKU(SKUEnableDisableForInventoryPage model)
        {
            token = Request.Cookies["Token"];
            int skuUpdate = 0; int skuError = 0;
            int dropshipQty = model.DropshipQty;
            try
            {

                var Sessiondata = HttpContext.Session.GetString("selectedSkuListForEnableDisableSKU");
                List<string> skulist = JsonConvert.DeserializeObject<List<string>>(Sessiondata);

                foreach (var sku in skulist)
                {
                    try
                    {
                        BBProductViewModel viewModel = new BBProductViewModel();
                        int productID = ProductApiAccess.GetProductIDBySKU(ApiURL, token, sku.Trim());
                        if (productID > 0)
                        {


                            viewModel.ShopSKU_OfferSKU = sku;
                            viewModel.dropship_Qty = dropshipQty;
                            viewModel.dropship_status = model.EnableOrDisableDropship;
                            viewModel.DropshipComments = model.Comments;
                            ProductApiAccess.UpdateProductDropshipAndQty(ApiURL, token, viewModel);

                            BestBuyDropShipQtyMovementViewModel qtyViewModel = new BestBuyDropShipQtyMovementViewModel();
                            qtyViewModel.ProductSku = viewModel.ShopSKU_OfferSKU;
                            qtyViewModel.DropShipQuantity = viewModel.dropship_Qty;
                            qtyViewModel.DropShipStatus = viewModel.dropship_status;
                            qtyViewModel.OrderDate = DateTime.Now;
                            productWarehouseQtyApiAccess.SaveBestBuyQtyMovementForDropship(ApiURL, token, qtyViewModel);


                            skuUpdate = skuUpdate + 1;
                        }
                    }
                    catch (Exception)
                    {
                        skuError = skuError + 1;
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                skuError = skuError + 1;
            }
            return Json(new { SkuUpdate = skuUpdate, SkuError = skuError });
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

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public Stream GetStreamOfReducedImage(Image inputPath)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                var settings = new MagickReadSettings { Format = MagickFormat.Raw };
                byte[] vs = ImageToByteArray(inputPath);
                int size = 100;
                int quality = 75;
                using (var image = new MagickImage(vs))
                {
                    image.Resize(size, size);
                    image.Strip();
                    image.Quality = quality;
                    image.Write(memoryStream, MagickFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                memoryStream = null;
            }
            finally
            {

            }
            return memoryStream;
        }
        [HttpGet]
        public List<AsinAmazonePriceViewModel> GetSkuAmazonePrice(string sku)
        {
            string token = Request.Cookies["Token"];
            List<AsinAmazonePriceViewModel> tagViewModel = new List<AsinAmazonePriceViewModel>();
            tagViewModel = ProductApiAccess.GetSkuAmazonePrice(ApiURL, token, sku);
            return tagViewModel;


        }

        [HttpPost]
        public string ProductContinueDisContinue(List<ProductContinueDisContinueViewModel> data)
        {
            string token = Request.Cookies["Token"];

            var staus = ProductApiAccess.ContinueDisContinue(ApiURL, token, data);
            return staus;


        }
        [HttpPost]
        public string ProductSetAsKitOrShadow(List<ProductSetAsKitOrShadowViewModel> data)
        {
            string token = Request.Cookies["Token"];

            var staus = ProductApiAccess.KitOrShadow(ApiURL, token, data);
            return staus;


        }
        [HttpPost]
        public int GetStausFromZinc(List<GetStatusFromZincViewModel> data)
        {
            string token = Request.Cookies["Token"];

            var ID = ProductApiAccess.GetStatusFromZinc(ApiURL, token, data);
            return ID;


        }

        [HttpGet]
        public List<ProductWarehouseQtyViewModel> GetWareHousesQtyList(string SKU)
        {
            token = Request.Cookies["Token"];
            var Response = ProductApiAccess.GetWareHousesQtyList(ApiURL, token, SKU);
            Response = Response.Where(s => s.AvailableQty != 0).ToList();
            return Response;
        }
    }
}
