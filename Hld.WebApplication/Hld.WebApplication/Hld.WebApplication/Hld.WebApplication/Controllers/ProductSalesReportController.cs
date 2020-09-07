using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ProductSalesReportController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";


        ProductReportApiAccess _bApiAccess = null;
     
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        public ProductSalesReportController(IConfiguration configuration)
        {
            _configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _bApiAccess = new ProductReportApiAccess();
        
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PanelReport()
        {
            //token = Request.Cookies["Token"];
            //List<ProductSalesViewModel> productSalesViewModel = new List<ProductSalesViewModel>();

            //productSalesViewModel = _bApiAccess.GetProductSales(ApiURL, token);
            return View();
        }
        public List<ProductSalesViewModel> PanelReportDatatable(DTParameters dTModel)
        {
            token = Request.Cookies["Token"];
            List<ProductSalesViewModel> productSalesViewModel = new List<ProductSalesViewModel>();

            productSalesViewModel = _bApiAccess.GetProductSales(ApiURL, token);
            return productSalesViewModel;
        }
    }
}