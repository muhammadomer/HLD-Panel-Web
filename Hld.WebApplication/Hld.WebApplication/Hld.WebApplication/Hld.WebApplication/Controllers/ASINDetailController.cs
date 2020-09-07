using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Hld.WebApplication.Enum;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ASINDetailController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string connectionString = "";
        string ZincUserName = "";
        ZincApiAccess _zincApiAccess = null;
        public static IHostingEnvironment _environment;
        // ZincController ctrl = null;
        ZincOrderLogAndDetailApiAccess _zincOrderLogAndDetailApiAccess = null;

        SellerCloudController _sellerCloud = null;
        UploadFilesToS3 uploadFilesToS3 = null;
        EncDecChannel _encDecChannel = null;
        string s3BucketURL = "";
        string s3BucketURL_large = "";
        GetChannelCredViewModel _Zinc = null;

        public ASINDetailController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            connectionString = _configuration.GetValue<string>("ConnectionString:bb2HldNet");
            ZincUserName = _configuration.GetValue<string>("ZincCredential:UserName");
            _zincApiAccess = new ZincApiAccess();
            _zincOrderLogAndDetailApiAccess = new ZincOrderLogAndDetailApiAccess();
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            _encDecChannel = new EncDecChannel();

            _sellerCloud = new SellerCloudController(configuration, environment);
            _sellerCloud.ControllerContext = ControllerContext;

            uploadFilesToS3 = new UploadFilesToS3(environment, configuration);
        }

        public IActionResult Index()
        {
            token = Request.Cookies["Token"];
            List<ASINDetailViewModel> listModel = _zincApiAccess.GetASINProductDetail(ApiURL, token);
            return View("~/Views/ASINDetail/_Index.cshtml", listModel);
        }

        public IActionResult ASINDetailMainView()
        {

            token = Request.Cookies["Token"];
            List<ASINDetailViewModel> listModel = _zincApiAccess.GetASINProductDetail(ApiURL, token);

            return View(listModel);
        }

        public IActionResult ASINDetailMainViewJSONData()
        {
            var querystring = Request.QueryString;
            string dynamicQueryGeneration = "";
            List<ASINDetailViewModel> listModel = null;
            NameValueCollection query = HttpUtility.ParseQueryString(Request.QueryString.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            if (filtersCount > 0)
            {
                dynamicQueryGeneration = BuildQuery(querystring);
                listModel = GetASINListDataDynamicQuery(dynamicQueryGeneration);
                if (listModel != null && listModel.Count > 0)
                {
                    string skuList = JsonConvert.SerializeObject(listModel);
                    foreach (var item in listModel)
                    {
                        item.feature_bullets = "";
                    }
                    HttpContext.Session.SetString("exportData", skuList);
                }
                else
                {

                    listModel = new List<ASINDetailViewModel>();
                    string skuList = JsonConvert.SerializeObject(listModel);
                    HttpContext.Session.SetString("exportData", skuList);
                }
            }
            else
            {
                token = Request.Cookies["Token"];
                listModel = _zincApiAccess.GetASINProductDetail(ApiURL, token);
                if (listModel != null && listModel.Count > 0)
                {
                    string skuList = JsonConvert.SerializeObject(listModel);
                    foreach (var item in listModel)
                    {
                        item.feature_bullets = "";
                    }

                    HttpContext.Session.SetString("exportData", skuList);
                }
                else
                {
                    listModel = new List<ASINDetailViewModel>();
                    string skuList = JsonConvert.SerializeObject(listModel);
                    HttpContext.Session.SetString("exportData", skuList);
                }


            }

            return Json(listModel);
        }

        public IActionResult ASINDetailList(ASINDetailSearchViewModel viewModel)
        {
            token = Request.Cookies["Token"];
            //List<ASINDetailViewModel> list = new List<ASINDetailViewModel>();
            string DateTo = DateTime.Now.ToString("yyyy-MM-dd"); ;
            string DateFrom = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"); ;
            int count = 100;
            if ("0001-01-01" != viewModel.orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                DateTo = viewModel.orderDateTimeTo.ToString("yyyy-MM-dd");
                DateFrom = viewModel.orderDateTimeFrom.ToString("yyyy-MM-dd");
            }
            if ((!string.IsNullOrEmpty(viewModel.ASIN) && viewModel.ASIN != "undefined") || (!string.IsNullOrEmpty(viewModel.Title) && viewModel.Title != "undefined")
                || "0001-01-01" != viewModel.orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {
                count = _zincApiAccess.GetASINProductDetailCount(ApiURL, token, DateTo, DateFrom, viewModel.ASIN, viewModel.Title);
            }
            ViewBag.TotalCount = count;
            //ViewBag.S3BucketURL = s3BucketURL;
            //ViewBag.S3BucketURL_large = s3BucketURL_large;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ASINDetailListPartial(int? page, DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string ASIN, string Title)
        {
            token = Request.Cookies["Token"];

            string DateTo = DateTime.Now.ToString("yyyy-MM-dd"); ;
            string DateFrom = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"); ;

            if ("0001-01-01" != orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                DateTo = orderDateTimeTo.ToString("yyyy-MM-dd");
                DateFrom = orderDateTimeFrom.ToString("yyyy-MM-dd");
            }


            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<ASINDetailViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int offset = 0;
            int limit = pageSize;

            if (page.HasValue)
            {
                offset = (pageNumber - 1) * pageSize;
            }

            List<ASINDetailViewModel> _viewModel = null;
            _viewModel = _zincApiAccess.GetASINProductDetailList(ApiURL, token, DateTo, DateFrom, ASIN, Title, limit, offset);
            data = new StaticPagedList<ASINDetailViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            ViewBag.POMasterID = Convert.ToInt32(Request.Cookies["POMasterID"]);
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/ASINDetail/ASINDetailPartialView.cshtml", data);
        }

        [HttpPost]
        public JsonResult ExportFile(ASINDetailSearchViewModel data)
        {
            string URL = "https://s3.us-east-2.amazonaws.com/amzca.bb.images/";
            token = Request.Cookies["Token"];
            string DateTo = DateTime.Now.ToString("yyyy-MM-dd"); ;
            string DateFrom = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"); ;
            if ("0001-01-01" != data.orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                DateTo = data.orderDateTimeTo.ToString("yyyy-MM-dd");
                DateFrom = data.orderDateTimeFrom.ToString("yyyy-MM-dd");
            }
            List<ASINDetailViewModel> _viewModel = null;
            if (data.list == null)
            {
                _viewModel = _zincApiAccess.GetASINProductDetailList(ApiURL, token, DateTo, DateFrom, data.ASIN, data.Title, data.count, 0);
            }
            else
            {
                _viewModel = data.list;
            }
            List<ASINDetailViewModel> aSINDetailViews = _viewModel;
            List<ASINDetailViewModelForJQwidigts> aSINDetailViewsExport = new List<ASINDetailViewModelForJQwidigts>();
            var stream = new MemoryStream();

            for (int i = 0; i < aSINDetailViews.Count; i++)
            {
                ASINDetailViewModelForJQwidigts model = new ASINDetailViewModelForJQwidigts();
                model.Date = aSINDetailViews[i].asin_date.ToString();
                model.ASIN = aSINDetailViews[i].ASIN;
                model.Title = aSINDetailViews[i].Title;
                model.feature_bullets = aSINDetailViews[i].feature_bullets.Replace(" | ", System.Environment.NewLine.ToString());
                model.AmazonPrice = (Convert.ToDecimal(aSINDetailViews[i].amazon_price) / 100).ToString();
                model.Images = URL + aSINDetailViews[i].MainImage_imgPath + "|" + URL + aSINDetailViews[i].Image1_imgPath + "|" + URL + aSINDetailViews[i].Image2_imgPath + "|" + aSINDetailViews[i].OtherImagesURL;
                aSINDetailViewsExport.Add(model);
            }

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(aSINDetailViewsExport, true);
                package.Save();
            }
            stream.Position = 0;
            byte[] Bytes = stream.ToArray();
            string excelName = $"ASINList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            FileStreamModel _fileStream = new FileStreamModel();
            _fileStream.Content = Bytes;
            _fileStream.ContentLength = Bytes.Length;
            _fileStream.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //return File(stream, "application/octet-stream", excelName);  
            return Json(_fileStream);
        }

        [HttpGet]
        public IActionResult Export()
        {
            string URL = "https://s3.us-east-2.amazonaws.com/amzca.bb.images/";
            var key = HttpContext.Session.GetString("exportData");
            List<ASINDetailViewModel> aSINDetailViews = null;
            List<ASINDetailViewModelForJQwidigts> aSINDetailViewsExport = new List<ASINDetailViewModelForJQwidigts>();
            if (!string.IsNullOrEmpty(key))
            {
                aSINDetailViews = JsonConvert.DeserializeObject<List<ASINDetailViewModel>>(key);
            }
            var stream = new MemoryStream();

            for (int i = 0; i < aSINDetailViews.Count; i++)
            {
                ASINDetailViewModelForJQwidigts model = new ASINDetailViewModelForJQwidigts();
                model.Date = aSINDetailViews[i].asin_date.ToString();
                model.ASIN = aSINDetailViews[i].ASIN;
                model.Title = aSINDetailViews[i].Title;
                model.feature_bullets = aSINDetailViews[i].feature_bullets.Replace(" | ", System.Environment.NewLine.ToString());
                model.AmazonPrice = (Convert.ToDecimal(aSINDetailViews[i].amazon_price) / 100).ToString();
                string[] asinImagesList = null;
                if (aSINDetailViews[i].AmazonImagesListCombined != string.Empty)
                {
                    asinImagesList = aSINDetailViews[i].AmazonImagesListCombined.Split("|");
                    for (int j = 0; j < asinImagesList.Length; j++)
                    {
                        asinImagesList[j] = URL + asinImagesList[j];
                    }
                    model.Images = string.Join("|", asinImagesList);
                }
                else
                {
                    model.Images = URL + aSINDetailViews[i].MainImage_imgPath + "|" + URL + aSINDetailViews[i].Image1_imgPath + "|" + URL + aSINDetailViews[i].Image2_imgPath;
                }
                aSINDetailViewsExport.Add(model);
            }

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(aSINDetailViewsExport, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"ASINList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private string BuildQuery(QueryString queryStringField)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(queryStringField.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            var queryString = @"select asin_complete_detail_id,asin,bbzinc.product_sku,asin_date,title,description,asin_images_other_urls,BBPZ.amazon_price,main_image_path,image_1_path,asin_image_2_bucket_url,asin_image_1_bucket_url,S3BucketImagesList, image_2_path,feature_bullets  from ASINProduct_complete_detail   inner join ASINOfferDetail BBPZ on ASINProduct_complete_detail.asin=BBPZ.z_asin_ca left outer join  BestBuyProductZinc bbzinc on bbzinc.z_asin_ca=BBPZ.z_asin_ca  ";
            var tmpDataField = "";
            var tmpFilterOperator = "";
            var where = "";
            if (filtersCount > 0)
            {
                where = " WHERE (";
            }
            for (var i = 0; i < filtersCount; i += 1)
            {
                var filterValue = query.GetValues("filtervalue" + i)[0];
                var filterCondition = query.GetValues("filtercondition" + i)[0];
                var filterDataField = query.GetValues("filterdatafield" + i)[0];
                var filterOperator = query.GetValues("filteroperator" + i)[0];
                if (tmpDataField == "")
                {
                    tmpDataField = filterDataField;
                }
                else if (tmpDataField != filterDataField)
                {
                    where += ") AND (";
                }
                else if (tmpDataField == filterDataField)
                {
                    if (tmpFilterOperator == "0")
                    {
                        where += " AND ";
                    }
                    else
                    {
                        where += " OR ";
                    }
                }
                // build the "WHERE" clause depending on the filter's condition, value and datafield.
                where += this.GetFilterCondition(filterCondition, filterDataField, filterValue);
                if (i == filtersCount - 1)
                {
                    where += ")";
                }
                tmpFilterOperator = filterOperator;
                tmpDataField = filterDataField;
            }
            queryString += where;
            return queryString;
        }

        private string GetFilterCondition(string filterCondition, string filterDataField, string filterValue)
        {
            switch (filterCondition)
            {
                case "NOT_EMPTY":
                case "NOT_NULL":
                    return " " + filterDataField + " NOT LIKE '" + "" + "'";
                case "EMPTY":
                case "NULL":
                    return " " + filterDataField + " LIKE '" + "" + "'";
                case "CONTAINS_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '%" + filterValue + "%'" + "  ";
                case "CONTAINS":
                    return " " + filterDataField + " LIKE '%" + filterValue + "%'";
                case "DOES_NOT_CONTAIN_CASE_SENSITIVE":
                    return " " + filterDataField + " NOT LIKE '%" + filterValue + "%'" + "  "; ;
                case "DOES_NOT_CONTAIN":
                    return " " + filterDataField + " NOT LIKE '%" + filterValue + "%'";
                case "EQUAL_CASE_SENSITIVE":
                    return " " + filterDataField + " = '" + filterValue + "'" + "  "; ;
                case "EQUAL":
                    return " " + filterDataField + " = '" + filterValue + "'";
                case "NOT_EQUAL_CASE_SENSITIVE":
                    return " BINARY " + filterDataField + " <> '" + filterValue + "'";
                case "NOT_EQUAL":
                    return " " + filterDataField + " <> '" + filterValue + "'";
                case "GREATER_THAN":
                    return " " + filterDataField + " > '" + filterValue + "'";
                case "LESS_THAN":
                    return " " + filterDataField + " < '" + filterValue + "'";
                case "GREATER_THAN_OR_EQUAL":
                    if (filterDataField == "asin_date")
                    {
                        return "convert( " + filterDataField + ",date) >= '" + filterValue + "'";
                    }
                    else
                    {
                        return " " + filterDataField + " >= '" + filterValue + "'";
                    }
                case "LESS_THAN_OR_EQUAL":
                    if (filterDataField == "asin_date")
                    {
                        return "convert( " + filterDataField + ",date) <= '" + filterValue + "'";
                    }
                    else
                    {
                        return " " + filterDataField + " <= '" + filterValue + "'";
                    }

                case "STARTS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '" + filterValue + "%'" + "  ";
                case "STARTS_WITH":
                    return " " + filterDataField + " LIKE '" + filterValue + "%'";
                case "ENDS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '%" + filterValue + "'" + "  ";
                case "ENDS_WITH":
                    return " " + filterDataField + " LIKE '%" + filterValue + "'";
            }
            return "";
        }

        [HttpGet]
        public IActionResult SaveASIN()
        {
            return PartialView("~/Views/ASINDetail/_SaveASIN.cshtml");
        }

        [HttpGet]
        public IActionResult ServerSideProcessing()
        {
            //token = Request.Cookies["Token"];
            //List<ASINDetailViewModel> listModel = _zincApiAccess.GetASINProductDetail(ApiURL, token);
            //listModel
            return View();
        }

        [HttpGet]
        public IActionResult SaveASINDetail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveASIN(string sku)
        {
            token = Request.Cookies["Token"];
            string _productstatus = "";
            string _offerStatus = "";
            string _message = "";
            int count = 0;
            List<string> list = null;
            string URL = "https://s3.us-east-2.amazonaws.com/amzca.bb.images/";
            List<string> SavedAmazonImagesList = new List<string>();


            try
            {


                if (!string.IsNullOrEmpty(sku))
                {
                    count = _zincApiAccess.GetProductASINAlreadyExistsInASINProductDetail(ApiURL, token, sku);
                    list = _zincApiAccess.CheckASINProductImageExist(ApiURL, token, sku);

                    if (count == 0 && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            await uploadFilesToS3.DeleteObjectNonVersionedBucketAsync("amzca.bb.images", item);
                            await uploadFilesToS3.DeleteObjectNonVersionedBucketAsync("amzca.bb.images.thumbnails", item);
                        }
                        _zincApiAccess.DeleteASINProductImageList(ApiURL, token, sku);
                        _zincApiAccess.DeleteBestBuyProductZinc_ByZincID(ApiURL, token, sku);
                    }

                    if (count == 0)
                    {
                        _zincApiAccess.DeleteBestBuyProductZinc_ByZincID(ApiURL, token, sku);


                        ZincProductSaveViewModel ZincOfferDetailmodel = GetASINDetailFromZincinDetail(sku.Trim(), "");

                        ZincASINOfferDetail offerDetail = DataMappingFromASINSaveToOfferModel(ZincOfferDetailmodel);
                        if (offerDetail != null)
                        {
                            ASINDetail.RootObject productASIN = GetProductDetailFromZincinDetail(sku.Trim());

                            _offerStatus = offerDetail.status;
                            if ((productASIN.status != "processing" && productASIN.status != "failed"))
                            {
                                _productstatus = productASIN.status;
                                ASINDetailViewModel asinDetil = ExtractProductDetailThroughASINProductInDetail(productASIN);
                                if (asinDetil != null)
                                {
                                    int status = _zincApiAccess.SaveZincASINOffer(ApiURL, token, offerDetail);
                                    if (status == 1)
                                    {
                                        //string mainImageID = Guid.NewGuid() + "_" + sku + ".jpg";
                                        //string asinImage1_ID = Guid.NewGuid() + ".jpg";
                                        //string asinImage2_ID = Guid.NewGuid() + ".jpg";

                                        if (asinDetil.AmazonImagesList != null)
                                        {
                                            foreach (var item in asinDetil.AmazonImagesList)
                                            {
                                                Image img = _sellerCloud.DownloadImageFromUrl(item);
                                                Stream reduceImageStream = _sellerCloud.GetStreamOfReducedImage(img);
                                                if (reduceImageStream != null)
                                                {
                                                    string keyName = Guid.NewGuid() + "_" + sku + ".jpg";
                                                    Stream downloadStream = _sellerCloud.GetStream(img, ImageFormat.Jpeg);
                                                    var imageStatus = await uploadFilesToS3.uploadASINImagesToS3(downloadStream, keyName);
                                                    await uploadFilesToS3.uploadASINImagesToS3Compressed(reduceImageStream, keyName);

                                                    //disposing stream object
                                                    reduceImageStream.Dispose();
                                                    downloadStream.Dispose();
                                                    if (imageStatus)
                                                    {
                                                        SavedAmazonImagesList.Add(keyName);
                                                        ASINProductImageViewModel model = new ASINProductImageViewModel();
                                                        model.ASIN = sku;
                                                        model.BucketName = "amzca.bb.images & amzca.bb.images.thumbnails";
                                                        model.KeyName = keyName;
                                                        _zincApiAccess.SaveASINProductImageDetail(ApiURL, token, model);
                                                    }
                                                }
                                            }
                                        }

                                        //if (!string.IsNullOrEmpty(asinDetil.AsinMainImage_Url))
                                        //{
                                        //    System.Drawing.Image img = _sellerCloud.DownloadImageFromUrl(asinDetil.AsinMainImage_Url);
                                        //    bool mainImageStatus = await uploadFilesToS3.uploadASINImagesToS3(_sellerCloud.GetStream(img, ImageFormat.Jpeg), mainImageID);

                                        //}
                                        //if (!string.IsNullOrEmpty(asinDetil.AsinImage1_Url))
                                        //{                                        
                                        //    Image asinImage1 = _sellerCloud.DownloadImageFromUrl(asinDetil.AsinImage1_Url);
                                        //    bool image_1_status = await uploadFilesToS3.uploadASINImagesToS3(_sellerCloud.GetStream(asinImage1, ImageFormat.Jpeg), asinImage1_ID);
                                        //}
                                        //if (!string.IsNullOrEmpty(asinDetil.AsinImage2_Url))
                                        //{
                                        //    Image asinImage2 = _sellerCloud.DownloadImageFromUrl(asinDetil.AsinImage2_Url);
                                        //    bool image_2_status = await uploadFilesToS3.uploadASINImagesToS3(_sellerCloud.GetStream(asinImage2, ImageFormat.Jpeg), asinImage2_ID);
                                        //}

                                        if (SavedAmazonImagesList.ElementAtOrDefault(0) != null)
                                        {
                                            asinDetil.MainImage_imgPath = SavedAmazonImagesList[0].ToString();

                                        }

                                        if (SavedAmazonImagesList.ElementAtOrDefault(1) != null)
                                        {
                                            asinDetil.Image1_imgPath = SavedAmazonImagesList[1].ToString(); ;

                                        }
                                        if (SavedAmazonImagesList.ElementAtOrDefault(2) != null)
                                        {
                                            asinDetil.Image2_imgPath = SavedAmazonImagesList[2].ToString(); ;
                                        }
                                        if (SavedAmazonImagesList.ElementAtOrDefault(3) != null)
                                        {
                                            asinDetil.S3BucketULR_image1 = SavedAmazonImagesList[3].ToString(); ;
                                        }
                                        if (SavedAmazonImagesList.ElementAtOrDefault(4) != null)
                                        {
                                            asinDetil.S3BucketURL_image2 = SavedAmazonImagesList[4].ToString(); ;
                                        }
                                        if (SavedAmazonImagesList.Count > 0)
                                        {
                                            asinDetil.AmazonImagesListCombined = string.Join("|", SavedAmazonImagesList);
                                        }


                                        if (asinDetil != null)
                                        {
                                            _zincApiAccess.SaveASINProductDetail(ApiURL, token, asinDetil);
                                        }
                                    }
                                    else if (status == 2)
                                    {
                                        _message = "Can't insert duplicate ASIN " + sku;
                                    }
                                }

                            }
                            else
                            {
                                _productstatus = "Error";
                            }
                        }
                        else
                        {
                            _productstatus = "Error";
                            _offerStatus = "Error";
                        }
                    }
                    else
                    {
                        _message = sku + " Already Exist in database";
                    }
                }
            }
            catch (Exception ex)
            {
                _productstatus = "Error";
                _offerStatus = "Error";

            }
            return Json(new { offerstatus = _offerStatus, productstatus = _productstatus, asin = sku, message = _message });
        }


        private List<ASINDetailViewModel> GetASINListDataDynamicQuery(string query)
        {

            List<ASINDetailViewModel> listViewModel = null;
            try
            {
                //using (MySqlConnection conn = new MySqlConnection(connectionString))
                //{
                //    conn.Open();
                //    MySqlCommand cmd = new MySqlCommand(query, conn);

                //    cmd.CommandType = System.Data.CommandType.Text;

                //    MySqlDataReader reader = cmd.ExecuteReader();
                //    if (reader.HasRows)
                //    {
                //        listViewModel = new List<ASINDetailViewModel>();
                //        while (reader.Read())
                //        {

                //            ASINDetailViewModel model = new ASINDetailViewModel();
                //            model.asin_date = Convert.ToDateTime(reader["asin_date"] != DBNull.Value ? reader["asin_date"] : DateTime.Now);
                //            model.ASIN = Convert.ToString(reader["asin"] != DBNull.Value ? reader["asin"] : "0");
                //            model.Title = Convert.ToString(reader["title"] != DBNull.Value ? reader["title"] : "").Replace("\n", " , ").Replace("[", " ").Replace("]", " ");
                //            model.feature_bullets = Convert.ToString(reader["feature_bullets"] != DBNull.Value ? reader["feature_bullets"] : "");
                //            model.OtherImagesURL = Convert.ToString(reader["asin_images_other_urls"] != DBNull.Value ? reader["asin_images_other_urls"] : "");
                //            //model.Description = model.Description.Replace("\n", " , ").Replace("["," ").Replace("]"," ");
                //            model.amazon_price = Convert.ToString(reader["amazon_price"] != DBNull.Value ? reader["amazon_price"].ToString() : "0");
                //            model.Image1_imgPath = Convert.ToString(reader["image_1_path"] != DBNull.Value ? reader["image_1_path"] : "");
                //            model.Image2_imgPath = Convert.ToString(reader["image_2_path"] != DBNull.Value ? reader["image_2_path"] : "");
                //            model.MainImage_imgPath = Convert.ToString(reader["main_image_path"] != DBNull.Value ? reader["main_image_path"] : "");
                //            model.S3BucketULR_image1 = Convert.ToString(reader["asin_image_1_bucket_url"] != DBNull.Value ? reader["asin_image_1_bucket_url"] : "");
                //            model.S3BucketURL_image2 = Convert.ToString(reader["asin_image_2_bucket_url"] != DBNull.Value ? reader["asin_image_2_bucket_url"] : "");
                //            model.AmazonImagesListCombined = Convert.ToString(reader["S3BucketImagesList"] != DBNull.Value ? reader["S3BucketImagesList"] : "");
                //            model.AsinProductDetailID = Convert.ToInt32(reader["asin_complete_detail_id"] != DBNull.Value ? reader["asin_complete_detail_id"] : "0");
                //            model.product_sku = Convert.ToString(reader["product_sku"] != DBNull.Value ? reader["product_sku"] : "");


                //            listViewModel.Add(model);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
            }
            return listViewModel;
        }

        private ZincASINOfferDetail DataMappingFromASINSaveToOfferModel(ZincProductSaveViewModel zincProductModel)
        {
            ZincASINOfferDetail model = null;
            if (zincProductModel != null)
            {
                model = new ZincASINOfferDetail();
                model.ASIN = zincProductModel.ASIN;
                model.bb_product_zinc_id = zincProductModel.bb_product_zinc_id;
                model.delivery_days_max = zincProductModel.delivery_days_max;
                model.delivery_days_min = zincProductModel.delivery_days_min;
                model.handlingday_max = zincProductModel.handlingday_max;
                model.handlingday_min = zincProductModel.handlingday_min;
                model.itemavailable = zincProductModel.itemavailable;
                model.itemprice = Convert.ToInt32(zincProductModel.itemprice);
                model.item_condition = zincProductModel.item_condition;
                model.item_prime_badge = zincProductModel.item_prime_badge;
                model.max_price_limit = zincProductModel.max_price_limit;
                model.percent_positive = zincProductModel.percent_positive;
                model.primeAvailable = zincProductModel.primeAvailable;
                model.Priorty = zincProductModel.Priorty;
                model.Product_sku = zincProductModel.Product_sku;
                model.sellerName = zincProductModel.sellerName;
                model.status = zincProductModel.status;
                model.timestemp = zincProductModel.timestemp;
                model.updateDate = zincProductModel.updateDate;
            }
            return model;
        }

        public ZincProductSaveViewModel GetASINDetailFromZincinDetail(string orderid, string ProductSKU)
        {

            ZincProductSaveViewModel zincProductSaveViewModel = null;
            int? minPriceOfOffer = null;
            string offerID = "";
            try
            {
                token = Request.Cookies["Token"];
                _Zinc = new GetChannelCredViewModel();
                _Zinc = _encDecChannel.DecryptedData(ApiURL, token, "Zinc");
                ZincUserName = _Zinc.Key;
                string uri = " https://api.zinc.io/v1/products/" + orderid.Trim() + "/offers?retailer=amazon_ca";
                string response = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Credentials = new NetworkCredential(ZincUserName, "");
                using (var webResponse = request.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        response = new StreamReader(responseStream).ReadToEnd();
                    }
                }

                ZincProductOfferViewModel.RootObject model = JsonConvert.DeserializeObject<ZincProductOfferViewModel.RootObject>(response);
                if ((model.status != "processing" || model.status != "failed") && model.offers != null && model.offers.Count > 0)
                {
                    // getting all those offers which have fulfilled true
                    var offerids = model.offers.Where(e => e.marketplace_fulfilled.Equals(true)).Select(
                        e => new
                        {
                            offerid = e.offer_id,
                            offerPrice = e.price
                        }).ToList();

                    // based on offerid's list getting minimum price and then select offer from offer's list

                    if (offerids != null && offerids.Count > 0)
                    {
                        minPriceOfOffer = offerids.Min(e => e.offerPrice);
                        offerID = offerids.Where(e => e.offerPrice.Value == minPriceOfOffer.Value).Select(e => e.offerid).FirstOrDefault();
                    }
                    var models = model.offers.Where(e => e.offer_id == offerID).ToList();

                    if (models != null && models.Count > 0)
                    {
                        zincProductSaveViewModel = new ZincProductSaveViewModel();
                        zincProductSaveViewModel.timestemp = model.timestamp.HasValue ? model.timestamp.Value : 0;
                        zincProductSaveViewModel.status = model.status;
                        zincProductSaveViewModel.ASIN = orderid;
                        zincProductSaveViewModel.Product_sku = ProductSKU;
                        var status = model.status;
                        foreach (var item in models)
                        {
                            zincProductSaveViewModel.sellerName = item.seller.name;
                            zincProductSaveViewModel.percent_positive = item.seller.percent_positive.HasValue ? item.seller.percent_positive.Value : 0;
                            zincProductSaveViewModel.itemprice = item.price.HasValue ? item.price.Value : 0;
                            zincProductSaveViewModel.itemavailable = item.available;
                            zincProductSaveViewModel.handlingday_min = item.handling_days.min.HasValue ? item.handling_days.min.Value : 0;
                            zincProductSaveViewModel.handlingday_max = item.handling_days.max.HasValue ? item.handling_days.max.Value : 0;
                            zincProductSaveViewModel.item_prime_badge = item.prime_badge;

                            foreach (var shippingOption in item.shipping_options)
                            {
                                if (shippingOption.delivery_days != null)
                                {
                                    zincProductSaveViewModel.delivery_days_max = shippingOption.delivery_days.max.HasValue ? shippingOption.delivery_days.max.Value : 0;
                                    zincProductSaveViewModel.delivery_days_min = shippingOption.delivery_days.min.HasValue ? shippingOption.delivery_days.min.Value : 0;
                                }
                            }
                            zincProductSaveViewModel.item_condition = item.condition;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                return zincProductSaveViewModel;
            }
            return zincProductSaveViewModel;
        }

        public ASINDetail.RootObject GetProductDetailFromZincinDetail(string ASIN)
        {
            ASINDetail.RootObject model = null;
            try
            {
                token = Request.Cookies["Token"];
                _Zinc = new GetChannelCredViewModel();
                _Zinc = _encDecChannel.DecryptedData(ApiURL, token, "Zinc");
                ZincUserName = _Zinc.Key;

                string uri = " https://api.zinc.io/v1/products/" + ASIN.Trim() + "?retailer=amazon_ca";
                string response = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Credentials = new NetworkCredential(ZincUserName, "");
                using (var webResponse = request.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        model = new ASINDetail.RootObject();
                        response = new StreamReader(responseStream).ReadToEnd();
                    }
                }
                model = JsonConvert.DeserializeObject<ASINDetail.RootObject>(response);

                if (model.status == "processing" || model.status == "failed")
                {
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
            return model;
        }

        public ASINDetailViewModel ExtractProductDetailThroughASINProductInDetail(ASINDetail.RootObject rootObject)
        {
            ASINDetailViewModel model = null;
            try
            {
                if (rootObject != null)
                {
                    model = new ASINDetailViewModel();
                    List<string> featureList = new List<string>();

                    model.ASIN = rootObject.asin;
                    if (rootObject.feature_bullets != null)
                    {

                        model.feature_bullets = string.Join(" | ", rootObject.feature_bullets);
                    }
                    model.Title = rootObject.title;
                    model.Description = rootObject.product_description;
                    if (rootObject.images != null)
                    {
                        model.OtherImagesURL = string.Join("|", rootObject.images);

                        model.AmazonImagesList = rootObject.images;
                        model.AsinMainImage_Url = rootObject.main_image;
                        model.AsinImage1_Url = rootObject.images.Select((item, index) => new { item, index }).FirstOrDefault(e => e.index == 1).item;
                        model.AsinImage2_Url = rootObject.images.Select((item, index) => new { item, index }).FirstOrDefault(e => e.index == 2).item;
                    }
                    model.BrandName = rootObject.brand;
                    if (rootObject.product_details != null)
                    {
                        if (rootObject.product_details.Contains("Colour"))
                        {
                            model.Color = rootObject.product_details[4].ToString();
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {

                return model;
            }
        }

    }
}