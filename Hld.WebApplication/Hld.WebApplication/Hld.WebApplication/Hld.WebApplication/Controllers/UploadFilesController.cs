using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    [Authorize(Policy = "Access to BulkUpdate Tab")]
    public class UploadFilesController : Controller
    {
        ProductApiAccess ProductApiAccess = null;
        public static IHostingEnvironment _environment;
        public static IConfiguration _configuration;
        FileUploadApiAccess fileUploadApiAccess = null;
        UploadFilesToS3 uploadFiles = null;
        string ApiURL = "";
        string token = "";
        public UploadFilesController(IConfiguration configuration, IHostingEnvironment environment)
        {

            _environment = environment;
            _configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            uploadFiles = new UploadFilesToS3(_environment, _configuration);
            ProductApiAccess = new ProductApiAccess();
            fileUploadApiAccess = new FileUploadApiAccess();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadExcelFile()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadExcelFileOfASIN()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> UploadExcelFile(IFormFile files)
        {
            token = Request.Cookies["Token"];
            int _successCounter = 0;
            int _failureCounter = 0;
            string status = "";
            bool fileStatus = false;
            List<ProductDisplayViewModel> model = new List<ProductDisplayViewModel>();
            using (var stream = new MemoryStream())
            {
                //upload file to s3
                await files.CopyToAsync(stream);


                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    string skuHeader = worksheet.Cells[1, 1].Value.ToString().Trim();
                    string AvgCostHeader = worksheet.Cells[1, 2].Value.ToString().Trim();

                    if (skuHeader == "SKU" && AvgCostHeader == "AvgCostUSD")
                    {
                        await uploadFiles.uploadExcelFileToS3(stream, Guid.NewGuid() + "--" + files.FileName);
                        //prepare model for save file upload data
                        FileUploadViewModel viewModel = new FileUploadViewModel();
                        viewModel.FileName = files.FileName;
                        viewModel.FileExtension = ".xlsx";
                        viewModel.IsUploaded = "true";
                        viewModel.UploadDate = DateTime.Now;
                        fileUploadApiAccess.SaveFileUpload(ApiURL, token, viewModel);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            model.Add(new ProductDisplayViewModel
                            {
                                ProductSKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                AvgCost = worksheet.Cells[row, 2].Value.ToString().Trim()
                            });
                        }
                    }
                }
            }

            if (model.Count > 0)
            {
                fileStatus = true;
                foreach (var item in model)
                {
                    try
                    {
                        int productId = ProductApiAccess.GetProductIDBySKU(ApiURL, token, item.ProductSKU);
                        if (productId > 0)
                        {
                            decimal result;
                            if (decimal.TryParse(item.AvgCost, out result))
                            {
                                status = ProductApiAccess.UpdateProductAverageCost(ApiURL, token, item.ProductSKU, item.AvgCost);
                                _successCounter++;
                            }
                            else
                            {
                                FileUploadStatusLogViewModel statusViewModel = new FileUploadStatusLogViewModel();
                                statusViewModel.Sku = item.ProductSKU;
                                statusViewModel.Status = "false";
                                statusViewModel.ErrorMessage = "Avg Cost is not valid " + item.AvgCost;
                                fileUploadApiAccess.SaveFileUploadLogs(ApiURL, token, statusViewModel);
                                _failureCounter++;
                            }
                        }
                        else
                        {
                            FileUploadStatusLogViewModel statusViewModel = new FileUploadStatusLogViewModel();
                            statusViewModel.Sku = item.ProductSKU;
                            statusViewModel.Status = "false";
                            statusViewModel.ErrorMessage = "sku not found";
                            fileUploadApiAccess.SaveFileUploadLogs(ApiURL, token, statusViewModel);
                            _failureCounter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        FileUploadStatusLogViewModel statusViewModel = new FileUploadStatusLogViewModel();
                        statusViewModel.Sku = item.ProductSKU;
                        statusViewModel.Status = "false";
                        statusViewModel.ErrorMessage = "un known error";
                        fileUploadApiAccess.SaveFileUploadLogs(ApiURL, token, statusViewModel);
                        _failureCounter++;
                    }
                }
            }
            return Json(new { success = fileStatus, failureCounter = _failureCounter, successCount = _successCounter });
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcelFileOfASIN(IFormFile files)
        {
            List<ASINSKUMappingViewModel> SkuAsinmodel = new List<ASINSKUMappingViewModel>();
            try
            {


                token = Request.Cookies["Token"];
                List<string> failure = new List<string>();
              
                string status = "";
               
                using (var stream = new MemoryStream())
                {
                    await files.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        string ASIN = worksheet.Cells[1, 1].Value.ToString().Trim();
                        string SKU = worksheet.Cells[1, 2].Value.ToString().Trim();
                        string AmzPrice = worksheet.Cells[1, 3].Value.ToString().Trim();
                        string MaxPrice = worksheet.Cells[1, 4].Value.ToString().Trim();

                        if (ASIN == "ASIN" && SKU == "SKU" && MaxPrice == "MAX_PRICE" && AmzPrice == "AMZ_PRICE")
                        {
                            for (int row = 2; row <= rowCount; row++)
                            {
                                if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 1].Value.ToString() != "" && worksheet.Cells[row, 2].Value != null && worksheet.Cells[row, 2].Value.ToString() != "" && worksheet.Cells[row, 3].Value != null && worksheet.Cells[row, 3].Value.ToString().Trim() != "-" && worksheet.Cells[row, 3].Value.ToString() != "" && worksheet.Cells[row, 4].Value != null && worksheet.Cells[row, 4].Value.ToString() != "" && worksheet.Cells[row, 4].Value.ToString().Trim() != "-")
                                {
                                    SkuAsinmodel.Add(new ASINSKUMappingViewModel
                                    {


                                        ASIN = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        SKU = worksheet.Cells[row, 2].Value.ToString().Trim(),

                                        AmzPrice = Convert.ToDecimal(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                        MAXPrice = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString().Trim())

                                    });
                                }
                                else
                                {
                                    failure.Add(row.ToString());
                                }
                            }
                        }
                    }
                }
                if (failure.Count > 0)
                {
                    return Json(new { success = false, failure = failure });

                }
                if (SkuAsinmodel.Count > 0)
                {
                     status = ProductApiAccess.SaveAsinSkuMappingFromExcelFile(ApiURL, token, SkuAsinmodel);

                }
                return Json(new { success = status, failure = false });
            }
            catch (Exception ex)
            {

                throw;
            }
           

        }

        [HttpPost]
        public async Task<IActionResult> UploadExcelFileForUpdateProductDetail(IFormFile files)
        {
            token = Request.Cookies["Token"];
            int _successCounter = 0;
            int _failureCounter = 0;
            int _newInsertion = 0;
            string status = "";
            bool fileStatus = false;
            List<ProductDisplayViewModel> model = new List<ProductDisplayViewModel>();
            using (var stream = new MemoryStream())
            {
                //upload file to s3
                await files.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    string skuHeader = worksheet.Cells[1, 1].Value.ToString().Trim();
                    string AvgCostHeader = worksheet.Cells[1, 2].Value.ToString().Trim();
                    string Title = worksheet.Cells[1, 3].Value.ToString().Trim();
                    string UPC = worksheet.Cells[1, 4].Value.ToString().Trim();
                    string ImageURL = worksheet.Cells[1, 5].Value.ToString().Trim();
                    if (skuHeader == "SKU" && AvgCostHeader == "AvgCostCAD" && Title == "Title" && UPC == "UPC" && ImageURL == "ImageMain")
                    {
                        await uploadFiles.uploadExcelFileToS3(stream, Guid.NewGuid() + "-ProductDetail" + "-" + files.FileName);
                        //prepare model for save file upload data
                        FileUploadViewModel viewModel = new FileUploadViewModel();
                        viewModel.FileName = files.FileName;
                        viewModel.FileExtension = ".xlsx";
                        viewModel.IsUploaded = "true";
                        viewModel.UploadDate = DateTime.Now;
                        fileUploadApiAccess.SaveFileUpload(ApiURL, token, viewModel);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            model.Add(new ProductDisplayViewModel
                            {
                                ProductSKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                AvgCost = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                ProductTitle = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                Upc = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                ImageURL = worksheet.Cells[row, 5].Value.ToString().Trim(),

                            });
                        }
                    }
                }
            }

            if (model.Count > 0)
            {
                fileStatus = true;
                foreach (var item in model)
                {
                    try
                    {
                        int productId = ProductApiAccess.GetProductIDBySKU(ApiURL, token, item.ProductSKU);
                        if (productId > 0)
                        {
                            decimal result;
                            if (decimal.TryParse(item.AvgCost, out result))
                            {
                                status = ProductApiAccess.UpdateProductDetailFromExcelFile(ApiURL, token, item);
                                _successCounter++;
                            }
                            else
                            {
                                FileUploadStatusLogViewModel statusViewModel = new FileUploadStatusLogViewModel();
                                statusViewModel.Sku = item.ProductSKU;
                                statusViewModel.Status = "false";
                                statusViewModel.ErrorMessage = "Avg Cost is not valid " + item.AvgCost;
                                fileUploadApiAccess.SaveFileUploadLogs(ApiURL, token, statusViewModel);
                                _failureCounter++;
                            }
                        }
                        else
                        {
                            decimal result;
                            if (decimal.TryParse(item.AvgCost, out result))
                            {
                                status = ProductApiAccess.SaveProductDetailFromExcelFile(ApiURL, token, item);
                                _newInsertion++;
                            }
                            else
                            {
                                item.AvgCost = "0";
                                status = ProductApiAccess.SaveProductDetailFromExcelFile(ApiURL, token, item);
                                _newInsertion++;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        FileUploadStatusLogViewModel statusViewModel = new FileUploadStatusLogViewModel();
                        statusViewModel.Sku = item.ProductSKU;
                        statusViewModel.Status = "false";
                        statusViewModel.ErrorMessage = "un known error";
                        fileUploadApiAccess.SaveFileUploadLogs(ApiURL, token, statusViewModel);
                        _failureCounter++;
                    }
                }
            }
            return Json(new { success = fileStatus, newItemInserted = _newInsertion, failureCounter = _failureCounter, successCount = _successCounter });
        }
    }
}