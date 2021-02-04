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

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class BulkUpdateController : Controller
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
        BulkUpdateApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        
        private readonly IConfiguration _configuration;
        public BulkUpdateController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ApprovedPriceController> _logger)
        {
            _hostingEnvironment = environment;


            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            
            this.logger = _logger;
            _ApiAccess = new BulkUpdateApiAccess();
        }
        public IActionResult Index()
        {
            return View();
        }

        //public List<BulkUpdateViewModel> BulkUpdate(List<BulkUpdateViewModel> data)
        //{
        //    token = Request.Cookies["Token"];
        //    List<BulkUpdateViewModel> viewlList = new List<BulkUpdateViewModel>();
        //    try
        //    {
        //        viewlList = _ApiAccess.GetBulkUpdate(ApiURL, token,data);              
        //        return viewlList;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
    }
}
