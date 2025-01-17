﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Transfer;
using DataAccess.ViewModels;
using ExcelLibrary.SpreadSheet;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.Views.Product;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
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
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        string SCRestURL = "";
        SellerCloudApiAccess sellerCloudApiAccess = null;
        BrandApiAccess brandApiAccess = null;
        ColorApiAccess colorApiAccess = null;
        ConditionApiAccess conditionApiAccess = null;
        CategoryApiAccess categoryApiAccess = null;
        ProductApiAccess ProductApiAccess = null;
        UploadFilesToS3 uploadFiles = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        ProductWarehouseQtyApiAccess productWarehouseQtyApiAccess = null;
        CurrencyExchangeApiAccess currencyExchangeApiAccess = null;
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        OrderNotesAPiAccess _OrderApiAccess = null;
        public static IHostingEnvironment _environment;
        
        public ProductController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ProductController> _logger)
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
            _OrderApiAccess = new OrderNotesAPiAccess();
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            _encDecChannel = new EncDecChannel();
            this.logger = _logger;

        }
        [Authorize(Policy = "Access to Inventory")]
        public IActionResult IndexMainView(ProductInventorySearchViewModel model, string input)
        {

            if (!string.IsNullOrEmpty(model.BBPriceUpdate) && model.BBPriceUpdate != "undefined")
            {
                string[] Val = model.BBPriceUpdate.Split(',');
                int length = Val.Length;

                foreach (var item in Val)
                {
                    if (item != null)
                    {
                        model.BBPriceUpdate = item;

                    }
                }

            }
            else
            {
                model.BBPriceUpdate = "ALL";
            }
            if (!string.IsNullOrEmpty(model.DSTag) && model.DSTag != "undefined")
            {
                string[] Val = model.DSTag.Split(',');
                int length = Val.Length;

                foreach (var item in Val)
                {
                    if (item != null)
                    {
                        model.DSTag = item;

                    }
                }

            }
            else
            {
                model.DSTag = "ALL";
            }

            if (!string.IsNullOrEmpty(model.WHQStatus) && model.WHQStatus != "undefined")
            {
                string[] Val = model.WHQStatus.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "1")
                    {
                        model.WHQStatus = "1";
                    }
                    if (item == "2")
                    {
                        model.WHQStatus = "2";
                    }
                }
            }
            else
            {
                model.WHQStatus = "ALL";
            }

            if (!string.IsNullOrEmpty(model.BBProductID) && model.BBProductID != "undefined")
            {
                string[] Val = model.BBProductID.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        model.BBProductID = "Yes";
                    }
                    if (item == "No")
                    {
                        model.BBProductID = "No";
                    }
                }
            }
            else
            {
                model.BBProductID = "ALL";
            }
            if (!string.IsNullOrEmpty(model.ASINS) && model.ASINS != "undefined")
            {
                string[] Val = model.ASINS.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        model.ASINS = "Yes";
                    }
                    if (item == "No")
                    {
                        model.ASINS = "No";
                    }
                }
            }
            else
            {
                model.ASINS = "ALL";
            }

            if (!string.IsNullOrEmpty(model.ApprovedUnitPrice) && model.ApprovedUnitPrice != "undefined")
            {
                string[] Val = model.ApprovedUnitPrice.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        model.ApprovedUnitPrice = "Yes";
                    }
                    if (item == "No")
                    {
                        model.ApprovedUnitPrice = "No";
                    }
                }
            }
            else
            {
                model.ApprovedUnitPrice = "ALL";
            }

            if (!string.IsNullOrEmpty(model.ASINListingRemoved) && model.ASINListingRemoved != "undefined")
            {
                string[] Val = model.ASINListingRemoved.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        model.ASINListingRemoved = "Yes";
                    }
                    if (item == "No")
                    {
                        model.ASINListingRemoved = "No";
                    }
                }
            }
            else
            {
                model.ASINListingRemoved = "ALL";
            }
            if (!string.IsNullOrEmpty(model.BBInternalDescription) && model.BBInternalDescription != "undefined")
            {
                string[] Val = model.BBInternalDescription.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        model.BBInternalDescription = "Yes";
                    }
                    if (item == "No")
                    {
                        model.BBInternalDescription = "No";
                    }
                }
            }
            else
            {
                model.BBInternalDescription = "ALL";
            }
            if (!string.IsNullOrEmpty(model.SearchFromSkuList))
            {
                var lines = model.SearchFromSkuList.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string newLines = string.Join(",", lines);
                model.SearchFromSkuList = getString(newLines);
                string skuList = JsonConvert.SerializeObject(model.SearchFromSkuList);
                HttpContext.Session.SetString("skuList", skuList);
                HttpContext.Session.SetString("dropshipsearch", model.dropshipstatusSearch);
                HttpContext.Session.SetString("seltedtaglist", model.seltedtaglist);
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
            if (string.IsNullOrEmpty(model.seltedtaglist))
            {
                model.seltedtaglist = "ALL";
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
            if ((string.IsNullOrEmpty(model.EmptyFirstTime) || model.EmptyFirstTime == "undefined"))
            {

            }
            else
            {
                token = Request.Cookies["Token"];
                ViewBag.TotalCount = ProductApiAccess.GetAllProductsCount(ApiURL, token, model);
                ViewBag.Sku = model.Sku == null || model.Sku == "undefined" ? "Nill" : model.Sku.Trim();
                ViewBag.asin = model.asin == null || model.asin == "undefined" ? "Nill" : model.asin.Trim();
                if (model.Sku == "Nill")
                {
                    model.Sku = "";
                }
            }
           
            
            return View(model);
        }

        public int GetWatchlistLogsSelectAllCount(ProductInventorySearchViewModel model)
        {
            string token = Request.Cookies["Token"];
            
            int count = ProductApiAccess.GetAllProductsCount(ApiURL, token, model);
            return count;
        }
        public IActionResult Index(int? page, string sort, string dropshipstatus, string dropshipstatusSearch, string Sku, string asin, string Producttitle, string DSTag, string TypeSearch,string WHQStatus,string BBProductID,string ASINS,string ApprovedUnitPrice,string ASINListingRemoved,string BBPriceUpdate, string BBInternalDescription,string EmptyFirstTime="")
        {
            //getting skulist from session data
            token = Request.Cookies["Token"];
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            var key = HttpContext.Session.GetString("skuList");
            var search = HttpContext.Session.GetString("dropshipsearch");
            var seltedtaglist = HttpContext.Session.GetString("seltedtaglist");

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
                viewmodel.seltedtaglist = seltedtaglist;
                viewmodel.Sku = "Nill";
                viewmodel.SearchFromSkuList = skuList;
                viewmodel.DSTag = "ALL";
                viewmodel.WHQStatus = "ALL";
                viewmodel.BBProductID = "ALL";
                viewmodel.BBPriceUpdate = "ALL";
                viewmodel.ASINListingRemoved = "ALL";
                viewmodel.ApprovedUnitPrice = "ALL";
                viewmodel.ASINS = "ALL";
                viewmodel.BBInternalDescription = "ALL";
                ViewBag.asin = "Nill";
                ViewBag.Producttitle = "Nill";
                List<ProductDisplayInventoryViewModel> productList = ProductApiAccess.GetAllProductsWihtoutPageLimit(ApiURL, token, viewmodel);
                HttpContext.Session.Remove("skuList");
                ViewBag.skuSearchList = "searchList";
                return PartialView("~/Views/Product/_Index.cshtml", productList);
            }
            else
            {
                if (!string.IsNullOrEmpty(BBInternalDescription) && BBInternalDescription != "undefined")
                {
                    string[] Val = BBInternalDescription.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "Yes")
                        {
                           BBInternalDescription = "Yes";
                        }
                        if (item == "No")
                        {
                          BBInternalDescription = "No";
                        }
                    }
                }
                else
                {
                    BBInternalDescription = "ALL";
                }
                if (!string.IsNullOrEmpty(WHQStatus) && WHQStatus != "undefined")
                {
                    string[] Val =WHQStatus.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "1")
                        {
                            WHQStatus = "1";
                        }
                        if (item == "2")
                        {
                            WHQStatus = "2";
                        }
                    }
                }
                else
                {
                  WHQStatus = "ALL";
                }
                if (!string.IsNullOrEmpty(BBPriceUpdate) && BBPriceUpdate != "undefined")
                {
                    string[] Val = BBPriceUpdate.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item != null)
                        {
                            BBPriceUpdate = item;
                        }

                    }
                }
                else
                {
                    BBPriceUpdate = "ALL";
                }
                if (!string.IsNullOrEmpty(DSTag) &&DSTag != "undefined")
                {
                    string[] Val = DSTag.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item != null)
                        {
                            DSTag = item;
                        }

                    }
                }
                else
                {
                  DSTag = "ALL";
                }
                if (!string.IsNullOrEmpty(BBProductID) && BBProductID != "undefined")
                {
                    string[] Val = BBProductID.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "Yes")
                        {
                            BBProductID = "Yes";
                        }
                        if (item == "No")
                        {
                            BBProductID = "No";
                        }
                    }
                }
                else
                {
                    BBProductID = "ALL";
                }
                if (!string.IsNullOrEmpty(ASINS) && ASINS != "undefined")
                {
                    string[] Val = ASINS.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "Yes")
                        {
                            ASINS = "Yes";
                        }
                        if (item == "No")
                        {
                            ASINS = "No";
                        }
                    }
                }
                else
                {
                    ASINS = "ALL";
                }
                if (!string.IsNullOrEmpty(ApprovedUnitPrice) && ApprovedUnitPrice != "undefined")
                {
                    string[] Val = ApprovedUnitPrice.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "Yes")
                        {
                            ApprovedUnitPrice = "Yes";
                        }
                        if (item == "No")
                        {
                            ApprovedUnitPrice = "No";
                        }
                    }
                }
                else
                {
                    ApprovedUnitPrice = "ALL";
                }
                if (!string.IsNullOrEmpty(ASINListingRemoved) && ASINListingRemoved != "undefined")
                {
                    string[] Val = ASINListingRemoved.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "Yes")
                        {
                            ASINListingRemoved = "Yes";
                        }
                        if (item == "No")
                        {
                           ASINListingRemoved = "No";
                        }
                    }
                }
                else
                {
                    ASINListingRemoved = "ALL";
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
                if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
                { 

                }
               
                    List<ProductDisplayInventoryViewModel> viewModels = ProductApiAccess.GetAllProducts(ApiURL, token, startlimit, endLimit, sort, dropshipstatus, dropshipstatusSearch, Sku.Trim(), asin, Producttitle, DSTag, TypeSearch, WHQStatus, BBProductID, ASINS, ApprovedUnitPrice, ASINListingRemoved, BBPriceUpdate,BBInternalDescription);
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
        [Authorize(Policy = "Access to Add or Edit Product")]
       
        [HttpGet]
        public IActionResult productAddMultipleSku(string ProductSKU ="", SaveParentSkuVM saveParentSkuVM=null)
        {
            token = Request.Cookies["Token"];
            double currencyRate = currencyExchangeApiAccess.GetLatestCurrencyRate(ApiURL, token);
            ProductInsetUpdateViewModel model = ProductPageIndexMethod();
            SaveParentSkuVM viewModelUpdate = new SaveParentSkuVM();
            
            viewModelUpdate.Condition = model.Condition;
            if (!string.IsNullOrEmpty(ProductSKU))
            {

                int productID = 0;
                productID = ProductApiAccess.GetProductIDBySKU(ApiURL, token, ProductSKU);


                if (ProductSKU.Length > 0)
                {
                     viewModelUpdate = ProductApiAccess.GetProductDetail_ProductIDByid(ApiURL, token, ProductSKU);

                    //viewModelUpdate.CADPrice = Convert.ToDouble(viewModelUpdate.AvgCost) * currencyRate;
                    //viewModelUpdate.CurrencyRate = currencyRate;
                    TempData["ProductId"] = productID;

                    return View(viewModelUpdate);

                }

                
            }
            //model.CurrencyRate = currencyRate;
           
            if(saveParentSkuVM != null)

            {
                saveParentSkuVM.Condition = viewModelUpdate.Condition;
                return View(saveParentSkuVM);
            }
            return View(viewModelUpdate);
        }
        [HttpPost]
        public IActionResult productAddMultipleSku(SaveParentSkuVM ViewModel)
        {

            token = Request.Cookies["Token"];
            token = Request.Cookies["Token"];
            
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
            if (ViewModel.Parentproduct_id > 0)
            {
                ProductApiAccess.productAddMultipleSku(ApiURL, token, ViewModel);
            }
            else
            {
               

                ViewModel.Condition = _listConditionSelectListItems;

                conditionApiAccess.GetAllCondition(ApiURL, token);
                ProductApiAccess.productAddMultipleSku(ApiURL, token, ViewModel);
                ViewBag.S3BucketURL = s3BucketURL;
                ViewBag.S3BucketURL_large = s3BucketURL_large;
            }
            return RedirectToAction("GetMultipleproductDetailist", "Product",ViewModel);
            //return View(ViewModel);
            //return RedirectToAction("ProductAddUpdate", ViewModel);
        }

        [HttpGet]
        public IActionResult EditParentById(int id)
        {
            string token = Request.Cookies["Token"];
            SaveParentSkuVM ViewModel = new SaveParentSkuVM();
            ViewModel = ProductApiAccess.EditParentById(ApiURL, token, id);
            return RedirectToAction("productAddMultipleSku", "Product", ViewModel);
        }
        [HttpGet]
        public IActionResult GetproductById(int id)
        {
            string token = Request.Cookies["Token"];
            GetParentSkuById ViewModel = new GetParentSkuById();
            var list = ProductApiAccess.GetChildProductList(ApiURL, token,id);
            ViewModel = ProductApiAccess.GetproductById(ApiURL, token, id);

            ViewModel.list = list;
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View(ViewModel);
        }
       


        [HttpPost]
        public string ShadowCreate(List<SaveSkuShadowViewModel> data)
        {
            string token = Request.Cookies["Token"];

            var staus=ProductApiAccess.ShadowCreate(ApiURL, token, data);
            return staus;


        }
        [HttpGet]
        public List<GetImageUrlOfChildSkuViewModel> GetImageUrlOfChildSku(string childSku)
        {
            string token = Request.Cookies["Token"];
            List<GetImageUrlOfChildSkuViewModel> listmodel = new List<GetImageUrlOfChildSkuViewModel>();
            listmodel = ProductApiAccess.GetImageUrlOfChildSku(ApiURL, token, childSku);
            return listmodel;
        }
        [HttpGet]
        public CheckChildSkuImageUpdatedOnSCViewModel CheckChildSkuImageUpdatedOnSC(string SKU)
        {
            token = Request.Cookies["Token"];
            var Response = ProductApiAccess.CheckChildSkuImageUpdatedOnSC(ApiURL, token, SKU);
            return Response;
        }
        [HttpPost]
        public string ImageUpdateOnSellerCloud(List<UpdateImageOnScViewModel> data)
        {
            var status = "";
            string token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            List<GetImageUrlOfChildSkuViewModel> getImage = new List<GetImageUrlOfChildSkuViewModel>();
            foreach (var item in data)
            {
                var IsIamgeCreatedOnSC = CheckChildSkuImageUpdatedOnSC(item.ProductID);
                if (IsIamgeCreatedOnSC.IsImageUpdateOnSC==0) {
                    getImage = GetImageUrlOfChildSku(item.ProductID);
                    
                    foreach (var url in getImage)
                    {
                      var getBase64=  DownloadImageFromUrl(s3BucketURL_large+"/"+url.ChildSkuImageUrl);
                        item.Content = getBase64 ;
                        item.FileName = item.ProductID;
                        item.Caption = "";
                        status = ProductApiAccess.ImageUpdateOnSellerCloud(ApiURL, authenticate.access_token, item);
                        
                    }
                    
                }
                else
                {

                }
               
            }

            return status;
        }

        public string DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;
            string base64String = "";
            try
            {
                Uri ImageUri = new Uri(imageUrl, UriKind.Absolute);
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ImageUri);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

               
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                         base64String = Convert.ToBase64String(imageBytes);
                        
                    }
               

                webResponse.Close();
            }
            catch (Exception ex)
            {
                 throw ex;
            }

            return base64String;
        }
        [HttpPost]           
        public string SaveChildSku(List<SaveChildSkuVM> data)
        {
            string token = Request.Cookies["Token"];

            var staus = ProductApiAccess.SaveChildSku(ApiURL, token, data);
            return staus;


        }
        [HttpGet]
        public List<GetShadowsOfChildViewModel> GetShadowsOfChild(string childSku)
        {
            string token = Request.Cookies["Token"];
            List<GetShadowsOfChildViewModel> listmodel = new List<GetShadowsOfChildViewModel>();
            listmodel = ProductApiAccess.GetShadowsOfChild(ApiURL, token, childSku);
            return listmodel;
        }
        [HttpGet]
        public CheckChildOrShadowCreatedOnSCViewModel CheckChildOrShadowCreatedOnSC(string SKU)
        {
            token = Request.Cookies["Token"];
            var Response = ProductApiAccess.CheckChildOrShadowCreatedOnSC(ApiURL, token, SKU);
            return Response;
        }
        [HttpPost]
        public string CreateProductOnSellerCloudAsync(List<CreateProductOnSallerCloudViewModel> data)
        {
            string token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            List<GetShadowsOfChildViewModel> childskuShadow = new List<GetShadowsOfChildViewModel>();
            CheckChildOrShadowCreatedOnSCViewModel model = new CheckChildOrShadowCreatedOnSCViewModel();
           
            var status = "";
            FileContents _fileContents = new FileContents();
            foreach (var item in data)
            {
                
                var isCreated=CheckChildOrShadowCreatedOnSC(item.ProductSKU);
                    if (isCreated.IsCreatedOnSC == 0) {
                    item.CompanyId = 513;
                    item.ProductTypeName = "Misc";
                    item.PurchaserId = 0;
                    item.SiteCost = 0;
                    item.DefaultPrice = 0;
                    item.ManufacturerId = 0;
                    item.AutoAssignUPC = false;
                    item.ProductNotes = "Create by Hld Panel";

                    status = ProductApiAccess.CreateProductOnSellerCloud(ApiURL, authenticate.access_token, item);
                    if (!string.IsNullOrEmpty(status))
                    {
                        ProductApiAccess.UpdateSkustatusonsellercloud(ApiURL, token, status);
                        childskuShadow = GetShadowsOfChild(item.ProductSKU);
                        foreach (var childshadowList in childskuShadow)
                        {
                            CreateProductOnSallerCloudViewModel shadowSKU = new CreateProductOnSallerCloudViewModel();
                            shadowSKU.ProductName = childshadowList.title;
                            shadowSKU.ProductSKU = childshadowList.sku;
                            shadowSKU.CompanyId = 513;
                            shadowSKU.ProductTypeName = "Misc";
                            shadowSKU.PurchaserId = 0;
                            shadowSKU.SiteCost = 0;
                            shadowSKU.DefaultPrice = 0;
                            shadowSKU.ManufacturerId = 0;
                            shadowSKU.AutoAssignUPC = false; 
                            shadowSKU.ProductNotes = "Create by Hld Panel"; 
                            status = ProductApiAccess.CreateProductOnSellerCloud(ApiURL, authenticate.access_token, shadowSKU);
                            ProductApiAccess.UpdateSkustatusonsellercloud(ApiURL, token, status);
                        }

                    }
                    else
                    {
                        TempData["status"] = "Product Already Created Against This SKU!";
                    }
                }
                else
                {
                    TempData["status"] = "Product Already Created Against This SKU!!";
                }

            }           
            return status;
        }
       

        [HttpPost]
        public GetXlsFileResponseViewModel CreateProductShadowOnSellerCloud(CreateshadowOnSellerCloudViewModel data)
        {
            GetXlsFileResponseViewModel status = new GetXlsFileResponseViewModel();
            string token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            //var status = "";
           
            status = ProductApiAccess.CreateProductShadowOnSellerCloud(ApiURL, authenticate.access_token, data);
            if (status != null)
            {
                
                ViewBag.status = "Relation Created Succesfully";
            }

            return status;
        }

        [HttpPost]
        public GetBulkUpdateResponseViewModel CreateBulkUpdateOnSellerCloud(CreateBulkUpdateOnSellerCloudViewModel data)
        {
            GetBulkUpdateResponseViewModel status = new GetBulkUpdateResponseViewModel();
            string token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            //var status = "";

            status = ProductApiAccess.CreateBulkUpdateOnSellerCloud(ApiURL, authenticate.access_token, data);
            if (status != null)
            {
                ViewBag.status = "Relation Created Succesfully";
            }

            return status;
        }
        [HttpPost]
        public async Task<JsonResult> GenerateXls(List<CreateRelationOnSCViewModel> data)

        {
            UpdateIsRelationViewModel updateIsRelation = new UpdateIsRelationViewModel();
            try
            {
                string excelName="";
                string fileCreationDate = DateTime.Now.ToString("yyyyMMddHHmmssfff"); 
                data = data.GroupBy(s => s.ProductSku).Select(p => p.FirstOrDefault()).Distinct().ToList();
                token = Request.Cookies["Token"];
                CreateshadowOnSellerCloudViewModel createshadowOnSeller = new CreateshadowOnSellerCloudViewModel();
                excelName =fileCreationDate + @"ShadowData.xls";
                string file = Path.GetTempPath() +excelName; 
                Workbook workbook = new Workbook();
                Worksheet worksheet = new Worksheet("FirstSheet");
                worksheet.Cells[0, 0] = new Cell("ParentSKU");
                worksheet.Cells[0, 1] = new Cell("ShadowSKU");
                worksheet.Cells[0, 2] = new Cell("CompanyID");
                List<FileContents> viewModels = ProductApiAccess.GetShadowsOfChildSku(ApiURL, token, data);
              
                for (int row = 1; row <= viewModels.Count; row++)
                {
                   
                        worksheet.Cells[row, 0] = new Cell(viewModels[row-1].ParentSKU.Trim().ToString());
                        worksheet.Cells[row, 1] = new Cell(viewModels[row-1].ShadowSKU.Trim().ToString());
                        worksheet.Cells[row, 2] = new Cell(viewModels[row-1].CompanyID.ToString().Trim());
                    
                }
                var getParentSku = ProductApiAccess.GetParentOfThisSku(ApiURL, token, viewModels.Select(a => a.ParentSKU).FirstOrDefault());
                workbook.Worksheets.Add(worksheet); 
                workbook.Save(file);
               var getFileDiractory= uploadExcellToS3(file);
                Byte[] bytes = System.IO.File.ReadAllBytes(file);
                Metadata metadata = new Metadata();
                metadata.CompanyId = 512;
                metadata.ScheduleDate = "";
                createshadowOnSeller.FileContents = Convert.ToBase64String(bytes);
                createshadowOnSeller.Format = 2;
                createshadowOnSeller.Metadata = metadata;

            var status=   CreateProductShadowOnSellerCloud(createshadowOnSeller);
                updateIsRelation.QueuedJobLink = status.QueuedJobLink;
                updateIsRelation.ParentSKU = getParentSku.ParentSku.ToString();
                updateIsRelation.FileDirectory = getFileDiractory;
                updateIsRelation.FileName = excelName ;
                updateIsRelation.JobCreationTime = DateTime.Now;
                updateIsRelation.JobType = "Relation Created for product and Shadows";
                updateIsRelation.Status = "Submitted";
                ProductApiAccess.UpdateRelationInBulkUpdateTable(ApiURL, token, updateIsRelation);
                foreach (var item in viewModels)
                {
                    updateIsRelation.QueuedJobLink = status.QueuedJobLink;
                    updateIsRelation.ParentSKU = item.ShadowSKU;
                    ProductApiAccess.UpdateRelationInProductTable(ApiURL, token, updateIsRelation);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(new { status = true });
        }

        [HttpPost]
        public async Task<JsonResult> GenerateBulkUpdateXls(List<GetBulkUpdateSkuViewModel> data)

        {
           
            try
            {
                string fileCreationDate = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                UpdateJobIdForBulkUpdateViewModel updateJobIdForBulkUpdate = new UpdateJobIdForBulkUpdateViewModel();
                string excelName = "";
                data = data.GroupBy(s => s.Sku).Select(p => p.FirstOrDefault()).Distinct().ToList();
                token = Request.Cookies["Token"];
                CreateBulkUpdateOnSellerCloudViewModel createBulkUpdateOnSellerCloudViewModel = new CreateBulkUpdateOnSellerCloudViewModel();
                excelName = fileCreationDate+@"BulkUpdateData.xls";
                string file = Path.GetTempPath() + excelName;
                Workbook workbook = new Workbook();
                Worksheet worksheet = new Worksheet("FirstSheet");
                worksheet.Cells[0, 0] = new Cell("ProductID");
                worksheet.Cells[0, 1] = new Cell("UPC");
                worksheet.Cells[0, 2] = new Cell("BrandName");
                worksheet.Cells[0, 3] = new Cell("Manufacturer");
                worksheet.Cells[0, 4] = new Cell("ManufacturerSKU");
                worksheet.Cells[0, 5] = new Cell("PackageWeightOz");
                worksheet.Cells[0, 6] = new Cell("ShippingWidth");
                worksheet.Cells[0, 7] = new Cell("ShippingHeight");
                worksheet.Cells[0, 8] = new Cell("ShippingLength");
                worksheet.Cells[0, 9] = new Cell("ShortDescription");
                worksheet.Cells[0, 10] = new Cell("LongDescription");
                worksheet.Cells[0, 11] = new Cell("AmazonEnabled");
                worksheet.Cells[0, 12] = new Cell("ASIN");
                worksheet.Cells[0, 13] = new Cell("AmazonMerchantSKU");
                worksheet.Cells[0, 14] = new Cell("FulfilledBy");
                worksheet.Cells[0, 15] = new Cell("AmazonFBASKU");
                worksheet.Cells[0, 16] = new Cell("CompanyID");
                List<BulkUpdateFileContents> viewModels = ProductApiAccess.GetDataOfSku(ApiURL, token, data);

                for (int row = 1; row <= viewModels.Count; row++)
                {
                    worksheet.Cells[row, 0] = new Cell(viewModels[row - 1].ProductID.Trim().ToString());
                    worksheet.Cells[row, 1] = new Cell(viewModels[row - 1].UPC);
                    worksheet.Cells[row, 2] = new Cell(viewModels[row - 1].BrandName);
                    worksheet.Cells[row, 3] = new Cell(viewModels[row - 1].Manufacturer);
                    worksheet.Cells[row, 4] = new Cell(viewModels[row - 1].ManufacturerSKU);
                    worksheet.Cells[row, 5] = new Cell(viewModels[row - 1].PackageWeightOz);
                    worksheet.Cells[row, 6] = new Cell(viewModels[row - 1].ShippingWidth);
                    worksheet.Cells[row, 7] = new Cell(viewModels[row - 1].ShippingHeight);
                    worksheet.Cells[row, 8] = new Cell(viewModels[row - 1].ShippingLength);
                    worksheet.Cells[row, 9] = new Cell(viewModels[row - 1].ShortDescription);
                    worksheet.Cells[row, 10] = new Cell(viewModels[row - 1].LongDescription);
                    worksheet.Cells[row, 11] = new Cell(viewModels[row - 1].AmazonEnabled);
                    worksheet.Cells[row, 12] = new Cell(viewModels[row - 1].ASIN);
                    worksheet.Cells[row, 13] = new Cell(viewModels[row - 1].AmazonMerchantSKU);
                    worksheet.Cells[row, 14] = new Cell(viewModels[row - 1].FulfilledBy);
                    worksheet.Cells[row, 15] = new Cell(viewModels[row - 1].AmazonFBASKU);
                    worksheet.Cells[row, 16] = new Cell(viewModels[row - 1].CompanyID);
                }
                workbook.Worksheets.Add(worksheet);
                workbook.Save(file);
               var getS3FilePath= uploadExcellToS3(file);
                Byte[] bytes = System.IO.File.ReadAllBytes(file);
                MetadataForBulkUpdate metadata = new MetadataForBulkUpdate();
                metadata.ScheduleDate = "";
                metadata.CreateProductIfDoesntExist = true;
                metadata.CompanyIdForNewProduct = 512;
                metadata.DoNotUpdateProducts = false;
                createBulkUpdateOnSellerCloudViewModel.FileContents = Convert.ToBase64String(bytes);
                createBulkUpdateOnSellerCloudViewModel.Format = 2;
                createBulkUpdateOnSellerCloudViewModel.Metadata = metadata;

                var status = CreateBulkUpdateOnSellerCloud(createBulkUpdateOnSellerCloudViewModel);

                if (status != null)
                {
                    var getParentSku = ProductApiAccess.GetParentOfThisSku(ApiURL, token, data.Select(a => a.Sku).FirstOrDefault());
                    updateJobIdForBulkUpdate.Sku = getParentSku.ParentSku.ToString();
                    updateJobIdForBulkUpdate.ID = status.ID;
                    updateJobIdForBulkUpdate.QueuedJobLink = status.QueuedJobLink;
                    updateJobIdForBulkUpdate.CreatedDate = DateTime.Now;
                    updateJobIdForBulkUpdate.S3FileDirectoryPath = getS3FilePath;
                    updateJobIdForBulkUpdate.FileNames = excelName;
                    updateJobIdForBulkUpdate.JobType = "Bulk Update";
                    updateJobIdForBulkUpdate.Status = "Submitted";
                    ProductApiAccess.BulkUpdateJobId(ApiURL, token, updateJobIdForBulkUpdate);
                    foreach (var item in data)
                    {
                        
                        updateJobIdForBulkUpdate.Sku = item.Sku;
                        updateJobIdForBulkUpdate.ID = status.ID;
                        updateJobIdForBulkUpdate.QueuedJobLink = status.QueuedJobLink;
                        ProductApiAccess.BulkUpdateJobIdForProductData(ApiURL, token, updateJobIdForBulkUpdate);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(new { status = true });
        }
        public string uploadExcellToS3(string filePath)
        {
            string bucketName = "";
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                logger.LogInformation("uploadExcellToS3 in uploading file filename = " + filePath);


                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _bucketName + @"/" + _bucketSubdirectory;
                }


                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);



            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
                logger.LogInformation("uploadExcellToS3 in exception  = " + s3Exception.Message);
            }
            return bucketName;
        }
        [HttpPost]
        public IActionResult UpdateChildSku(SaveChildSkuVM data)
        {
            token = Request.Cookies["Token"];
            ProductApiAccess.Update(ApiURL, token, data);
            return RedirectToAction("GetproductById", "product", data);
        }
        public IActionResult DeleteChildProduct(int child_id)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            status = ProductApiAccess.DeleteChildProduct(ApiURL, token, child_id);
            return RedirectToAction("GetMultipleproductDetailist", "product", status);
        }
        public IActionResult DeleteChildProductChildImage(int ChildImage)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            status = ProductApiAccess.DeleteChildProductChildImage(ApiURL, token, ChildImage);
            return RedirectToAction("GetMultipleproductDetailist", "product", status);
        }
        //public IActionResult productAddMultipleSku(string ProductSKU)
        //{
        //    token = Request.Cookies["Token"];
        //    double currencyRate = currencyExchangeApiAccess.GetLatestCurrencyRate(ApiURL, token);
        //    ProductInsetUpdateViewModel model = ProductPageIndexMethod();
        //    if (!string.IsNullOrEmpty(ProductSKU))
        //    {

        //        int productID = 0;
        //        productID = ProductApiAccess.GetProductIDBySKU(ApiURL, token, ProductSKU);


        //        if (ProductSKU.Length > 0)
        //        {
        //            ProductInsetUpdateViewModel viewModelUpdate = ProductApiAccess.GetProductDetail_ProductID(ApiURL, token, ProductSKU);

        //            viewModelUpdate.Condition = model.Condition;
        //            viewModelUpdate.CADPrice = Convert.ToDouble(viewModelUpdate.AvgCost) * currencyRate;
        //            viewModelUpdate.CurrencyRate = currencyRate;
        //            TempData["ProductId"] = productID;
        //            return View(viewModelUpdate);
        //        }
        //    }
        //    model.CurrencyRate = currencyRate;

        //    return View(model);
        //}
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
        [HttpGet]
        public IActionResult GetMultipleproductDetailist()
        {
            string token = Request.Cookies["Token"];
            List<SaveParentSkuVM> listmodel = new List<SaveParentSkuVM>();
            listmodel = ProductApiAccess.GetMultipleproductDetailist(ApiURL, token);
            return View(listmodel);
        }
        [HttpPost]
        //public async Task<IActionResult> productAddMultipleSku(ProductInsetUpdateViewModel model, string action)
        //{

        //    token = Request.Cookies["Token"];
        //    if (action == "Delete")
        //    {

        //        bool _status = false;
        //        DeleteProductViewModel deleteViewModel = new DeleteProductViewModel();
        //        deleteViewModel.PorductSku = model.ProductSKU;
        //        deleteViewModel.status = 0;
        //        _status = ProductApiAccess.DeleteProduct(ApiURL, token, deleteViewModel);
        //        if (_status == false)
        //        {
        //            TempData["DeleteProductMessage"] = "Deleted successfully";
        //        }
        //        else
        //        {
        //            TempData["DeleteProductMessage"] = "Can't be deleted,because it exists in SellerCloud Orders";
        //        }
        //        return RedirectToAction("productAddMultipleSku");
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (model.ProductId > 0)
        //            {
        //                int ProductId = 0;
        //                ProductId = ProductApiAccess.UpdateProduct(ApiURL, token, model);
        //                if (ProductId > 0)
        //                {

        //                    TempData["SaveProductMessage"] = "update";
        //                    return RedirectToAction("productAddMultipleSku");
        //                }
        //            }
        //            else
        //            {
        //                model.ProductSKU = model.ProductSKU.ToUpper();
        //                int ProductId = ProductApiAccess.AddProduct(ApiURL, token, model);
        //                if (ProductId > 0)
        //                {
        //                    await SaveImages(model.UploadImages, model.ProductSKU);
        //                    TempData["SaveProductMessage"] = "Save";
        //                    return RedirectToAction("productAddMultipleSku");
        //                }
        //                else
        //                {
        //                    return RedirectToAction("productAddMultipleSku");
        //                }
        //            }
        //        }
        //    }

        //    //repopulate form if error is occured

        //    ProductInsetUpdateViewModel viewModel = ProductPageIndexMethod();

        //    return View(viewModel);
        //}



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
        public JsonResult GetAlColorForAutoComplete(string Prefix){
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
       [HttpPost]
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
                        System.Drawing.Image img = System.Drawing.Image.FromStream(Imagefile.OpenReadStream(), true, true);
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
        [HttpPost]
        private async Task<bool> SaveImagesChildSku(List<IFormFile> files, string ProductSKU)
        {
            try
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
                            System.Drawing.Image img = System.Drawing.Image.FromStream(Imagefile.OpenReadStream(), true, true);
                            using (Stream ms = new MemoryStream())
                            {
                                Imagefile.CopyTo(ms);
                                await uploadFiles.uploadToS3(ms, fileName);
                                await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);

                            }
                            ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                            databaseImagesURL.product_Sku = ProductSKU;
                            databaseImagesURL.FileName = fileName;

                            status = ProductApiAccess.SaveSellerCloudImagesForChildSku(ApiURL, token, databaseImagesURL);
                        }

                    }
                }
                return status;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
           
        }

        [HttpPost]
        public async Task<IActionResult> SaveImagesSingle(List<IFormFile> files, string ProductSKU)
        {
            token = Request.Cookies["Token"];
            if (files.Count > 0)
            {
                await SaveImagesChildSku(files, Convert.ToString(HttpContext.Request.Form["ProductSKU"]));

            }
            return Json(new { message = "data is save successfully" });
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

        public JsonResult CheckManufactureExists(string name)
        {


            bool status = false;
            try
            {
                string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                string token = Request.Cookies["Token"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/CheckManufactureExists/" + name + "");
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

        public Stream GetStreamOfReducedImage(System.Drawing.Image inputPath)
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

        [HttpGet]
        public List<ApprovedPriceForInventoryPageViewModel> GetApprovedPrice(string sku)
        {
            string token = Request.Cookies["Token"];
            List<ApprovedPriceForInventoryPageViewModel> ViewModel = new List<ApprovedPriceForInventoryPageViewModel>();
            ViewModel = ProductApiAccess.GetApprovedPrice(ApiURL, token, sku);
          
            return ViewModel;


        }
        [HttpGet]
        public List<SkuTagOrderViewModel> GetTags(string sku)
        {
            string token = Request.Cookies["Token"];
            List<SkuTagOrderViewModel> ViewModel = new List<SkuTagOrderViewModel>();
            ViewModel = ProductApiAccess.GetTags(ApiURL, token, sku);

            return ViewModel;


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

        [HttpPost]
        public int GetStausFromZincNew(List<GetStatusFromZincViewModel> data)
        {
            string token = Request.Cookies["Token"];

            var ID = ProductApiAccess.GetStatusFromZincNew(ApiURL, token, data);
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
        
        public IActionResult ProductManufactured()
        {
            string token = Request.Cookies["Token"];


         
            return View();


        }
        [HttpPost]
        public IActionResult ProductManufactured(ProductManufacturedViewModel viewmodel)
        {
            string token = Request.Cookies["Token"];

            
            ProductApiAccess.SaveProductManufactured(ApiURL, token, viewmodel);
            ModelState.Clear();
            return View();


        }
        [HttpPost]
        public bool ProductManufacturedModel(ProductManufacturedViewModel model)
        {
            string token = Request.Cookies["Token"];
            
            var status= ProductApiAccess.SaveProductManufacturedModel(ApiURL, token, model);
           
            return status;
            

        }
        //public List<ProductManufacturedViewModel> GetManufacture()
        //{

        //    string token = Request.Cookies["Token"];
        //    List<ProductManufacturedViewModel> listmodel = new List<ProductManufacturedViewModel>();
        //    listmodel = ProductApiAccess.GetManufacture(ApiURL, token);
        //    return listmodel;
        //}

        [HttpPost]
        public JsonResult GetAllVendorForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
            if (Prefix == null)
            { Prefix = ""; }
            List<ProductManufacturedViewModel> model = ProductApiAccess.GetManufacture(ApiURL, token, Prefix);
            model = model.Where(x =>!string.IsNullOrWhiteSpace(x.ManufactureName)).ToList();
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
        public JsonResult GetAllVendorManuForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
          
            List<ProductManufacturedViewModel> model = ProductApiAccess.GetManufactureName(ApiURL, token, Prefix);
            
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
        public List<GetManufactureModelViewModel> GetManufactureIdByNameChange(int ManufactureId)
        {
            token = Request.Cookies["Token"];
            var list = ProductApiAccess.GetManufactureIdByNameChange(ApiURL, token, ManufactureId);
            list = list.Where(x => x.ManufactureModel !="0").ToList();
            return list;
            
        }
        [HttpGet]
        public List<ProductManufactureListViewModel> GetManufactureList(int ManufactureId)
        {
            token = Request.Cookies["Token"];
            var list = ProductApiAccess.GetManufactureList(ApiURL, token, ManufactureId);
            list = list.Where(x => x.ManufactureModel != "0").ToList();
           
            return list;

        }

        [HttpGet]
        public List<GetDeviceModelViewMdel> GetManufactureDeviceIdByNameChange(int ManufactureModel, int ManufactureId)
        {
            token = Request.Cookies["Token"];
            var list = ProductApiAccess.GetManufactureDeviceIdByNameChange(ApiURL, token, ManufactureModel, ManufactureId);
            list = list.Where(x =>!string.IsNullOrWhiteSpace(x.DeviceModel)).ToList();
            return list;

        }

        [HttpPost]
        public bool ProductDeviceModel(AddDeviceModelView model)
        {
            string token = Request.Cookies["Token"];

            var status = ProductApiAccess.ProductDeviceModel(ApiURL, token, model);
            ModelState.Clear();
            return status;
           

        }

        public IActionResult ProductManufacturedList()
        {
            string token = Request.Cookies["Token"];
            return View();
        }
        [HttpPost]
        public IActionResult ProductManufacturedList(ProductManufacturedViewModel viewmodel)
        {
            string token = Request.Cookies["Token"];


            ProductApiAccess.SaveProductManufactured(ApiURL, token, viewmodel);
            ModelState.Clear();
            return View();


        }

        [HttpPost]
        public string ManufacturedUpdate(EditManufactureListModelView data)
        {
            string token = Request.Cookies["Token"];

            var staus = ProductApiAccess.ManufacturedUpdate(ApiURL, token, data);
            return staus;


        }


        [HttpPost]
        public string BulkUpdate(EditBulkUpdateViewModel data)
        {
            string token = Request.Cookies["Token"];

            var staus = ProductApiAccess.BulkUpdateList(ApiURL, token, data);
            return staus;


        }

        [HttpGet]
        public IActionResult Style(int StyleId)
        {
            string token = Request.Cookies["Token"];
            AddStyleViewModel viewModel = new AddStyleViewModel();
            
            
            viewModel = ProductApiAccess.EditStyle(ApiURL, token, StyleId);
            var list = ProductApiAccess.GetAllStyle(ApiURL, token);
            viewModel.list = list;
            return View(viewModel);


        }
        [HttpPost]
        public IActionResult Style(AddStyleViewModel viewmodel)
        {
            string token = Request.Cookies["Token"];


            if (viewmodel.StyleId > 0)
            {
                ProductApiAccess.Style(ApiURL, token, viewmodel);
            }
            else
            {
                ProductApiAccess.Style(ApiURL, token, viewmodel);
            }
            var list = ProductApiAccess.GetAllStyle(ApiURL, token);
            viewmodel.list = list;
            
            return RedirectToAction("Style","Product");


        }
       

        //[HttpGet]
        //public List<AddStyleViewModel> Stylelist()
        //{
        //    token = Request.Cookies["Token"];
        //    var list = ProductApiAccess.Stylelist(ApiURL, token);

        //    return list;

        //}
        [HttpPut]
        public bool UpdateShadowSingleColoumn(UpdateShadowSingleColoumnViewModel data)
        
        {
            token = Request.Cookies["Token"];
            ProductApiAccess.UpdateShadowSingleColoumn(ApiURL, token, data);
            return true;
        }

        [HttpPut]
        public bool UpdateShadowSingleColoumnASIN(UpdateShadowSingleColoumnViewModel data)

        {
            token = Request.Cookies["Token"];
            ProductApiAccess.UpdateShadowSingleColoumnASIN(ApiURL, token, data);
            return true;
        }

        [HttpPut]
        public string UpdateShadowSingleColoumnForistAsin(List<UpdateShadowSingleColoumnViewModel> list)
        {
            string token = Request.Cookies["Token"];

            var staus= ProductApiAccess.UpdateShadowSingleColoumnForistAsin(ApiURL, token, list);
            return staus;


        }

        [HttpPost]
        public JsonResult GetAllVendorForAutoCompleteStyle(string Prefix)
        {
            token = Request.Cookies["Token"];
            if (Prefix == null)
            { Prefix = ""; }
            List<StyleListViewModel> model = ProductApiAccess.GetAllStyleList(ApiURL, token, Prefix);
            model = model.Where(x => !string.IsNullOrWhiteSpace(x.StyleName)).ToList();
            if (model == null)
            {
                return Json(model);
            }
            else
            {
                return Json(model);
            }

        }

        public IActionResult GetDataListForBulkUpdate(string ParentID)
        {
            string token = Request.Cookies["Token"];
            List<GetDataForBulkUpdatejobViewModel> listviewmodel = new List<GetDataForBulkUpdatejobViewModel>();
               listviewmodel = ProductApiAccess.GetDataListForBulkUpdate(ApiURL, token, ParentID);
               return View(listviewmodel);
        }
        [HttpPut]
        public bool BBQtyupdate(string SKU,bool BBQtyUpdate)
        {
            string token = Request.Cookies["Token"];

            ProductApiAccess.BBQtyupdate(ApiURL, token, SKU, BBQtyUpdate);
           
            return true;


        }

        [HttpPost]
        public bool ExecuteJob(int JobId)
        {
            bool Status = false;
            try
            {
                string token = Request.Cookies["Token"];
                Status = ProductApiAccess.ExecuteJob(ApiURL, token, JobId);
                Status = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Status;
        }
        //FOR EXPORT DATE
        public async Task<JsonResult> EXPORTDate(string dropshipstatus, string dropshipstatusSearch, string Sku, string DSTag, string TypeSearch, string WHQStatus, string BBProductID, string ASINS, string ApprovedUnitPrice,string ASINListingRemoved,string BBPriceUpdate,string BBInternalDescription)
        {
            //getting skulist from session data
            token = Request.Cookies["Token"];
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            var key = HttpContext.Session.GetString("skuList");
            var search = HttpContext.Session.GetString("dropshipsearch");

            string folder = _environment.WebRootPath;
            string excelName = $"ProductData.xlsx";
            string downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
            FileInfo file = new FileInfo(Path.GetTempPath() + excelName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.GetTempPath() + excelName);
            }



            //if (string.IsNullOrEmpty(key) && (string.IsNullOrEmpty(dropshipstatusSearch) || dropshipstatusSearch == "undefined") && (string.IsNullOrEmpty(dropshipstatus) || dropshipstatus == "undefined") && (string.IsNullOrEmpty(Sku) || Sku == "undefined") && (string.IsNullOrEmpty(asin) || asin == "undefined") && string.IsNullOrEmpty(Producttitle))

            //{

            //    List<ProductDisplayInventoryViewModel> viewModels = new List<ProductDisplayInventoryViewModel>();

            //    return PartialView("~/Views/Product/_Index.cshtml", viewModels);

            //}


            if (!string.IsNullOrEmpty(WHQStatus) && WHQStatus != "undefined")
                {
                    string[] Val = WHQStatus.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item == "1")
                        {
                            WHQStatus = "1";
                        }
                        if (item == "2")
                        {
                            WHQStatus = "2";
                        }
                    }
                }
                else
                {
                    WHQStatus = "ALL";
                }

                if (!string.IsNullOrEmpty(DSTag) && DSTag != "undefined")
                {
                    string[] Val = DSTag.Split(',');
                    int length = Val.Length;
                    foreach (var item in Val)
                    {
                        if (item !=null)
                        {
                            DSTag = item;
                        }
                      
                    }
                }
                else
                {
                    DSTag = "ALL";
                }
            if (!string.IsNullOrEmpty(BBPriceUpdate) && BBPriceUpdate != "undefined")
            {
                string[] Val = BBPriceUpdate.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item != null)
                    {
                        BBPriceUpdate = item;
                    }

                }
            }
            else
            {
                BBPriceUpdate = "ALL";
            }

            if (!string.IsNullOrEmpty(BBProductID) && BBProductID != "undefined")
            {
                string[] Val = BBProductID.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        BBProductID = "Yes";
                    }
                    if (item == "No")
                    {
                        BBProductID = "No";
                    }
                }
            }
            else
            {
                BBProductID = "ALL";
            }
            if (!string.IsNullOrEmpty(ASINS) && ASINS != "undefined")
            {
                string[] Val = ASINS.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ASINS = "Yes";
                    }
                    if (item == "No")
                    {
                        ASINS = "No";
                    }
                }
            }
            else
            {
                ASINS = "ALL";
            }
            if (!string.IsNullOrEmpty(ApprovedUnitPrice) && ApprovedUnitPrice != "undefined")
            {
                string[] Val = ApprovedUnitPrice.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ApprovedUnitPrice = "Yes";
                    }
                    if (item == "No")
                    {
                        ApprovedUnitPrice = "No";
                    }
                }
            }
            else
            {
                ApprovedUnitPrice = "ALL";
            }
            if (!string.IsNullOrEmpty(ASINListingRemoved) && ASINListingRemoved != "undefined")
            {
                string[] Val = ASINListingRemoved.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ASINListingRemoved = "Yes";
                    }
                    if (item == "No")
                    {
                        ASINListingRemoved = "No";
                    }
                }
            }
            else
            {
                ASINListingRemoved = "ALL";
            }
            if (!string.IsNullOrEmpty(BBInternalDescription) && BBInternalDescription != "undefined")
            {
                string[] Val = BBInternalDescription.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        BBInternalDescription = "Yes";
                    }
                    if (item == "No")
                    {
                        BBInternalDescription = "No";
                    }
                }
            }
            else
            {
                BBInternalDescription = "ALL";
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
                //if (asin == null || asin == "undefined")
                //{
                //    asin = "Nill";
                //}
                //if (dropshipstatusSearch == "asin")
                //{
                //    asin = Sku;
                //    Sku = "Nill";
                //}
                //if (Producttitle == null || Producttitle == "undefined")
                //{
                //    Producttitle = "Nill";
                //}
                //if (dropshipstatusSearch == "ProductTitle")
                //{
                //    Producttitle = Sku;
                //    Sku = "Nill";
                //}
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
                //if (asin == null || asin == "undefined")
                //{
                //    asin = "Nill";
                //}
                //if (dropshipstatus == "asin")
                //{
                //    asin = Sku;
                //    Sku = "Nill";
                //}
                //if (Producttitle == null || Producttitle == "undefined")
                //{
                //    Producttitle = "Nill";
                //}
                //if (dropshipstatus == "ProductTitle")
                //{
                //    Producttitle = Sku;
                //    Sku = "Nill";
                //}
                if (dropshipstatus == "asin" || dropshipstatus == "sku" || dropshipstatus == "ProductTitle")
                {
                    dropshipstatus = "All";
                }
                
                
                List<ExportProductDataViewModel> viewModels = ProductApiAccess.GetAllProductsForExportWithLimitCount(ApiURL, token, dropshipstatus, dropshipstatusSearch, Sku.Trim(), DSTag, TypeSearch, WHQStatus,BBProductID,ASINS,ApprovedUnitPrice, ASINListingRemoved, BBPriceUpdate, BBInternalDescription);
                await Task.Yield();
                using (var package = new ExcelPackage(file))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(viewModels, true);
                    package.Save();
                }

            return Json(new { filePath = Path.GetTempPath(), fileName = excelName });
        }
     
        [HttpPost]
        public async Task<JsonResult>  GetSinglePageExportResult( List<ExportProductDataViewModel> data)
        {
            string token = Request.Cookies["Token"];
            string folder = _environment.WebRootPath;
            string excelName = $"ProductData.xlsx";
            string downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
            FileInfo file = new FileInfo(Path.GetTempPath() + excelName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.GetTempPath() + excelName);
            }


            List<ExportProductDataViewModel> viewModels = ProductApiAccess.GetSinglePageExportResult(ApiURL, token, data);
            await Task.Yield();
            using (var package = new ExcelPackage(file))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(viewModels, true);
                package.Save();
            }
            
            return Json(new { filePath = Path.GetTempPath(), fileName = excelName });
        }
        public int SelectAllForGetStatusFromZinc(string dropshipstatus, string dropshipstatusSearch, string Sku, string DSTag, string TypeSearch, string WHQStatus, string BBProductID, string ASINS, string ApprovedUnitPrice,string ASINListingRemoved,string BBPriceUpdate,string BBInternalDescription)
        {
          
            token = Request.Cookies["Token"];
           
            if (!string.IsNullOrEmpty(WHQStatus) && WHQStatus != "undefined")
            {
                string[] Val = WHQStatus.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "1")
                    {
                        WHQStatus = "1";
                    }
                    if (item == "2")
                    {
                        WHQStatus = "2";
                    }
                }
            }
            else
            {
                WHQStatus = "ALL";
            }
            if (!string.IsNullOrEmpty(BBInternalDescription) && BBInternalDescription != "undefined")
            {
                string[] Val = BBInternalDescription.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        BBInternalDescription = "Yes";
                    }
                    if (item == "No")
                    {
                        BBInternalDescription = "No";
                    }
                }
            }
            else
            {
                BBInternalDescription = "ALL";
            }
            if (!string.IsNullOrEmpty(BBPriceUpdate) && BBPriceUpdate != "undefined")
            {
                string[] Val = BBPriceUpdate.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item != null)
                    {
                        BBPriceUpdate = item;
                    }

                }
            }
            else
            {
                BBPriceUpdate = "ALL";
            }
            if (!string.IsNullOrEmpty(DSTag) && DSTag != "undefined")
            {
                string[] Val = DSTag.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item !=null)
                    {
                        DSTag = item;
                    }
                    
                }
            }
            else
            {
                DSTag = "ALL";
            }
            if (!string.IsNullOrEmpty(BBProductID) && BBProductID != "undefined")
            {
                string[] Val = BBProductID.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        BBProductID = "Yes";
                    }
                    if (item == "No")
                    {
                        BBProductID = "No";
                    }
                }
            }
            else
            {
                BBProductID = "ALL";
            }
            if (!string.IsNullOrEmpty(ASINS) && ASINS != "undefined")
            {
                string[] Val = ASINS.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ASINS = "Yes";
                    }
                    if (item == "No")
                    {
                        ASINS = "No";
                    }
                }
            }
            else
            {
                ASINS = "ALL";
            }
            if (!string.IsNullOrEmpty(ApprovedUnitPrice) && ApprovedUnitPrice != "undefined")
            {
                string[] Val = ApprovedUnitPrice.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ApprovedUnitPrice = "Yes";
                    }
                    if (item == "No")
                    {
                        ApprovedUnitPrice = "No";
                    }
                }
            }
            else
            {
                ApprovedUnitPrice = "ALL";
            }
            if (!string.IsNullOrEmpty(ASINListingRemoved) && ASINListingRemoved != "undefined")
            {
                string[] Val = ASINListingRemoved.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ASINListingRemoved = "Yes";
                    }
                    if (item == "No")
                    {
                        ASINListingRemoved = "No";
                    }
                }
            }
            else
            {
                ASINListingRemoved = "ALL";
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
            //if (asin == null || asin == "undefined")
            //{
            //    asin = "Nill";
            //}
            //if (dropshipstatusSearch == "asin")
            //{
            //    asin = Sku;
            //    Sku = "Nill";
            //}
            //if (Producttitle == null || Producttitle == "undefined")
            //{
            //    Producttitle = "Nill";
            //}
            //if (dropshipstatusSearch == "ProductTitle")
            //{
            //    Producttitle = Sku;
            //    Sku = "Nill";
            //}
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
           
            if (dropshipstatus == "asin" || dropshipstatus == "sku" || dropshipstatus == "ProductTitle")
            {
                dropshipstatus = "All";
            }

            int count;
            count = ProductApiAccess.SelectAllForGetStatusFromZinc(ApiURL, token, dropshipstatus, dropshipstatusSearch, Sku.Trim(), DSTag, TypeSearch, WHQStatus, BBProductID, ASINS, ApprovedUnitPrice, ASINListingRemoved, BBPriceUpdate, BBInternalDescription);
            //var ASIN = viewModels.ASIN;
            //var SKU = viewModels.SKU;
            //var TotalCount = viewModels.TotalCount;
            //viewModels.ASIN =viewModels.ASIN;
            //viewModels.SKU =viewModels.SKU;
            //viewModels.TotalCount =viewModels.TotalCount;
            return count;
        }
        [HttpPost]
        public int SelectAllSKUandASINGetStatusFromZinc(string dropshipstatus, string dropshipstatusSearch, string Sku, string DSTag, string TypeSearch, string WHQStatus,string BBProductID, string ASINS, string ApprovedUnitPrice,string ASINListingRemoved,string BBPriceUpdate,string BBInternalDescription)
        {
            //getting skulist from session data
            token = Request.Cookies["Token"];
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            var key = HttpContext.Session.GetString("skuList");
            var search = HttpContext.Session.GetString("dropshipsearch");

            string folder = _environment.WebRootPath;
            string excelName = $"ProductData.xlsx";
            string downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
            FileInfo file = new FileInfo(Path.GetTempPath() + excelName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.GetTempPath() + excelName);
            }



            //if (string.IsNullOrEmpty(key) && (string.IsNullOrEmpty(dropshipstatusSearch) || dropshipstatusSearch == "undefined") && (string.IsNullOrEmpty(dropshipstatus) || dropshipstatus == "undefined") && (string.IsNullOrEmpty(Sku) || Sku == "undefined") && (string.IsNullOrEmpty(asin) || asin == "undefined") && string.IsNullOrEmpty(Producttitle))

            //{

            //    List<ProductDisplayInventoryViewModel> viewModels = new List<ProductDisplayInventoryViewModel>();

            //    return PartialView("~/Views/Product/_Index.cshtml", viewModels);

            //}
            if (!string.IsNullOrEmpty(BBInternalDescription) && BBInternalDescription != "undefined")
            {
                string[] Val = BBInternalDescription.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        BBInternalDescription = "Yes";
                    }
                    if (item == "No")
                    {
                        BBInternalDescription = "No";
                    }
                }
            }
            else
            {
                BBInternalDescription = "ALL";
            }

            if (!string.IsNullOrEmpty(WHQStatus) && WHQStatus != "undefined")
            {
                string[] Val = WHQStatus.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "1")
                    {
                        WHQStatus = "1";
                    }
                    if (item == "2")
                    {
                        WHQStatus = "2";
                    }
                }
            }
            else
            {
                WHQStatus = "ALL";
            }

            if (!string.IsNullOrEmpty(DSTag) && DSTag != "undefined")
            {
                string[] Val = DSTag.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item != null)
                    {
                        DSTag = item;
                    }

                }
            }
            else
            {
                DSTag = "ALL";
            }
            if (!string.IsNullOrEmpty(BBPriceUpdate) && BBPriceUpdate != "undefined")
            {
                string[] Val = BBPriceUpdate.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item != null)
                    {
                        BBPriceUpdate = item;
                    }

                }
            }
            else
            {
                BBPriceUpdate = "ALL";
            }
            if (!string.IsNullOrEmpty(BBProductID) && BBProductID != "undefined")
            {
                string[] Val = BBProductID.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        BBProductID = "Yes";
                    }
                    if (item == "No")
                    {
                        BBProductID = "No";
                    }
                }
            }
            else
            {
                BBProductID = "ALL";
            }
            if (!string.IsNullOrEmpty(ASINS) && ASINS != "undefined")
            {
                string[] Val = ASINS.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ASINS = "Yes";
                    }
                    if (item == "No")
                    {
                        ASINS = "No";
                    }
                }
            }
            else
            {
                ASINS = "ALL";
            }
            if (!string.IsNullOrEmpty(ApprovedUnitPrice) && ApprovedUnitPrice != "undefined")
            {
                string[] Val = ApprovedUnitPrice.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ApprovedUnitPrice = "Yes";
                    }
                    if (item == "No")
                    {
                        ApprovedUnitPrice = "No";
                    }
                }
            }
            else
            {
                ApprovedUnitPrice = "ALL";
            }

            if (!string.IsNullOrEmpty(ASINListingRemoved) && ASINListingRemoved != "undefined")
            {
                string[] Val = ASINListingRemoved.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Yes")
                    {
                        ASINListingRemoved = "Yes";
                    }
                    if (item == "No")
                    {
                        ASINListingRemoved = "No";
                    }
                }
            }
            else
            {
                ASINListingRemoved = "ALL";
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
            //if (asin == null || asin == "undefined")
            //{
            //    asin = "Nill";
            //}
            //if (dropshipstatusSearch == "asin")
            //{
            //    asin = Sku;
            //    Sku = "Nill";
            //}
            //if (Producttitle == null || Producttitle == "undefined")
            //{
            //    Producttitle = "Nill";
            //}
            //if (dropshipstatusSearch == "ProductTitle")
            //{
            //    Producttitle = Sku;
            //    Sku = "Nill";
            //}
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
            //if (asin == null || asin == "undefined")
            //{
            //    asin = "Nill";
            //}
            //if (dropshipstatus == "asin")
            //{
            //    asin = Sku;
            //    Sku = "Nill";
            //}
            //if (Producttitle == null || Producttitle == "undefined")
            //{
            //    Producttitle = "Nill";
            //}
            //if (dropshipstatus == "ProductTitle")
            //{
            //    Producttitle = Sku;
            //    Sku = "Nill";
            //}
            if (dropshipstatus == "asin" || dropshipstatus == "sku" || dropshipstatus == "ProductTitle")
            {
                dropshipstatus = "All";
            }

            List<GetStatusFromZincViewModel> viewModels = new List<GetStatusFromZincViewModel>();
            int JobId;
            JobId = ProductApiAccess.SelectAllSKUandASINGetStatusFromZinc(ApiURL, token, dropshipstatus, dropshipstatusSearch, Sku.Trim(), DSTag, TypeSearch, WHQStatus, BBProductID, ASINS, ApprovedUnitPrice, ASINListingRemoved, BBPriceUpdate,BBInternalDescription);
     
            return JobId;
        }
    }
}
