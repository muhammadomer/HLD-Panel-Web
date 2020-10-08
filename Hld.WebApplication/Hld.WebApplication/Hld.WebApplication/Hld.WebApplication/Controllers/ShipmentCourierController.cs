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
    public class ShipmentCourierController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        string ApiURL = "";
        string token = "";
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        ShipmentCourierApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;

        private readonly IConfiguration _configuration;
        public ShipmentCourierController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ApprovedPriceController> _logger)
        {
            _hostingEnvironment = environment;
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            this.logger = _logger;
            _ApiAccess = new ShipmentCourierApiAccess();
        }
        public IActionResult Index(int id)
        {
            string token = Request.Cookies["Token"];
            SaveAndEditShipmentCourierVM ViewModel = new SaveAndEditShipmentCourierVM();
            ViewModel = _ApiAccess.EditShipmentCourier(ApiURL, token, id);
            return View(ViewModel);
            
        }
        [HttpPost]
        public IActionResult Index(SaveAndEditShipmentCourierVM viewmodel)
        {
            string token = Request.Cookies["Token"];
            if (viewmodel.ShipmentCourier_ID > 0)
            {

                _ApiAccess.SaveCourier(ApiURL, token, viewmodel);

            }
            else
            {
                _ApiAccess.SaveCourier(ApiURL, token, viewmodel);
            }
            return RedirectToAction("CourierDetail", "ShipmentCourier", viewmodel);
        
          
        }
        public IActionResult CourierDetail()
        {
            string token = Request.Cookies["Token"];
            List<SaveAndEditShipmentCourierVM> listmodel = new List<SaveAndEditShipmentCourierVM>();
            listmodel = _ApiAccess.CourierDetail(ApiURL, token);
            return View(listmodel);
        }
        public List<SaveAndEditShipmentCourierVM> CourierDetailGet()
        {
            string token = Request.Cookies["Token"];
            List<SaveAndEditShipmentCourierVM> listmodel = new List<SaveAndEditShipmentCourierVM>();
            listmodel = _ApiAccess.CourierDetail(ApiURL, token);
            return listmodel;
        }
    }
}
