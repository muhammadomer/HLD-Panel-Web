using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class PredictionHistoryController : Controller
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
        string SCRestURL = "";
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        PredictionHistoryApiAccess _ApiAccess = null;
        OrderNotesAPiAccess _OrderApiAccess = null;

        private readonly Microsoft.Extensions.Logging.ILogger logger;
        UploadFilesToS3ForJobsAPIAccess ApiAccess = null;
        private readonly IConfiguration _configuration;
        public PredictionHistoryController(IConfiguration configuration, IHostingEnvironment environment, ILogger<PredictionHistoryController> _logger)
        {
            _hostingEnvironment = environment;
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ApiAccess = new UploadFilesToS3ForJobsAPIAccess();
            this.logger = _logger;
            _ApiAccess = new PredictionHistoryApiAccess();
            _OrderApiAccess = new OrderNotesAPiAccess();
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            _encDecChannel = new EncDecChannel();
        }

        public IActionResult Index(HistorySearchViewModel viewModel)
        {
            string token = Request.Cookies["Token"];
            bool Approved = false;
            bool Excluded = false;
            bool Continue = false;
            bool KitShadowStatus = false;
            if (!string.IsNullOrEmpty(viewModel.ItemType) && viewModel.ItemType != "undefined")
            {
                string[] Val = viewModel.ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Approved")
                    {
                        Approved = true;
                    }
                    if (item == "Excluded")
                    {
                        Excluded = true;
                    }
                    if (item == "Continue")
                    {
                        Continue = true;
                    }
                    if (item == "KitShadowStatus")
                    {
                        KitShadowStatus = true;
                    }

                }
            }

            if (viewModel.VendorId != 0 )
            {
                POMasterID = viewModel.VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }
            if (!string.IsNullOrEmpty(viewModel.SearchFromSkuListPredict))
            {
                var lines = viewModel.SearchFromSkuListPredict.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string newLines = string.Join(",", lines);
                viewModel.SearchFromSkuListPredict = getString(newLines);
                string skuList = JsonConvert.SerializeObject(viewModel.SearchFromSkuListPredict);
                HttpContext.Session.SetString("skuList", skuList);
               
                ViewBag.skuSearchList = "searchList";
            }
            else
            {
                viewModel.SearchFromSkuListPredict = "Nill";
                ViewBag.skuSearchList = "";
            }
            var Count = _ApiAccess.PredictionSummaryCount(ApiURL, token, POMasterID, viewModel.SKU, viewModel.Title, Approved,Excluded, KitShadowStatus,Continue, viewModel.SearchFromSkuListPredict, viewModel.Type);
            ViewBag.TotalCount = Count;
            return View(viewModel);
        }

        public IActionResult PredictionDetail(int? page, string SKU, string Title, int VendorId, string Sort, string SortedType, string ItemType = "", int Type = 0)
        {
            string token = Request.Cookies["Token"];
            var key = HttpContext.Session.GetString("skuList");
            bool Approved = false;
            bool Excluded = false;
            bool Continue = false;
            bool KitShadowStatus = false;

           

            if (!string.IsNullOrEmpty(ItemType) && ItemType != "undefined")
            {
                string[] Val = ItemType.Split(',');
                int length = Val.Length;
                foreach (var item in Val)
                {
                    if (item == "Approved")
                    {
                        Approved = true;
                    }
                    if (item == "Excluded")
                    {
                        Excluded = true;
                    }
                    if (item == "Continue")
                    {
                        Continue = true;
                    }
                    if (item == "KitShadowStatus")
                    {
                        KitShadowStatus = true;
                    }

                }
            }
           
            IPagedList<PredictionHistroyViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 50;
            int offset = 0;
            
            int startlimit = pageSize;
            if (page.HasValue)
            {
                offset = (pageNumber - 1) * pageSize;
                ViewBag.pageNumber = page.Value;
            }
            if (VendorId != 0)
            {
                POMasterID = VendorId;
            }
            else
            {
                POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            }
           
            if (!string.IsNullOrEmpty(key))
            {
                string skuList = JsonConvert.DeserializeObject<string>(key);
                List<PredictionHistroyViewModel> listmodels = new List<PredictionHistroyViewModel>();
                listmodels = _ApiAccess.GetPredictionDetailCopy(ApiURL, token, startlimit, offset, skuList);
                data = new StaticPagedList<PredictionHistroyViewModel>(listmodels, pageNumber, pageSize, listmodels.Count);
                
                ViewBag.S3BucketURL = s3BucketURL;
                ViewBag.S3BucketURL_large = s3BucketURL_large;
                HttpContext.Session.Remove("skuList");
                ViewBag.skuSearchList = "searchList";
                return PartialView("~/Views/PredictionHistory/PredictionDetail.cshtml", data);

            }
            if (string.IsNullOrEmpty(SKU))
                SKU = "undefined";
            if (string.IsNullOrEmpty(Title))
                Title = "undefined";
            List<PredictionHistroyViewModel> listmodel = new List<PredictionHistroyViewModel>();
            listmodel = _ApiAccess.GetPredictionDetail(ApiURL, token, startlimit, offset, POMasterID, SKU, Title, Approved, Excluded, KitShadowStatus, Continue, Sort, SortedType, Type);
            data = new StaticPagedList<PredictionHistroyViewModel>(listmodel, pageNumber, pageSize, listmodel.Count);
           
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
          
            return PartialView("~/Views/PredictionHistory/PredictionDetail.cshtml", data);
        }

        public IActionResult SkuSelectListPredict()
        {
            return PartialView("~/Views/PredictionHistory/_skuSelectListPredict.cshtml");
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
        public int Save(PredictionPOViewModel data)
        {
            token = Request.Cookies["Token"];
            PurchaseOrderDataViewModel viewModel = new PurchaseOrderDataViewModel();
            viewModel.VendorId = data.VendorId;
            viewModel.OrderedOn = DateTime.Now;
            if (data.Currency == "USD")
            { viewModel.CurrencyCode = 0; }
            if (data.Currency == "CAD")
            { viewModel.CurrencyCode = 4; }
            if (data.Currency == "CNY")
            { viewModel.CurrencyCode = 8; }
            viewModel.POStatus = 6;


            if (string.IsNullOrEmpty(data.Notes) || data.Notes == "undefined")
                viewModel.Notes = "PO created by Hld Panel";
            else
                viewModel.Notes = data.Notes;
            viewModel.InternalPOId = data.InternalPOID;
            int Id = _ApiAccess.SavePO(ApiURL, token, viewModel);

            foreach (var item in data.list)
            {
                PredictPOItemViewModel obj = new PredictPOItemViewModel();
                obj.idPurchaseOrdersItems = item.idPurchaseOrdersItems;
                obj.ProductID = item.SKU;
                obj.UnitPrice = item.ApprovedUnitPrice;
                obj.VendorID = data.VendorId;
                obj.Currency = data.Currency;
                obj.QtyOrdered = item.POQty;
                if (data.InternalPOID != 0)
                {
                    obj.InternalPOId = data.InternalPOID;
                }
                else
                {
                    obj.InternalPOId = Id;
                }
                int itemId = _ApiAccess.SavePOItem(ApiURL, token, obj);
            }
            return Id;
        }

        public int SaveToSC(PredictionPOViewModel data)
        {
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);

            PurchaseOrderDataViewModel viewModel = new PurchaseOrderDataViewModel();
            SCPOViewModel SC = new SCPOViewModel();
            SC.VendorID = viewModel.VendorId = data.VendorId;
            viewModel.OrderedOn = DateTime.Now;
            if (data.Currency == "USD")
            { viewModel.CurrencyCode = 0; }
            if (data.Currency == "CAD")
            { viewModel.CurrencyCode = 4; }
            if (data.Currency == "CNY")
            { viewModel.CurrencyCode = 8; }
            viewModel.POStatus = 1;
            viewModel.InternalPOId = data.InternalPOID;

            if (string.IsNullOrEmpty(data.Notes) || data.Notes == "undefined")
                SC.Description = viewModel.Notes = "PO created by Hld Panel";
            else
                SC.Description = viewModel.Notes = data.Notes;




            SC.CompanyID = 512;
            SC.POType = "PurchaseOrder";
            SC.CaseQtyMode = false;
            SC.DefaultWarehouseID = 358;
            SC.VendorNote = "";
            List<Product> list = new List<Product>();

            foreach (var item in data.list)
            {
                Product p = new Product()
                {
                    ProductID = item.SKU,
                    QtyUnitsOrdered = item.POQty,
                    UnitPrice = Convert.ToDouble(item.ApprovedUnitPrice),
                };
                list.Add(p);
            }
            SC.Products = list;

            SCShippingAddress shippingAddress = new SCShippingAddress()
            {
                FirstName = "Rizwan",
                LastName = "Sohail",
                MiddleName = "",
                ZipCode = "L5L 5X5",
                City = "Mississauga",
                Country = "CA",
                ContactName = "Rizwan",
                Business = "Rizwan",
                AddressLine1 = "4090 Ridgeway Drive ",
                AddressLine2 = "Unit 22",
                Fax = "",
                Region = "",
                Phone = "5149961278",
                State = "ON"
            };

            SC.ShippingAddress = shippingAddress;

            int POID = _ApiAccess.SCSavePO(ApiURL, authenticate.access_token, SC);
            viewModel.POId = POID;
            if (POID != 0)
            {
                int Id = _ApiAccess.SavePO(ApiURL, token, viewModel);

                foreach (var item in data.list)
                {
                    PredictPOItemViewModel obj = new PredictPOItemViewModel();
                    obj.POId = POID;
                    obj.idPurchaseOrdersItems = item.idPurchaseOrdersItems;
                    obj.ProductID = item.SKU;
                    obj.UnitPrice = item.ApprovedUnitPrice;
                    obj.VendorID = data.VendorId;
                    obj.Currency = data.Currency;
                    obj.QtyOrdered = item.POQty;
                    if (data.InternalPOID != 0)
                    {
                        obj.InternalPOId = data.InternalPOID;
                    }
                    else
                    {
                        obj.InternalPOId = Id;
                    }
                    int itemId = _ApiAccess.SavePOItem(ApiURL, token, obj);
                }
            }
            return POID;
        }

        [HttpGet]
        public PredictionPOViewModel GetPOById(int Id)
        {
            string token = Request.Cookies["Token"];
            var data = _ApiAccess.GetPO(ApiURL, token, Id);
            return (data);
        }
        //public IActionResult PredictionSummary()
        //{
        //    string token = Request.Cookies["Token"];
        //    int listmodel = 0;
        //    listmodel = _ApiAccess.PredictionSummaryCount(ApiURL, token);
        //    ViewBag.Records = listmodel;
        //    return View();
        //}



        [HttpGet]
        public List<PredictionInternalPOList> GetInternalPOIdBySku(string data)
        {
            string token = Request.Cookies["Token"];
            var responses = _ApiAccess.GetInternalPOIdBySku(ApiURL, token, data);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return responses;
        }


        public int GetOnDemand(string[] data)
        {
            int val = 0;
            string token = Request.Cookies["Token"];

            if (data != null && data.Count() > 0)
            {
                foreach (var sku in data)
                {

                    var responce = _ApiAccess.GetOnDemand(ApiURL, token, sku);
                    if (responce)
                        val = 1;
                    else
                        val = 0;
                }
            }


            return val;
        }

        [HttpGet]
        public IActionResult PredictionList(string data)
        {
            token = Request.Cookies["Token"];
            if (data.Length < 20)
            {

                var responses = _ApiAccess.GetPO(ApiURL, token, Convert.ToInt32(data));
                if (responses.POstatus == 6)
                {

                    ViewBag.S3BucketURL = s3BucketURL;
                    ViewBag.S3BucketURL_large = s3BucketURL_large;
                    return View(responses);

                }
                else
                {
                    ViewBag.S3BucketURL = s3BucketURL;
                    ViewBag.S3BucketURL_large = s3BucketURL_large;
                    //return View(responses);
                    return RedirectToAction("Index");
                }

            }
            else
            {
                PredictionPOViewModel responses = JsonConvert.DeserializeObject<PredictionPOViewModel>(data);
                if (responses.InternalPOID > 0)
                {
                    var responses2 = _ApiAccess.GetPO(ApiURL, token, responses.InternalPOID);
                    foreach (var item in responses.list)
                    {
                        var sku = responses2.list.Where(s => s.SKU == item.SKU).FirstOrDefault();
                        if (sku == null)
                            responses2.list.Add(item);
                    }
                    responses = responses2;
                }
                ViewBag.S3BucketURL = s3BucketURL;
                ViewBag.S3BucketURL_large = s3BucketURL_large;
                return View(responses);
            }
        }

        
     
        [HttpDelete]
        public bool DeletePO(int Id)
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.DeletePO(ApiURL, token, Id);
            return Response;
        }

        [HttpDelete]
        public bool DeletePOItem(int Id)
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.DeletePOItem(ApiURL, token, Id);
            return Response;
        }

        [HttpGet]
        public SoldQtyDaysViewModel GetSoldQtyDays(string SKU)
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.GetSoldQtyDays(ApiURL, token, SKU);
            return Response;
        }

        [HttpGet]
        public List<SKUDetailModel> GetSKUDetailBySku(string SKU)
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.GetSKUDetailBySku(ApiURL, token, SKU);
            return Response;
        }

        [HttpGet]
        public IActionResult DraftPO()
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.DraftPOList(ApiURL, token);
            return View(Response);
        }

        [HttpGet]
        public List<DraftPOViewModel> DraftPOList()
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.DraftPOList(ApiURL, token);
            return Response;
        }

        [HttpGet]
        public List<WareHouseProductQuantitylistViewModel> GetWareHouseProductQuantitylistBySku(string SKU)
        {
            token = Request.Cookies["Token"];
            var Response = _ApiAccess.GetWareHouseProductQuantitylistBySku(ApiURL, token, SKU);
            return Response;
        }

        [HttpPost]
        public string PredictIncludedExcluded(List<PredictIncludedExcludedViewModel> data)
        {
            string token = Request.Cookies["Token"];

            var staus = _ApiAccess.PredictIncludedExcluded(ApiURL, token, data);
            return staus;


        }
        [HttpGet]
        public IActionResult PredictionListDummy(string data)
        {
            token = Request.Cookies["Token"];
            if (data.Length < 20)
            {

                var responses = _ApiAccess.GetPO(ApiURL, token, Convert.ToInt32(data));
                if (responses.POstatus == 6)
                {

                    ViewBag.S3BucketURL = s3BucketURL;
                    ViewBag.S3BucketURL_large = s3BucketURL_large;
                    return View(responses);

                }
                else
                {
                    ViewBag.S3BucketURL = s3BucketURL;
                    ViewBag.S3BucketURL_large = s3BucketURL_large;
                    //return View(responses);
                    return RedirectToAction("Index");
                }

            }
            else
            {
                List<PredictionInternalSKUList> Obj = JsonConvert.DeserializeObject<List<PredictionInternalSKUList>>(data);
                PredictionPOViewModel responses = new PredictionPOViewModel();
                responses = _ApiAccess.GetDataForPOCreation(ApiURL, token, Obj);
                if (Obj[0].InternalPOID > 0)
                {
                    var responses2 = _ApiAccess.GetPO(ApiURL, token, Obj[0].InternalPOID);
                    foreach (var item in responses.list)
                    {
                        var sku = responses2.list.Where(s => s.SKU == item.SKU).FirstOrDefault();
                        if (sku == null)
                            responses2.list.Add(item);
                    }
                    responses = responses2;
                }
                ViewBag.S3BucketURL = s3BucketURL;
                ViewBag.S3BucketURL_large = s3BucketURL_large;
                return View(responses);
            }
        }
        [HttpPost]
        public PredictionPOViewModel GetDropftPOList(List<PredictionInternalSKUList> data)
        {
           token = Request.Cookies["Token"];

              var item =_ApiAccess.GetDataForPOCreation(ApiURL, token, data);

            return item;
        }

    }
}