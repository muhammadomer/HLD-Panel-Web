using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class SellerCloudApiAccess
    {

        public bool AddSellerCloudOrder(string ApiURL, string token, List<SellerCloudOrder_CustomerViewModel> ViewModel)
        {
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/SaveOrder");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                request.Timeout = 600000;

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
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
        }

        public List<SKUAndSellerCloudImageURLWhichImagesNotExistsViewModel> GetSKUAndSellerCloudImageURLWhichImagesNotExists(string ApiURL, string token)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/GetSKUAndSellerCloudImageURLWhichImagesNotExists");
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
                List<SKUAndSellerCloudImageURLWhichImagesNotExistsViewModel> skuList = JsonConvert.DeserializeObject<List<SKUAndSellerCloudImageURLWhichImagesNotExistsViewModel>>(strResponse);
                return skuList;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public List<string> GetSKUWhichImagesNotExists(string ApiURL, string token)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/GetSKUWhichImagesNotExists");
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
                List<string> skuList = JsonConvert.DeserializeObject<List<string>>(strResponse);
                return skuList;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool InsertProductDataToProductTable(string ApiURL, string token)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/InsertProductToBestBuy");
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
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<int> GetSellerCloudOrderIdsForImageImport(string ApiURL, string token)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/GetSellerCloudOrderIdsForImagesImport");
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
                List<int> responses = JsonConvert.DeserializeObject<List<int>>(strResponse);
                return responses;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool UpdateSellerCloudOrderDropShipStatus(string ApiURL, string token, UpdateSCDropshipStatusViewModel model)
        {
            try
            {
                var data = JsonConvert.SerializeObject(model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/UpdateSCOrderDropShipStatus");
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



        public bool SaveSellerCloudOrderStatus(string ApiURL, string token, string SellerCloudId, string statusName, string paymentStatus)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/SaveSellerCloudOrderStatus/" + SellerCloudId + "/" + statusName + "/" + paymentStatus);
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
                return Convert.ToBoolean(strResponse);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string SellerCloudOrderLatestUpdateStatus(string ApiURL, string token, string SellerCloudId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/SellerCloudOrderStatusLatestUpdate/" + SellerCloudId);
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
                return strResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<int> GetSellerCloudData(string ApiURL, string token, string SellerCloudIds)
        {
            try
            {
                var data = JsonConvert.SerializeObject(SellerCloudIds);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/GetSellerCloudOrders");
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
                List<int> responses = JsonConvert.DeserializeObject<List<int>>(strResponse);
                return responses;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public List<SCImage> GetSCImagesBySKU(string ApiURL, string token, string SKU)
        {
            List<SCImage> responses = new List<SCImage>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest
                    .Create("https://lp.api.sellercloud.com/rest/api/ProductImage?productID=" + SKU);
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
                responses = JsonConvert.DeserializeObject<List<SCImage>>(strResponse);

            }
            catch (Exception exp)
            {
                throw;
            }
            return responses;
        }


        public bool SaveSellerCloudImages(string ApiURL, string token, ImagesSaveToDatabaseWithURLViewMOdel viewModel)
        {
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/SaveProductImages");
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
        public string GetProductTitle(string ApiURL, string token, GetProductTitleViewModel saveWatchlistViewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(saveWatchlistViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/SellerCloud/GetproducTtitle");
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
                status = Convert.ToString(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return status;
        }

        public GetExplainAmountViewModel GetExplainAmount(string ApiURL, string token, string sellercloudId, string productSku)
        {
            GetExplainAmountViewModel model = new GetExplainAmountViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/BestBuyProduct/GetExplainAmount/"+ sellercloudId + "/" + productSku);
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
                model = JsonConvert.DeserializeObject<GetExplainAmountViewModel>(strResponse);
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }

    }
}
