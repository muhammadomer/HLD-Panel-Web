using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class AWS_S3_FileUploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _bucketName = "upload.hld.erp.images";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = String.Empty;

        public AWS_S3_FileUploadController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }


        public async Task uploadToS3(string filePath)
        {
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                string bucketName;
                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _bucketName + @"/" + _bucketSubdirectory;
                }
                // 1. Upload a file, file name is used as the object key name.
                await fileTransferUtility.UploadAsync(filePath, bucketName);
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
        }
    }
}