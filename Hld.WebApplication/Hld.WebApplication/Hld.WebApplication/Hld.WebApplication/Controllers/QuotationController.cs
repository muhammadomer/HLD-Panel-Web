using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Hld.WebApplication.Controllers
{
    public class QuotationController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        string ApiURL = "";
        string token = "";   
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        QuotationApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        
        private readonly IConfiguration _configuration;
        public QuotationController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ApprovedPriceController> _logger)
        {
            _hostingEnvironment = environment; 
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            this.logger = _logger;
            _ApiAccess = new QuotationApiAccess();
        }
        public IActionResult Index(int id)
        {
            string token = Request.Cookies["Token"];
            SaveQuotationMainVM ViewModel = new SaveQuotationMainVM();
            ViewModel = _ApiAccess.EditQuotation(ApiURL, token, id);
            return View(ViewModel);
        }
        [HttpPost]
        public IActionResult Index(SaveQuotationMainVM viewmodel)
        {
                    string token = Request.Cookies["Token"];
            if (viewmodel.Quotation_main_id > 0)
            {
                
                _ApiAccess.SaveQuotaion(ApiURL, token, viewmodel);

            }
            else
            {
                _ApiAccess.SaveQuotaion(ApiURL, token, viewmodel);

            }
                    return RedirectToAction("QuotaionDetail", "Quotation", viewmodel);
        }
        public IActionResult QuotaionDetail()
        {
            string token = Request.Cookies["Token"];
            List<SaveQuotationMainVM> listmodel = new List<SaveQuotationMainVM>();
         listmodel = _ApiAccess.QuotaionDetail(ApiURL, token);
            return View(listmodel);
        }

        [HttpPost]
        public string GenrateSKU()
        {
            token = Request.Cookies["Token"];
            var sku = _ApiAccess.GenrateSKU(ApiURL, token);

            return sku;
        }
       
        public IActionResult DeleteQuotaion(int Quotation_main_id)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            status=_ApiAccess.DeleteQuotaion(ApiURL, token, Quotation_main_id);
            return RedirectToAction("QuotaionDetail", "Quotation",status);
        }

        public IActionResult Uploadimage()
        {
           
            return View();
        }


        //public ActionResult Edit(int id)
        //{
        //    string token = Request.Cookies["Token"];
        //    SaveQuotationMainVM ViewModel = new SaveQuotationMainVM();
        //    ViewModel = _ApiAccess.EditQuotation(ApiURL, token, id);
        //    return View (ViewModel);  

        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {


        //        return RedirectToAction(nameof(QuotaionDetail));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
