using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ExportController : Controller
    {
            private readonly IConfiguration _configuration;
            ExportImgUrlApiAccess _exportapiaccess = new ExportImgUrlApiAccess();
            public string ApiURL { get; private set; }
            public static IHostingEnvironment _environment;
            string s3BucketURL = "";
            string s3BucketURL_large = "";
            public ExportController(IConfiguration configuration, IHostingEnvironment environment)
            {
                _environment = environment;
                this._configuration = configuration;
                ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
                s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            }
            public IActionResult ExportSkuUrlImgUrl()
            {
                return View();
            }
            [HttpGet]
            public JsonResult ExportskuUrlImg()
            {
                string token = Request.Cookies["Token"];
                List<ExportSkuImgUrlViewModel> listmodel = new List<ExportSkuImgUrlViewModel>();
                listmodel = _exportapiaccess.ExportSkuImgUrl(ApiURL, token);
                string folder = _environment.WebRootPath;
                string excelName = $"Product_ImageURL.xlsx";
                string downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
                FileInfo file = new FileInfo(Path.GetTempPath() + excelName);
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.GetTempPath() + excelName);
                }
                using (var package = new ExcelPackage(file))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(listmodel, true);
                    package.Save();
                }
                return Json(new { filePath = Path.GetTempPath(), fileName = excelName });
            }
            public FileResult downloadFileSku(string filePath, string fileName, string file)
            {
                IFileProvider provider = new PhysicalFileProvider(filePath);
                IFileInfo fileInfo = provider.GetFileInfo(fileName);
                var readStream = fileInfo.CreateReadStream();
                var mimeType = "application/vnd.ms-excel";
                return File(readStream, mimeType, file);
            }
        
    }
}