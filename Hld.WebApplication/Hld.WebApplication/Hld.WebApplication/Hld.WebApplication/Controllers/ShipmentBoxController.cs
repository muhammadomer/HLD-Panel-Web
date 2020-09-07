using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using BarcodeLib;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ShipmentBoxController : Controller
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
        ShipmentBoxApiAccess _ApiAccess = null;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        readonly UploadFilesToS3ForJobsAPIAccess ApiAccess = null;
        private readonly IConfiguration _configuration;
        public ShipmentBoxController(IConfiguration configuration, IHostingEnvironment environment, ILogger<ShipmentBoxController> _logger)
        {
            _hostingEnvironment = environment;
            this._configuration = configuration;
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ApiAccess = new UploadFilesToS3ForJobsAPIAccess();
            this.logger = _logger;
            _ApiAccess = new ShipmentBoxApiAccess();
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(string ShipmentId)
        {
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            token = Request.Cookies["Token"];
            ShipmentHeaderViewModel viewModel = new ShipmentHeaderViewModel();
            viewModel = _ApiAccess.GetListbyShipemntId(ApiURL, token, ShipmentId);
            return View(viewModel);
        }

        [HttpPost]
        public int Update(List<ShipmentBoxViewModel> data)
        {
            token = Request.Cookies["Token"];
            foreach (var item in data)
            {
                bool status = _ApiAccess.Update(ApiURL, token, item);
            }
            return 0;
        }
        [HttpPost]
        public int UpdateBox(ShipmentBoxViewModel data)
        {
            token = Request.Cookies["Token"];
            bool status = _ApiAccess.Update(ApiURL, token, data);
            return 0;
        }

        [HttpGet]
        public string CreateBox(string Id)
        {
            token = Request.Cookies["Token"];
            ShipmentBoxViewModel viewModel = new ShipmentBoxViewModel();
            viewModel.ShipmentId = Id;
            string BoxId = _ApiAccess.Create(ApiURL, token, viewModel);
            return BoxId;
        }
        [HttpPost]
        public int Delete(List<ShipmentBoxViewModel> data)
        {
            token = Request.Cookies["Token"];
            foreach (var item in data)
            {
                bool status = _ApiAccess.Delete(ApiURL, token, item.BoxId);
            }
            return 0;
        }

        public FileStreamModel DownloadBoxLabels(List<string> data)
        {
            System.Drawing.Font font = new System.Drawing.Font("HELVETICA", 14f);
            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = false,
                Alignment = AlignmentPositions.CENTER,
                Width = 300,
                Height = 100,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
                LabelFont = font
            };
            string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";
            string dateTime = DateTime.Today.ToString("yyyy-MM-dd");
            var pgSize = new iTextSharp.text.Rectangle(384, 288);
            Document doc = new Document(pgSize);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Path.GetTempPath() + filename, FileMode.Create));
            doc.Open();
            PdfContentByte pdfContentByte = writer.DirectContent;
            foreach (var item in data)
            {
                doc.NewPage();
                System.Drawing.Image img = barcode.Encode(TYPE.CODE128, item);
                System.Drawing.Image image = img; //Here I passed a bitmap
                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(image,
                System.Drawing.Imaging.ImageFormat.Jpeg);
                BaseFont heading_font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                pdfContentByte.BeginText();
                pdfContentByte.SetFontAndSize(heading_font, 10f);
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, dateTime, pgSize.Right - 200, pgSize.GetTop(Utilities.MillimetersToPoints(12)), 0);
                pdfContentByte.SetFontAndSize(heading_font, 14f);
                pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, item, pgSize.Right - 200, pgSize.GetTop(Utilities.MillimetersToPoints(52)), 0);
                pdfContentByte.EndText();
                pdfContentByte.Stroke();
                doc.Add(pdfImage);
            }
            doc.Close();
            FileStreamModel _fileStream = new FileStreamModel();
            //using (FileStream _file = new FileStream(Path.GetTempPath() + filename, FileMode.Open))
            //{
            //    var memoryStream = new MemoryStream();
            //    _file.CopyTo(memoryStream);
            //    var dataBytes = memoryStream.ToArray();
            byte[] dataBytes = System.IO.File.ReadAllBytes(Path.GetTempPath() + filename);

            _fileStream.Content = dataBytes;
            _fileStream.ContentLength = dataBytes.Length;
            _fileStream.ContentType = "application/octet-stream";
            //}

            if ((System.IO.File.Exists(Path.GetTempPath() + filename))) { System.IO.File.Delete(Path.GetTempPath() + filename); }
            return _fileStream;
        }


    }
}