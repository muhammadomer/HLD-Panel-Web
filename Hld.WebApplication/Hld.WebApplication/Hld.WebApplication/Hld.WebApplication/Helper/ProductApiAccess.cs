using DataAccess.ViewModels;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.Views.Product;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class ProductApiAccess
    {
        public int AddProduct(string ApiURL, string token, ProductInsetUpdateViewModel ViewModel)
        {
            int productId = 0;
            try
            {
                if (ViewModel.ProductId == 0)
                {
                    if (!string.IsNullOrEmpty(ViewModel.CategoryIds))
                    {
                        string[] categoryiesId = ViewModel.CategoryIds.Split(",");

                        ViewModel.CategoryMain = int.Parse(categoryiesId[0]);
                        ViewModel.CategorySub1 = int.Parse(categoryiesId[1]);
                        ViewModel.CategorySub2 = int.Parse(categoryiesId[2]);
                        ViewModel.CategorySub3 = int.Parse(categoryiesId[3]);
                        ViewModel.CategorySub4 = int.Parse(categoryiesId[4]);

                    }


                    var data = JsonConvert.SerializeObject(ViewModel);


                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/save");
                    request.Method = "POST";
                    request.Accept = "application/json;";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + token;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    var response = (HttpWebResponse)request.GetResponse();
                    string strResponse = "";
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        strResponse = sr.ReadToEnd();
                        string array = JObject.Parse(strResponse)["productId"].ToString();
                        productId = int.Parse(array);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return productId;
        }

        public int AddProductnew(string ApiURL, string token, ProductSaveViewModel ViewModel)
        {
            int productId = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/savenew");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                    string array = JObject.Parse(strResponse)["productId"].ToString();
                    productId = int.Parse(array);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return productId;
        }

        public int UpdateProduct(string ApiURL, string token, ProductInsetUpdateViewModel ViewModel)
        {
            int productId = 0;
            try
            {
                if (ViewModel.ProductId > 0)
                {
                    if (!string.IsNullOrEmpty(ViewModel.CategoryIds))
                    {
                        string[] categoryiesId = ViewModel.CategoryIds.Split(",");

                        ViewModel.CategoryMain = int.Parse(categoryiesId[0]);
                        ViewModel.CategorySub1 = int.Parse(categoryiesId[1]);
                        ViewModel.CategorySub2 = int.Parse(categoryiesId[2]);
                        ViewModel.CategorySub3 = int.Parse(categoryiesId[3]);
                        ViewModel.CategorySub4 = int.Parse(categoryiesId[4]);

                    }
                    ProductInsertUpdateViewModel product = new ProductInsertUpdateViewModel()
                    {
                        ProductId = ViewModel.ProductId,
                        ProductSKU = ViewModel.ProductSKU,
                        ProductTitle = ViewModel.ProductTitle,
                        ConditionId = ViewModel.ConditionId,
                        Condition = ViewModel.Condition,
                        Color = ViewModel.Color,
                        ColorId = ViewModel.ColorId,
                        BrandId = ViewModel.BrandId,
                        Brand = ViewModel.Brand,
                        Upc = ViewModel.Upc,
                        Category = ViewModel.Category,
                        Description = ViewModel.Description,
                        AvgCost = ViewModel.AvgCost,
                        ShipmentWeight = ViewModel.ShipmentWeight,
                        CategoryIds = ViewModel.CategoryIds,
                        CategoryMain = ViewModel.CategoryMain,
                        CategorySub1 = ViewModel.CategorySub1,
                        CategorySub2 = ViewModel.CategorySub2,
                        CategorySub3 = ViewModel.CategorySub3,
                        CategorySub4 = ViewModel.CategorySub4,
                        shipmentLenght = ViewModel.shipmentLenght,
                        shipmentWidth = ViewModel.shipmentWidth,
                        shipmentHeight = ViewModel.shipmentHeight,
                        StatusName = ViewModel.StatusName
                    };

                    var data = JsonConvert.SerializeObject(product);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/Update");
                    request.Method = "POST";
                    request.Accept = "application/json;";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + token;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    var response = (HttpWebResponse)request.GetResponse();
                    string strResponse = "";
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        strResponse = sr.ReadToEnd();
                        string array = JObject.Parse(strResponse)["productId"].ToString();
                        productId = int.Parse(array);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return productId;
        }


        public bool DeleteProductImage(string ApiURL, string token, int id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/Delete/" + id + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }

            var jsonObjectResponse = JObject.Parse(strResponse)["status"].ToString();

            return Convert.ToBoolean(jsonObjectResponse);
        }

        public List<ProductDisplayInventoryViewModel> GetAllProducts(string ApiURL, string token, int startLimit, int endLimit, string sort, string dropship, string dropshipsearch, string sku, string asin, string ProductTitle, string DSTag, string TypeSearch)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/" + startLimit + "/" + endLimit + "/" + sort + "/" + dropship + "/" + "/" + dropshipsearch + "/" + "/" + sku + "/" + asin + "/" + ProductTitle + "/" + DSTag + "/" + TypeSearch);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductDisplayInventoryViewModel> responses = JsonConvert.DeserializeObject<List<ProductDisplayInventoryViewModel>>(strResponse);
            return responses;
        }

        public List<ExportProductDataViewModel> GetAllProductsForExport(string ApiURL, string token, string dropship, string dropshipstatusSearch, string sku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/Export/" + dropship + "/" + dropshipstatusSearch + "/" + sku);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ExportProductDataViewModel> responses = JsonConvert.DeserializeObject<List<ExportProductDataViewModel>>(strResponse);

            return responses;
        }

        public List<ProductDisplayInventoryViewModel> GetAllProductsWihtoutPageLimit(string ApiURL, string token, ProductInventorySearchViewModel ViewModel)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product");
            var data = JsonConvert.SerializeObject(ViewModel);
            request.Method = "POST";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductDisplayInventoryViewModel> responses = JsonConvert.DeserializeObject<List<ProductDisplayInventoryViewModel>>(strResponse);

            return responses;
        }

        public List<FileContents> GetShadowsOfChildSku(string ApiURL, string token, List<CreateRelationOnSCViewModel> dataSKU)
        {
            List<FileContents> responses = new List<FileContents>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetShadowsOfChildForXls");
                var data = JsonConvert.SerializeObject(dataSKU);
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                string strResponse = "";
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        strResponse = stream.ReadToEnd();
                    }
                }

                 responses = JsonConvert.DeserializeObject<List<FileContents>>(strResponse);
            }
            catch (Exception ex)
            {

                throw ex;
            }
         
            return responses;
        }
        public ProductDisplayViewModel GetProductDetailBySKU(string ApiURL, string token, string sku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/ProductDeatil?sku=" + sku);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            ProductDisplayViewModel responses = JsonConvert.DeserializeObject<ProductDisplayViewModel>(strResponse);
            return responses;
        }


        public int GetAllProductsCount(string ApiURL, string token, ProductInventorySearchViewModel ViewModel)
        {
            var data = JsonConvert.SerializeObject(ViewModel);
            //string alterSKU = sku == null || sku== "undefined" ? "Nill" : sku.Trim();
            //string skuListNew = skuList == null || skuList == "undefined" || skuList=="" ? "Nill" : skuList.Trim();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/TotalCountProductIn_inventory");
            request.Method = "POST";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }

            return Convert.ToInt32(strResponse);
        }




        public int GetProductIDBySKU(string ApiURL, string token, string SKU)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetProductId/" + SKU + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            string array = JObject.Parse(strResponse)["product_Id"].ToString();
            return Convert.ToInt32(array);
        }

        public int ExecuteJobs(string ApiURL, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchListJobs");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            return Convert.ToInt32(strResponse);
        }

        public int CheckProductId(string ApiURL, string token, string SKU)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/CheckProductId/" + SKU + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            string array = JObject.Parse(strResponse)["product_Id"].ToString();
            return Convert.ToInt32(array);
        }


        public List<ProductImagesViewModel> GetAllProductByProductID(string ApiURL, string token, string id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ProductImages/" + id + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductImagesViewModel> responses = JsonConvert.DeserializeObject<List<ProductImagesViewModel>>(strResponse);

            return responses;
        }


        public bool UpdateSCImageStatusInProductTable(string ApiURL, string token, string sku, bool status)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/updateSCImageStatusInProductTable/" + sku + "/" + status);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }


            return Convert.ToBoolean(strResponse);
        }


        public ProductInsetUpdateViewModel GetProductDetail_ProductID(string ApiURL, string token, string id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/ProductById/" + id + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            ProductInsetUpdateViewModel responses = JsonConvert.DeserializeObject<ProductInsetUpdateViewModel>(strResponse);

            return responses;
        }

        public bool AddProductImages(string ApiURL, string token, ProductImagesViewModel ViewModel)
        {
            try
            {
                if (ViewModel.ProductId != null)
                {
                    var data = JsonConvert.SerializeObject(ViewModel);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/saveImage");
                    request.Method = "POST";
                    request.Accept = "application/json;";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + token;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    var response = (HttpWebResponse)request.GetResponse();
                    string strResponse = "";
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }

        public bool UpdateProductDropshipAndQty(string ApiURL, string token, BBProductViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateDropshipStatusAndQty");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                status = Convert.ToBoolean(strResponse);
            }
            catch (Exception)
            {
                throw;
            }



            return true;
        }


        public string UpdateProductAverageCost(string ApiURL, string token, string sku, string averageCost)
        {
            string status = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateProductAverageCost/" + sku + "/" + averageCost);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;


                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                status = JsonConvert.DeserializeObject<string>(strResponse);

            }
            catch (Exception ex)
            {

            }
            return status;
        }


        public bool UpdateProductStatus(string ApiURL, string token, string sku, string productStatusId)
        {
            bool status = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/updateProductStatus/" + sku + "/" + productStatusId);
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;


                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception ex)
            {

            }
            return status;
        }

        public string UpdateProductDetailFromExcelFile(string ApiURL, string token, ProductDisplayViewModel viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateProductDetailFromExcelFile");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                status = JsonConvert.DeserializeObject<string>(strResponse);

            }
            catch (Exception ex)
            {

            }
            return status;
        }


        public string SaveProductDetailFromExcelFile(string ApiURL, string token, ProductDisplayViewModel viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/SaveProductDetailFromExcelFile");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                status = JsonConvert.DeserializeObject<string>(strResponse);

            }
            catch (Exception ex)
            {

            }
            return status;
        }

        public bool DeleteProduct(string ApiURL, string token, DeleteProductViewModel viewModel)
        {
            //,out string message
            bool status = false;

            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/Delete");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                status = Convert.ToBoolean(JObject.Parse(strResponse)["status"].ToString());
                //message = JObject.Parse(strResponse)["message"].ToString();
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public string SaveAsinSkuMappingFromExcelFile(string ApiURL, string token, List<ASINSKUMappingViewModel> viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/FileUpload/SaveSkuAsinMapping");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }

        public List<ProductSKUViewModel> GetAllSKUByName(string apiurl, string token, string name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Product/SKU/" + name + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductSKUViewModel> responses = JsonConvert.DeserializeObject<List<ProductSKUViewModel>>(strResponse);
            return responses;
        }

        public ProductDetailForAPViewModel GetProductDetailForAPBySKU(string ApiURL, string token, string sku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/GetProductDetailForAPBySKU/" + sku);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            ProductDetailForAPViewModel responses = JsonConvert.DeserializeObject<ProductDetailForAPViewModel>(strResponse);
            return responses;
        }
        public List<AsinAmazonePriceViewModel> GetSkuAmazonePrice(string ApiURL, string token, string sku)
        {
            List<AsinAmazonePriceViewModel> ViewList = new List<AsinAmazonePriceViewModel>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetProductBySKuAmazoneprice/" + sku);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewList = JsonConvert.DeserializeObject<List<AsinAmazonePriceViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewList;
        }

        public ProductSaveViewModel GetProductInfoFromSellerCloudForMIssingSku(string ApiURL, string token, string SKU)
        {
            var Item = new ProductSaveViewModel();
            ProductSaveListViewModel responses = new ProductSaveListViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest
                    .Create(ApiURL + "/Catalog?model.sKU=" + SKU);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                string strResponse = "";
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        strResponse = stream.ReadToEnd();
                    }
                }
                responses = JsonConvert.DeserializeObject<ProductSaveListViewModel>(strResponse);
                Item = responses.Items.FirstOrDefault();

            }
            catch (Exception exp)
            {
                throw;
            }
            return Item;
        }

        public string ContinueDisContinue(string ApiURL, string token, List<ProductContinueDisContinueViewModel> viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/ContinueDisContinue/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }

        public string KitOrShadow(string ApiURL, string token, List<ProductSetAsKitOrShadowViewModel> viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/KitOrShadow/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }
        public int GetStatusFromZinc(string ApiURL, string token, List<GetStatusFromZincViewModel> viewModel)
        {
          //  string status = "";
            int ID = 0;
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetStausFromZinc/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                
                ID = Convert.ToInt32(strResponse);
              //  status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return ID;
        }

        public List<ProductWarehouseQtyViewModel> GetWareHousesQtyList(string ApiURL, string token, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetWareHousesQtyList?SKU=" + SKU);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";


            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductWarehouseQtyViewModel> responses = JsonConvert.DeserializeObject<List<ProductWarehouseQtyViewModel>>(strResponse);

            return responses;
        }

        public bool productAddMultipleSku(string ApiURL, string token, SaveParentSkuVM ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/SaveParentSKU/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
        public SaveParentSkuVM GetProductDetail_ProductIDByid(string ApiURL, string token, string id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetParentSkuWithId/" + id + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            SaveParentSkuVM responses = JsonConvert.DeserializeObject<SaveParentSkuVM>(strResponse);

            return responses;
        }

        public List<SaveParentSkuVM> GetMultipleproductDetailist(string ApiURL, string token)
        {
            List<SaveParentSkuVM> listmodel = new List<SaveParentSkuVM>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetAllParentSKUList/");
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                listmodel = JsonConvert.DeserializeObject<List<SaveParentSkuVM>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }

        public GetParentSkuById GetproductById(string ApiURL, string token, int id)
        {
            GetParentSkuById ViewModel = new GetParentSkuById();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetParentSkuWithId?id=" + id);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<GetParentSkuById>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }
        public List<GetChildSkuVM> GetChildProductList(string apiurl, string token,int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Product/GetChildSkuById?id=" + id);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<GetChildSkuVM> responses = JsonConvert.DeserializeObject<List<GetChildSkuVM>>(strResponse);
            return responses;
        }

        public string SaveChildSku(string ApiURL, string token, List<SaveChildSkuVM> viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/SaveChildSKUList/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }

        public string CreateProductOnSellerCloud(string ApiURL, string token, CreateProductOnSallerCloudViewModel viewModel)
        {
            string status = "";
            try
            {

                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lp.api.sellercloud.com/rest/api/Products");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public string ImageUpdateOnSellerCloud(string ApiURL, string token, UpdateImageOnScViewModel viewModel)
        {
            string status = "";
            try
            {
                
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lp.api.sellercloud.com/rest/api/ProductImage");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }
        public List<GetImageUrlOfChildSkuViewModel> GetImageUrlOfChildSku(string ApiURL, string token, string childSku)
        {
            List<GetImageUrlOfChildSkuViewModel> listmodel = new List<GetImageUrlOfChildSkuViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetChildSkuImageUrl?childSku=" + childSku);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                listmodel = JsonConvert.DeserializeObject<List<GetImageUrlOfChildSkuViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }
        public CheckChildSkuImageUpdatedOnSCViewModel CheckChildSkuImageUpdatedOnSC(string ApiURL, string token, string sku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/CheckChildSkuImageUpdatedOnSC?sku=" + sku);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";


            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            CheckChildSkuImageUpdatedOnSCViewModel responses = JsonConvert.DeserializeObject<CheckChildSkuImageUpdatedOnSCViewModel>(strResponse);

            return responses;
        }
        public GetXlsFileResponseViewModel CreateProductShadowOnSellerCloud(string ApiURL, string token, CreateshadowOnSellerCloudViewModel viewModel)
        {
            //string status = "";
            GetXlsFileResponseViewModel status = new GetXlsFileResponseViewModel();
            try
            {

                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lp.api.sellercloud.com/rest/api/Catalog/Imports/Shadows");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<GetXlsFileResponseViewModel>(strResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public List<GetShadowsOfChildViewModel> GetShadowsOfChild(string ApiURL, string token,string childSku)
        {
            List<GetShadowsOfChildViewModel> listmodel = new List<GetShadowsOfChildViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/GetShadowsOfChild?childSku="+ childSku);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                listmodel = JsonConvert.DeserializeObject<List<GetShadowsOfChildViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }
        public CheckChildOrShadowCreatedOnSCViewModel CheckChildOrShadowCreatedOnSC(string ApiURL, string token, string sku)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/CheckChildOrShadowCreatedOnSC?sku=" + sku);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";


            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            CheckChildOrShadowCreatedOnSCViewModel responses = JsonConvert.DeserializeObject<CheckChildOrShadowCreatedOnSCViewModel>(strResponse);

            return responses;
        }
        public bool DeleteChildProduct(string ApiURL, string token, int child_id)
        {
            bool status = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/DeleteChildSku?child_id=" + child_id);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
        public bool DeleteChildProductChildImage(string ApiURL, string token, int ChildImage)
        {
            bool status = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/DeleteChildImage?ChildImage=" + ChildImage);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";

                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
        public bool Update(string ApiURL, string token, SaveChildSkuVM ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateChildSKU");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                    status = Convert.ToBoolean(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return status;
        }

        public bool UpdateRelation(string ApiURL, string token, UpdateIsRelationViewModel updateIsRelation)
        {
            bool status = false;
            
            try
            {
                var data = JsonConvert.SerializeObject(updateIsRelation);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateRelation");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                    status = Convert.ToBoolean(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return status;
        }
        public bool UpdateSkustatusonsellercloud(string ApiURL, string token, string sku)
        {
            bool status = false;
            try
            {
              
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateProductStatusWhenProductCreatedOnSC?sku="+sku);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";


                using (WebResponse webResponse = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        strResponse = stream.ReadToEnd();
                        status = Convert.ToBoolean(strResponse);
                    }
                }
               
                
                
            }
            catch (Exception ex)
            {
                throw;
            }
            return status;
        }
        public string ShadowCreate(string ApiURL, string token,List<SaveSkuShadowViewModel> viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/SaveChildSkuShadow/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }
       

        public bool SaveSellerCloudImagesForChildSku(string ApiURL, string token, ImagesSaveToDatabaseWithURLViewMOdel viewModel)
        {
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/product/SaveProductImages");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;


                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }

                return Convert.ToBoolean(strResponse);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool SaveProductManufactured(string ApiURL, string token, ProductManufacturedViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/AddManufacturer/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }

        public bool SaveProductManufacturedModel(string ApiURL, string token, ProductManufacturedViewModel Model)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(Model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/AddManufacturerModel/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }

        //public List<ProductManufacturedViewModel> GetManufacture(string ApiURL, string token)
        //{
        //    List<ProductManufacturedViewModel> listmodel = new List<ProductManufacturedViewModel>();
        //    try
        //    {



        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/GetManufacture/");
        //        request.Method = "GET";
        //        request.Accept = "application/json;";
        //        request.ContentType = "application/json";
        //        request.Headers["Authorization"] = "Bearer " + token;
        //        string strResponse = "";

        //        var response = (HttpWebResponse)request.GetResponse();

        //        using (var sr = new StreamReader(response.GetResponseStream()))
        //        {
        //            strResponse = sr.ReadToEnd();
        //        }
        //        listmodel = JsonConvert.DeserializeObject<List<ProductManufacturedViewModel>>(strResponse);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return listmodel;
        //}
        public List<ProductManufacturedViewModel> GetManufacture(string apiurl, string token, string name)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Manufacture/GetManufacture/" + name + "");
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductManufacturedViewModel> responses = JsonConvert.DeserializeObject<List<ProductManufacturedViewModel>>(strResponse);
            return responses;
        }

        //public bool GetManufactureIdByNameChange(string ApiURL, string token, int ManufactureId)
        //{
        //    bool status = false;

        //    try
        //    {
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/GetManufactureModel?ManufactureId=" + ManufactureId);
        //        request.Method = "GET";
        //        request.Accept = "application/json;";
        //        request.ContentType = "application/json";
        //        request.Headers["Authorization"] = "Bearer " + token;
        //        string strResponse = "";

        //        var response = (HttpWebResponse)request.GetResponse();

        //        using (var sr = new StreamReader(response.GetResponseStream()))
        //        {
        //            strResponse = sr.ReadToEnd();
        //        }
        //        status = JsonConvert.DeserializeObject<bool>(strResponse);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return status;
        //}

        public List<GetManufactureModelViewModel> GetManufactureIdByNameChange(string apiurl, string token, int ManufactureId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Manufacture/GetManufactureModel?ManufactureId=" + ManufactureId);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<GetManufactureModelViewModel> responses = JsonConvert.DeserializeObject<List<GetManufactureModelViewModel>>(strResponse);
            return responses;
        }

        public List<ProductManufactureListViewModel> GetManufactureList(string apiurl, string token, int ManufactureId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Manufacture/GetManufactureList?ManufactureId=" + ManufactureId);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<ProductManufactureListViewModel> responses = JsonConvert.DeserializeObject<List<ProductManufactureListViewModel>>(strResponse);
            return responses;
        }
        public List<GetDeviceModelViewMdel> GetManufactureDeviceIdByNameChange(string apiurl, string token, int ManufactureModel,int ManufactureId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/Manufacture/GetDeviceModelModel?ManufactureModel=" + ManufactureModel+ "&ManufactureId=" + ManufactureId);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";
            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<GetDeviceModelViewMdel> responses = JsonConvert.DeserializeObject<List<GetDeviceModelViewMdel>>(strResponse);
            return responses;
        }
        public bool ProductDeviceModel(string ApiURL, string token, AddDeviceModelView Model)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(Model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/AddDeviceModel/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
        public string ManufacturedUpdate(string ApiURL, string token, EditManufactureListModelView viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/EditManufactureList/");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public string BulkUpdateList(string ApiURL, string token,EditBulkUpdateViewModel viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BulkUpdate/EditBulkUpdate/");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }

        public bool Style(string ApiURL, string token, AddStyleViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Manufacture/AddStyle/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }

        public bool UpdateShadowSingleColoumn(string ApiURL, string token, UpdateShadowSingleColoumnViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/UpdateShadowSingleColoumn");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                string strResponse = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                    status = Convert.ToBoolean(strResponse);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
    }
}

