using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hld.WebApplication.ViewModel;
using Microsoft.Extensions.Configuration;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ServiceReference1;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ImageMagick;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    [Authorize(Policy = "Access to BulkUpdate Tab")]
    public class MissingSKUManagementController : Controller
    {

        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        SellerCloudApiAccess sellerCloudApiAccess = null;
        UploadFilesToS3 uploadFiles = null;
        ProductApiAccess productApiAccess = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        ServiceReference1.AuthHeader authHeader = null;
        OrderNotesAPiAccess _OrderApiAccess = null;
        string s3BucketURL = "";
        string SCRestURL = "";
        string s3BucketURL_large = "";

        public static IHostingEnvironment _environment;
        public MissingSKUManagementController(IConfiguration configuration, IHostingEnvironment environment)
        {

            _environment = environment;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            productApiAccess = new ProductApiAccess();
            _encDecChannel = new EncDecChannel();
            _OrderApiAccess = new OrderNotesAPiAccess();
            sellerCloudApiAccess = new SellerCloudApiAccess();
            uploadFiles = new UploadFilesToS3(_environment, _configuration);
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddMissingSKUFromSellerCloud()
        {
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View();
        }

        [HttpPost]
        public IActionResult AddMissingSKUFromSellerCloud(MissingSkuViewModel viewModel)
        {
            //var strKey = HttpContext.Session.GetString("kk");             
            //var obj = JsonConvert.DeserializeObject<string>(str);

            token = Request.Cookies["Token"];
            int missingSkuCounter = 0; int existSKUCounter = 0;
            List<ProductDisplayViewModel> ExistingProductList = new List<ProductDisplayViewModel>();
            List<string> missingSkuList = new List<string>();
            string newlist = viewModel.Sku;
            var lines = newlist.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Trim() != string.Empty)
                {
                    int productID = productApiAccess.GetProductIDBySKU(ApiURL, token, line.Trim());
                    if (productID > 0)
                    {
                        ProductDisplayViewModel model = productApiAccess.GetProductDetailBySKU(ApiURL, token, line.Trim());
                        ExistingProductList.Add(model);
                        existSKUCounter = existSKUCounter + 1;
                    }
                    else
                    {
                        missingSkuList.Add(line.Trim());
                        missingSkuCounter = missingSkuCounter + 1;
                    }
                }
            }
            var sessionMissingSku = JsonConvert.SerializeObject(missingSkuList);
            HttpContext.Session.SetString("MissingSku", sessionMissingSku);
            ViewBag.missingSkuCounter = missingSkuCounter;
            ViewBag.existSKUCounter = existSKUCounter;
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            ViewData["existingSku"] = ExistingProductList;

            return View(viewModel);
        }

        //public async Task<IActionResult> AddMissingSkuListFromSession()
        //{
        //    var key = HttpContext.Session.GetString("MissingSku");
        //    token = Request.Cookies["Token"];
        //    int addedCounter = 0;
        //    int errorCounter = 0;
        //    int noResultCounter = 0;
        //    List<string> missingSkuList = JsonConvert.DeserializeObject<List<string>>(key);
        //    foreach (var item in missingSkuList)
        //    {

        //        try
        //        {
        //            _Selller = new GetChannelCredViewModel();
        //            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
        //            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
        //            var product = productApiAccess.GetProductInfoFromSellerCloudForMIssingSku(SCRestURL, authenticate.access_token, item.Trim());

        //            //ProductInsetUpdateViewModel model = await GetProductInfoFromSellerCloudForMIssingSku(item.Trim());
        //            if (product == null)
        //            {
        //                noResultCounter = noResultCounter + 1;
        //            }
        //            else
        //            {
        //                product.SKU = item.Trim();
        //                productApiAccess.AddProductnew(ApiURL, token, product);
        //                addedCounter = addedCounter + 1;
        //                var imageURL = product.ImageUrl;
        //                if (imageURL != null && imageURL != "")
        //                {
        //                    Image img = DownloadImageFromUrl(imageURL);
        //                    // save seller cloud order images to prorduct_images table                  
        //                    ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
        //                    try
        //                    {
        //                        if (img != null)
        //                        {

        //                            string fileName = Guid.NewGuid().ToString() + "-" + item.Trim() + Path.GetExtension(imageURL);
        //                            databaseImagesURL.product_Sku = item.Trim();
        //                            databaseImagesURL.FileName = fileName;
        //                            databaseImagesURL.ImageURL = imageURL;
        //                            databaseImagesURL.isImageExistInSC = true;
        //                            if (sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL))
        //                            {
        //                                await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);

        //                                await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);
        //                            }
        //                            //ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, sku.Trim(), true);
        //                            //_successCounter++;                            
        //                        }

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            errorCounter = errorCounter + 1;
        //            continue;
        //        }
        //    }
        //    return Json(new { AddedSku = addedCounter, ErrorSku = errorCounter, NoResult = noResultCounter });
        //}

        public async Task<IActionResult> AddMissingSkuListFromSession()
        {
            int missingSkuCounter = 0;
            int existSKUCounter = 0;
            var key = HttpContext.Session.GetString("MissingSku");
            token = Request.Cookies["Token"];
            int addedCounter = 0;

            List<ProductDisplayViewModel> ExistingProductList = new List<ProductDisplayViewModel>();
            int errorCounter = 0;
            int noResultCounter = 0;
            List<string> missingSkuList = JsonConvert.DeserializeObject<List<string>>(key);
            foreach (var item in missingSkuList)
            {

                try
                {
                    _Selller = new GetChannelCredViewModel();
                    _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                    AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
                    var product = productApiAccess.GetProductInfoFromSellerCloudForMIssingSku(SCRestURL, authenticate.access_token, item.Trim());

                    //ProductInsetUpdateViewModel model = await GetProductInfoFromSellerCloudForMIssingSku(item.Trim());
                    if (product == null)
                    {
                        noResultCounter = noResultCounter + 1;
                    }
                    else
                    {
                        product.SKU = item.Trim();
                        productApiAccess.AddProductnew(ApiURL, token, product);
                        addedCounter = addedCounter + 1;
                        var imageURL = product.ImageUrl;
                        if (imageURL != null && imageURL != "")
                        {
                            Image img = DownloadImageFromUrl(imageURL);
                            // save seller cloud order images to prorduct_images table
                            ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                            try
                            {
                                if (img != null)
                                {

                                    string fileName = Guid.NewGuid().ToString() + "-" + item.Trim() + Path.GetExtension(imageURL);
                                    databaseImagesURL.product_Sku = item.Trim();
                                    databaseImagesURL.FileName = fileName;
                                    databaseImagesURL.ImageURL = imageURL;
                                    databaseImagesURL.isImageExistInSC = true;
                                    if (sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL))
                                    {
                                        await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);

                                        await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);
                                    }
                                    //ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, sku.Trim(), true);
                                    //_successCounter++;
                                }

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorCounter = errorCounter + 1;
                    continue;
                }
            }
            foreach (var line in missingSkuList)
            {
                if (line.Trim() != string.Empty)
                {
                    int productID = productApiAccess.GetProductIDBySKU(ApiURL, token, line.Trim());
                    if (productID > 0)
                    {
                        ProductDisplayViewModel model = productApiAccess.GetProductDetailBySKU(ApiURL, token, line.Trim());
                        ExistingProductList.Add(model);
                        existSKUCounter = existSKUCounter + 1;
                    }
                    else
                    {
                        missingSkuList.Add(line.Trim());
                        missingSkuCounter = missingSkuCounter + 1;
                    }
                }
            }


            return Json(new { AddedSku = addedCounter, ErrorSku = errorCounter, NoResult = noResultCounter, viewmodel = ExistingProductList });
        }

        public Stream GetStream(Image img, ImageFormat format)
        {
            var ms = new MemoryStream();
            img.Save(ms, format);
            return ms;
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
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

        public Stream GetStreamOfReducedImage(Image inputPath)
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



        public System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                Uri ImageUri = new Uri(imageUrl, UriKind.Absolute);
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ImageUri);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }

        public async Task<ProductInsetUpdateViewModel> GetProductInfoFromSellerCloudForMIssingSku(string productSku)
        {
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            authHeader = new AuthHeader();
            authHeader.ValidateDeviceID = false;
            authHeader.UserName = _Selller.UserName;
            authHeader.Password = _Selller.Key;

            ServiceReference1.SCServiceSoapClient sCServiceSoap =
                      new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);
            var request = await sCServiceSoap.GetProductInfoAsync(authHeader, null, productSku);
            ProductInfo productInfo = request.GetProductInfoResult;
            ProductInsetUpdateViewModel model = new ProductInsetUpdateViewModel();
            model.ProductTitle = productInfo.ProductName;
            model.Upc = productInfo.UPC;
            model.ProductSKU = productInfo.ID;
            return model;
        }
    }
}