using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    public class BestBuyUpdateLogs : Controller
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
        BestBuyUpdatelogsApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private readonly IConfiguration _configuration;
        public BestBuyUpdateLogs(IConfiguration configuration, IHostingEnvironment environment, ILogger<BestBuyUpdateLogs> _logger)
        {
            _hostingEnvironment = environment;
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            this.logger = _logger;
            _ApiAccess = new BestBuyUpdatelogsApiAccess();
        }

        public IActionResult Index(int JobId)
        {
            token = Request.Cookies["Token"];
            var count = _ApiAccess.GetCounter(ApiURL, token,JobId);
            ViewBag.logsRecords = count;
            return View();
        }

        [HttpPost]
        public IActionResult getlist(int JobId,int? page )
        {

            token = Request.Cookies["Token"];
            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<BestBuyUpdateLogsViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }

            List<BestBuyUpdateLogsViewModel> _viewModel = null;
            _viewModel = _ApiAccess.GetShipmentList(ApiURL, token, JobId ,startlimit, endLimit);
            data = new StaticPagedList<BestBuyUpdateLogsViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/BestBuyUpdateLogs/BustBuuPartialView.cshtml", data);
        }

    }
}
