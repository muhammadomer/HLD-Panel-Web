using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    [Authorize(Policy = "Access to BulkUpdate Tab")]

    public class UploadFilesToS3ForJobsController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "jobsfilesbucket";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = "ASINSKUMapping-Jobs";
        private static string _Ds_Qty_CommentsSubdirectory = "Ds_Qty_Comments";
        private static string _ImportMissingSkuSubdirectory = "MissingSKUFromSellerCloud";

        private static string _ImportInventoryContinueSubdirectory = "MissingSKUFromSellerCloud";

        string ApiURL = "";
        string token = "";
        private readonly ILogger logger;
        UploadFilesToS3ForJobsAPIAccess ApiAccess = null;
        private readonly IConfiguration _configuration;
        public UploadFilesToS3ForJobsController(IConfiguration configuration, IHostingEnvironment environment, ILogger<UploadFilesToS3ForJobsController> _logger)
        {
            _hostingEnvironment = environment;


            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ApiAccess = new UploadFilesToS3ForJobsAPIAccess();
            this.logger = _logger;
        }



        public IActionResult Index()
        {
            return View();
        }


        public IActionResult S3JobDetail()
        {
            string token = Request.Cookies["Token"];
            List<GetJobDetailViewModel> listmodel = new List<GetJobDetailViewModel>();
            listmodel = ApiAccess.GetJobDetail(ApiURL, token);
            return View(listmodel);

        }


        public IActionResult AsinSkuMappingToS3()
        {
            return View();
        }


        public IActionResult UploadAsinSkuMappingToS3(IFormFile files)

        {
            JobIdReturnViewModel jobIdReturnView = new JobIdReturnViewModel();

            string FileNameDate = DateTime.Now.Ticks + files.FileName;
            try
            {

                if (files.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(files.ContentDisposition)
                            .FileName
                            .TrimStart().ToString();
                    logger.LogInformation("UploadAsinSkuMappingToS3 filename = " + filename);
                    filename = Path.GetTempPath() + FileNameDate;
                    logger.LogInformation("UploadAsinSkuMappingToS3 after environment filename = " + filename);
                    using (var fs = System.IO.File.Create(filename))
                    {
                        files.CopyTo(fs);
                        fs.Flush();
                    }//these code snippets saves the uploaded files to the project directory
                    logger.LogInformation("UploadAsinSkuMappingToS3 after creating file filename = " + filename);
                    string bucketname = uploadExcellToS3(filename);//this is the method to upload saved file to S3
                    logger.LogInformation("UploadAsinSkuMappingToS3 after uploading file filename = " + filename);
                    if (System.IO.File.Exists(filename))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(filename);

                    }
                    token = Request.Cookies["Token"];

                    UploadFilesToS3ViewModel ViewModel = new UploadFilesToS3ViewModel();
                    ViewModel.FileName = FileNameDate;
                    ViewModel.FilePath = bucketname;
                    ViewModel.JobType = "skuasin_am_mxpr_dr_qty_comments";

                    jobIdReturnView = ApiAccess.SaveJobDetail(ApiURL, token, ViewModel);



                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("UploadAsinSkuMappingToS3 in exception = " + ex);
                throw;
            }

            return Json(new { success = jobIdReturnView.status, JobID = jobIdReturnView.jobid });
        }

        public IActionResult UploadDS_QTY_CommentsToS3(IFormFile files)

        {
            JobIdReturnViewModel jobIdReturnView = new JobIdReturnViewModel();

            string FileNameDate = DateTime.Now.Ticks + files.FileName;
            try
            {

                if (files.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(files.ContentDisposition)
                            .FileName
                            .TrimStart().ToString();

                    filename = Path.GetTempPath() + FileNameDate;

                    using (var fs = System.IO.File.Create(filename))
                    {
                        files.CopyTo(fs);
                        fs.Flush();
                    }
                    string bucketname = uploadExcellDSToS3(filename);//this is the method to upload saved file to S3

                    if (System.IO.File.Exists(filename))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(filename);

                    }
                    token = Request.Cookies["Token"];

                    UploadFilesToS3ViewModel ViewModel = new UploadFilesToS3ViewModel();
                    ViewModel.FileName = FileNameDate;
                    ViewModel.FilePath = bucketname;
                    ViewModel.JobType = "DS_QTY_COMMENTS";

                    jobIdReturnView = ApiAccess.SaveJobDetail(ApiURL, token, ViewModel);



                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("UploadAsinSkuMappingToS3 in exception = " + ex);
                throw;
            }

            return Json(new { success = jobIdReturnView.status, JobID = jobIdReturnView.jobid });


        }
        public IActionResult UploadMissingSkuFileToS3(IFormFile files)

        {
            JobIdReturnViewModel jobIdReturnView = new JobIdReturnViewModel();

            string FileNameDate = DateTime.Now.Ticks + files.FileName;
            try
            {

                if (files.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(files.ContentDisposition)
                            .FileName
                            .TrimStart().ToString();

                    filename = Path.GetTempPath() + FileNameDate;

                    using (var fs = System.IO.File.Create(filename))
                    {
                        files.CopyTo(fs);
                        fs.Flush();
                    }
                    string bucketname = uploadExcellSKUToS3(filename);//this is the method to upload saved file to S3

                    if (System.IO.File.Exists(filename))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(filename);

                    }
                    token = Request.Cookies["Token"];

                    UploadFilesToS3ViewModel ViewModel = new UploadFilesToS3ViewModel();
                    ViewModel.FileName = FileNameDate;
                    ViewModel.FilePath = bucketname;
                    ViewModel.JobType = "Import missing sku from seller-cloud";

                    jobIdReturnView = ApiAccess.SaveJobDetail(ApiURL, token, ViewModel);



                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("UploadAsinSkuMappingToS3 in exception = " + ex);
                throw;
            }

            return Json(new { success = jobIdReturnView.status, JobID = jobIdReturnView.jobid });


        }

        public string uploadExcellDSToS3(string filePath)
        {
            string bucketName = "";
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                if (_Ds_Qty_CommentsSubdirectory == "" || _Ds_Qty_CommentsSubdirectory == null)
                {
                    bucketName = _bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _bucketName + @"/" + _Ds_Qty_CommentsSubdirectory;
                }


                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);



            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
                logger.LogInformation("uploadExcellToS3 in exception  = " + s3Exception.Message);
            }
            return bucketName;
        }
        public string uploadExcellSKUToS3(string filePath)
        {
            string bucketName = "";
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                if (_ImportMissingSkuSubdirectory == "" || _ImportMissingSkuSubdirectory == null)
                {
                    bucketName = _bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _bucketName + @"/" + _ImportMissingSkuSubdirectory;
                }


                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);



            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
                logger.LogInformation("uploadExcellToS3 in exception  = " + s3Exception.Message);
            }
            return bucketName;
        }
        public string uploadExcellToS3(string filePath)
        {
            string bucketName = "";
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                logger.LogInformation("uploadExcellToS3 in uploading file filename = " + filePath);


                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _bucketName + @"/" + _bucketSubdirectory;
                }


                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);



            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
                logger.LogInformation("uploadExcellToS3 in exception  = " + s3Exception.Message);
            }
            return bucketName;
        }

        public IActionResult JobLogs(int id)
        {


            string token = Request.Cookies["Token"];
            S3LogViewModel listmodel = new S3LogViewModel();
            listmodel = ApiAccess.GetJobLogs(ApiURL, token, id);
            return View(listmodel);


        }

        public IActionResult DownloadformS3(string file, string bucket)
        {
            try
            {
                logger.LogInformation("DownloadformS3 ------------------> ");
                string filename = "excel.xlsx";
                TransferUtility fileTransferUtility = new
                   TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                //  string FileLocation = _hostingEnvironment.WebRootPath + "\\App_Data\\" + filename;
                string FileLocation = (Path.GetTempPath() + filename);
                logger.LogInformation("DownloadformS3 ------------------> FileLocation " + FileLocation);
                FileStream fs = System.IO.File.Create(FileLocation);
                fs.Close();
                logger.LogInformation("DownloadformS3 ------------------> FileLocation after create on local" + FileLocation);
                fileTransferUtility.Download(FileLocation, bucket, file);
                logger.LogInformation("DownloadformS3 ------------------> FileLocation after download on local" + FileLocation);
                fileTransferUtility.Dispose();

                logger.LogInformation("DownloadformS3 ------------------> FileLocation before  download file to browser on local" + FileLocation);
                return downloadFile(Path.GetTempPath(), filename, file);
            }
            catch (Exception ex)
            {
                logger.LogInformation("DownloadformS3 ------------------> exception --->" + ex);
                throw;
            }




        }

        public FileResult downloadFile(string filePath, string fileName, string file)
        {
            logger.LogInformation("downloadFile ------------------>  " + filePath);
            IFileProvider provider = new PhysicalFileProvider(filePath);
            IFileInfo fileInfo = provider.GetFileInfo(fileName);
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, file);
        }

        public IActionResult UpdateDiscontinueInventory()
        {
            return View();
        }
        // UploadInventoryToS3 by Mehdi
        public IActionResult UploadInventoryToS3(IFormFile files)
        {
            JobIdReturnViewModel jobIdReturnView = new JobIdReturnViewModel();

            string FileNameDate = DateTime.Now.Ticks + files.FileName;
            try
            {

                if (files.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(files.ContentDisposition)
                            .FileName
                            .TrimStart().ToString();
                    logger.LogInformation("UploadInventoryContinueDiscontinueToS3 filename = " + filename);
                    filename = Path.GetTempPath() + FileNameDate;
                    logger.LogInformation("UploadInventoryContinueDiscontinueToS3 after environment filename = " + filename);
                    using (var fs = System.IO.File.Create(filename))
                    {
                        files.CopyTo(fs);
                        fs.Flush();
                    }//these code snippets saves the uploaded files to the project directory
                    logger.LogInformation("UploadInventoryContinueDiscontinueToS3 after creating file filename = " + filename);
                    string bucketname = uploadExcellToS3(filename);//this is the method to upload saved file to S3
                    logger.LogInformation("UploadInventoryContinueDiscontinueToS3 after uploading file filename = " + filename);
                    if (System.IO.File.Exists(filename))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(filename);

                    }
                    token = Request.Cookies["Token"];

                    UploadFilesToS3ViewModel ViewModel = new UploadFilesToS3ViewModel();
                    ViewModel.FileName = FileNameDate;
                    ViewModel.FilePath = bucketname;
                    ViewModel.JobType = "InventoryContinueDiscontinue";
                    jobIdReturnView = ApiAccess.SaveJobDetail(ApiURL, token, ViewModel);
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("UploadInventoryContinueDiscontinueToS3 in exception = " + ex);
                throw;
            }

            return Json(new { success = jobIdReturnView.status, JobID = jobIdReturnView.jobid });
        }
    }
}