using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Transfer;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
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
    public class ApprovedPriceController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        private static string _Ds_Qty_CommentsSubdirectory = "Ds_Qty_Comments";
        private static string _ImportMissingSkuSubdirectory = "MissingSKUFromSellerCloud";

        string ApiURL = "";
        string token = "";
        int POMasterID = 0;
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        ApprovedPriceApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        UploadFilesToS3ForJobsAPIAccess ApiAccess = null;
        private readonly IConfiguration _configuration;
        public ApprovedPriceController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ApprovedPriceController> _logger)
        {
            _hostingEnvironment = environment;


            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ApiAccess = new UploadFilesToS3ForJobsAPIAccess();
            this.logger = _logger;
            _ApiAccess = new ApprovedPriceApiAccess();
        }

        public IActionResult Index(ApprovedSearchViewModel approvedSearch)
        {
            token = Request.Cookies["Token"];

            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            string SKU = approvedSearch.SKU;
            string Title = approvedSearch.Title;
            if (approvedSearch.Vendor != 0 && approvedSearch.Vendor != null)
                POMasterID = Convert.ToInt32(approvedSearch.Vendor);

            _ApiAccess = new ApprovedPriceApiAccess();

            var key = HttpContext.Session.GetString("skuList");
            if (!string.IsNullOrEmpty(approvedSearch.SearchFromSkuList))
            {
                var lines = approvedSearch.SearchFromSkuList.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string newLines = string.Join(",", lines);
                string skuList = getString(newLines);
                var count = _ApiAccess.GetCounter(ApiURL, token, POMasterID, SKU, Title, skuList);
                skuList = JsonConvert.SerializeObject(skuList);
                HttpContext.Session.SetString("skuList", skuList);
                ViewBag.skuSearchList = "searchList";
                ViewBag.TotalCount = count;
            }
            else
            {
                approvedSearch.SearchFromSkuList = "Nill";
                ViewBag.skuSearchList = "";
                var count = _ApiAccess.GetCounter(ApiURL, token, POMasterID, SKU,Title,"Nill");
                ViewBag.TotalCount = count;
             
            }

             
           
            return View(approvedSearch);
           

        }
        [HttpPost]
        public IActionResult ApprovedPricesList(int? page, string SKU, string Title, int Vendor)
        {
            //if (!string.IsNullOrEmpty(SKU) && SKU != "undefined")
            //    SKU = "";
            //if (!string.IsNullOrEmpty(Title) && Title != "undefined")
            //    Title = "";


            token = Request.Cookies["Token"];
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            if (Vendor != 0)
                POMasterID = Vendor;
            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<ApprovedPriceViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;

            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }

            List<ApprovedPriceViewModel> _viewModel = null;

           
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            var key = HttpContext.Session.GetString("skuList");
            if (!string.IsNullOrEmpty(key))
            {
                string skuList = JsonConvert.DeserializeObject<string>(key);

                ApprovedSearchViewModel viewmodel = new ApprovedSearchViewModel();

                viewmodel.SearchFromSkuList = skuList;

                _viewModel = _ApiAccess.GetApprovedPriceList(ApiURL, token, POMasterID, startlimit, endLimit, SKU, Title, skuList);
                data = new StaticPagedList<ApprovedPriceViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                HttpContext.Session.Remove("skuList");
                ViewBag.skuSearchList = "searchList";
                return PartialView("~/Views/ApprovedPrice/ApprovedPricePartialView.cshtml", data);
            }

            else
            {
                _viewModel = _ApiAccess.GetApprovedPriceList(ApiURL, token, POMasterID, startlimit, endLimit, SKU, Title);
                data = new StaticPagedList<ApprovedPriceViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                ViewBag.skuSearchList = "";
            }
                return PartialView("~/Views/ApprovedPrice/ApprovedPricePartialView.cshtml", data);
        }

       

        [HttpGet]
        public IActionResult AddUpdateApprovedPrice()
        {
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View();
        }
        [HttpGet]
        public IActionResult ApprovedPriceLogs(string SKU)
        {
            List<ApprovedPriceViewModel> _viewModel = new List<ApprovedPriceViewModel>();
            token = Request.Cookies["Token"];
            POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            _viewModel = _ApiAccess.GetApprovedPriceLogs(ApiURL, token, POMasterID, SKU);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View(_viewModel);
        }

        [HttpPost]
        public IActionResult AddUpdateApprovedPrice(ApprovedPriceViewModel viewModel)
        {

            token = Request.Cookies["Token"];
            if (viewModel.idApprovedPrice > 0)
            {
                bool st = _ApiAccess.UpdatePrice(ApiURL, token, viewModel);
            }
            else
            {
                int result = _ApiAccess.ApprovedPrice(ApiURL, token, viewModel);
            }


            return RedirectToAction("Index", "ApprovedPrice");
        }



        public IActionResult BullkApprovedPrices()
        {
            return View();
        }
        public IActionResult Edit(int id)
        {
            token = Request.Cookies["Token"];
            ApprovedPriceViewModel result = _ApiAccess.GetApprovedPricesForedit(ApiURL, token, id);

            return View(result);
        }

        public IActionResult UpdateBullkApprovedPrices(IFormFile files)

        {
            JobIdReturnViewModel jobIdReturnView = new JobIdReturnViewModel();

            string FileNameDate = DateTime.Now.Ticks + files.FileName;
            try
            {

                if (files.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(files.ContentDisposition)
                            .FileName
                            .TrimStart().ToString();
                    logger.LogInformation("UploadAsinSkuMappingToS3 filename = " + filename);
                    filename = Path.GetTempPath() + FileNameDate;
                    logger.LogInformation("UploadAsinSkuMappingToS3 after environment filename = " + filename);
                    using (var fs = System.IO.File.Create(filename))
                    {
                        files.CopyTo(fs);
                        fs.Flush();
                    }//these code snippets saves the uploaded files to the project directory
                    logger.LogInformation("UploadAsinSkuMappingToS3 after creating file filename = " + filename);
                    string bucketname = uploadExcellToS3(filename);//this is the method to upload saved file to S3
                    logger.LogInformation("UploadAsinSkuMappingToS3 after uploading file filename = " + filename);
                    if (System.IO.File.Exists(filename))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(filename);

                    }
                    token = Request.Cookies["Token"];

                    UploadFilesToS3ViewModel ViewModel = new UploadFilesToS3ViewModel();
                    ViewModel.FileName = FileNameDate;
                    ViewModel.FilePath = bucketname;
                    ViewModel.JobType = "ApprovedPriceUpdationJob";

                    jobIdReturnView = ApiAccess.SaveJobDetail(ApiURL, token, ViewModel);


                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("UploadAsinSkuMappingToS3 in exception = " + ex);
                throw;
            }

            return Json(new { success = jobIdReturnView.status, JobID = jobIdReturnView.jobid });


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
        public JsonResult GetAllVendorForAutoComplete(string Prefix)
        {
            token = Request.Cookies["Token"];
            List<GetVendorListViewModel> model = _ApiAccess.GetAllVendorByName(ApiURL, token, Prefix);
            if (model == null)
            {
                return Json(model);
            }
            else
            {
                return Json(model);
            }

        }

        public IActionResult GetVendorCurrancy()
        {
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View();
        }

        [HttpPost]
        public int AddMultiApprovedPrice(List<ApprovedPriceViewModel> data)
        {
            token = Request.Cookies["Token"];
            foreach (var item in data)
            {
                int result = _ApiAccess.ApprovedPrice(ApiURL, token, item);
            }
            return 0;
        }


        public IActionResult GetSkuSelectList()
        {
            return PartialView("~/Views/ApprovedPrice/ApprovedSkuListPartialView.cshtml");
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
        public IActionResult AddNotesInApprovedPrice(ApprovedPriceViewModel viewModel)
        {

            token = Request.Cookies["Token"];

            var data = _ApiAccess.AddNotesInApprovedPrice(ApiURL, token, viewModel);

            return RedirectToAction("GetVendorCurrancy", "ApprovedPrice", data);
        }

    }
}