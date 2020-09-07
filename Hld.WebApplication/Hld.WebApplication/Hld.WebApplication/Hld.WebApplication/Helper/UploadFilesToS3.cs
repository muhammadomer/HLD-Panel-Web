using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class UploadFilesToS3
    {
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration _configuration;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        private string _ImagesbucketName = "upload.hld.erp.images";//this is my Amazon Bucket name
        private string _ImagesbucketNameCompressed = "upload.hld.erp.images.thumbnail";//this is my compressed Amazon Bucket name
        private string _ExcelFilesbucketName = "";
        private string _AsinBucketURL = "";
        private string _AsinBucketURLCompressed = "";
        private static string _bucketSubdirectory = String.Empty;

        public UploadFilesToS3(IHostingEnvironment environment,IConfiguration configuration)
        {
            _hostingEnvironment = environment;
            _configuration = configuration;
            _ExcelFilesbucketName = _configuration.GetValue<string>("S3ExcelFileBucket:name");
            _AsinBucketURL = _configuration.GetValue<string>("S3AsinImagesBucket:name");
            _AsinBucketURLCompressed = _configuration.GetValue<string>("S3AsinImagesBucketCompressed:name");
        }
        public async Task<bool> uploadToS3(System.IO.Stream stream,string keyName)
        {
            bool status = false;
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                string bucketName;
                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _ImagesbucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _ImagesbucketName + @"/" + _bucketSubdirectory;
                }
                // 1. Upload a file, file name is used as the object key name.
                await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
                status = true;
                return status;
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
            return status;
        }

        public async Task<bool> uploadCompressedToS3(System.IO.Stream stream, string keyName)
        {
            bool status = false;
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                string bucketName;
                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _ImagesbucketNameCompressed; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _ImagesbucketNameCompressed + @"/" + _bucketSubdirectory;
                }
                // 1. Upload a file, file name is used as the object key name.
                await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
                status = true;
                return status;
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
            return status;
        }


        public async Task<bool> uploadExcelFileToS3(System.IO.Stream stream, string keyName)
        {
            bool status = false;
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                string bucketName;
                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _ExcelFilesbucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _ExcelFilesbucketName + @"/" + _bucketSubdirectory;
                }
                // 1. Upload a file, file name is used as the object key name.
                await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
                status = true;
                return status;
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
            return status;
        }

        public async Task<bool> uploadASINImagesToS3(Stream stream, string keyName)
        {
            bool status = false;
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                string bucketName;
                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _AsinBucketURL; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _AsinBucketURL + @"/" + _bucketSubdirectory;
                }
                // 1. Upload a file, file name is used as the object key name.
                await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
                status = true;
                return status;
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
            return status;
        }

        public async Task<bool> uploadASINImagesToS3Compressed(Stream stream, string keyName)
        {
            bool status = false;
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast2));

                string bucketName;
                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _AsinBucketURLCompressed; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _AsinBucketURLCompressed + @"/" + _bucketSubdirectory;
                }
                // 1. Upload a file, file name is used as the object key name.
                await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
                status = true;
                return status;
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
            return status;
        }

        public   async Task DeleteObjectNonVersionedBucketAsync(string bucketName,string KeyName)
        {
            
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = KeyName
                };

                Console.WriteLine("Deleting an object");
                await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }


    }
}
