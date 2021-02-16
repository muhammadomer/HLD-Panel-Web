using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Helper
{
    public class ZincApiAccess
    {


        public bool UpdateZincProductASIN(string ApiURL, string token, ZincProductSaveViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/UpdateZincProductASIN");
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



        public bool UpdateZincProductASINDetail(string ApiURL, string token, ZincProductSaveViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/UpdateZincProductASINDetail");
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



        public bool SaveASINProductDetail(string ApiURL, string token, ASINDetailViewModel model)
        {
            try
            {
                var data = JsonConvert.SerializeObject(model);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/SaveASINProductDetail");
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

                throw;
            }
        }



        public bool SaveASINProductImageDetail(string ApiURL, string token, ASINProductImageViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/SaveASINProductImageDetail");
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
            if (strResponse != null)
            {
                return Convert.ToBoolean(strResponse);
            }
            else
            {
                return false;
            }
        }


        public int SaveZincProductASIN(string ApiURL, string token, ZincProductSaveViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/saveZincProductASIN");
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
            if (strResponse != null)
            {
                return Convert.ToInt16(strResponse);
            }
            else
            {
                return 0;
            }
        }

        public int SaveZincASINOffer(string ApiURL, string token, ZincASINOfferDetail model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/SaveZincASINOffer");
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
            if (strResponse != null)
            {
                return Convert.ToInt16(strResponse);
            }
            else
            {
                return 0;
            }
        }

        public string GetCustomerDetailForSendOrderToZinc(string ApiURL, string token, string ASIN, string MaxPrice, string orderid, string SellerOrderID, string orderDetailID)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetCustomerDetailForSendOrderToZinc/" + ASIN + "/" + MaxPrice + "/" + orderid + "/" + SellerOrderID + "/" + orderDetailID);
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
                // SaveZincOrders.RootObject rootObject = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(strResponse);
                return strResponse;
            }
        }

        public SaveZincOrders.RootObject GetActiveZincAccountsList(string ApiURL, string token)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetActiveZincAccountsList/");
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
                SaveZincOrders.RootObject rootObject = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(strResponse);
                return rootObject;
            }
        }

        public string GetCustomerDetailForSendOrderToZincForOrderView(string ApiURL, string token, string ASIN, string orderid, string SellerOrderID)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetCustomerDetailForSendOrderToZincForOrderView/" + ASIN + "/" + orderid + "/" + SellerOrderID);
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
                // SaveZincOrders.RootObject rootObject = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(strResponse);
                return strResponse;
            }
        }
        public string SubmitOrderToZinc(string ApiURL, string token, string ASIN, string MaxPrice, string orderid, string SellerOrderID, string orderDetailID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetCustomerDetailForSendOrderToZinc/" + ASIN + "/" + MaxPrice + "/" + orderid + "/" + SellerOrderID + "/" + orderDetailID);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            string strResponse = "";
            try
            {
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        strResponse = stream.ReadToEnd();
                    }
                    // SaveZincOrders.RootObject rootObject = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(strResponse);

                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return strResponse;
        }


        public int GetProductASINAlreadyExists(string ApiURL, string token, string sku, string asin)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetProductASINAlreadyExists/" + sku + "/" + asin);
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
                return Convert.ToInt32(strResponse);
            }
        }

        public int GetProductASINAlreadyExistsInASINProductDetail(string ApiURL, string token, string asin)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetProductASINAlreadyExistsInASINProductDetail/" + asin);
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
                return Convert.ToInt32(strResponse);
            }
        }

        public bool DeleteASINProductImageList(string ApiURL, string token, string asin)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/DeleteASINProductImageList/" + asin);
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
                return Convert.ToBoolean(strResponse);
            }
        }
        public bool DeleteBestBuyProductZinc_ByZincID(string ApiURL, string token, string asin)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/DeleteBestBuyProductZinc_ByZincID/" + asin);
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
                return Convert.ToBoolean(strResponse);
            }
        }


        public List<string> CheckASINProductImageExist(string ApiURL, string token, string asin)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/CheckASINProductImageExist/" + asin);
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
                List<string> responses = JsonConvert.DeserializeObject<List<string>>(strResponse);
                return responses;
            }
        }



        public List<ProductZinAsinDetail> GetProductZincDetailBySKU(string ApiURL, string token, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetProductZincDetailBySKU/" + SKU);
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
                List<ProductZinAsinDetail> responses = JsonConvert.DeserializeObject<List<ProductZinAsinDetail>>(strResponse);
                return responses;
            }
        }



        public ZincProductSaveViewModel GetZincASINByID(string ApiURL, string token, int ID)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetZincDetailByID/" + ID);
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

                ZincProductSaveViewModel responses = JsonConvert.DeserializeObject<ZincProductSaveViewModel>(strResponse);

                return responses;
            }
        }
        public List<ZincProductSaveViewModel> GetZincASINBySKU(string ApiURL, string token, string sku)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetZincDetailBySKU/" + sku);
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

                List<ZincProductSaveViewModel> responses = JsonConvert.DeserializeObject<List<ZincProductSaveViewModel>>(strResponse);

                return responses;
            }

        }


        public int GetASINProductDetailCount(string ApiURL, string token, string DateTo, string DateFrom, string ASIN, string Title)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetASINProductDetailCount?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&Title=" + Title + "&ASIN=" + ASIN);
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
                        Count = Convert.ToInt32(strResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }

        public List<ASINDetailViewModel> GetASINProductDetailList(string apiurl, string token, string DateTo, string DateFrom, string ASIN, string Title, int limit, int offset)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/Zinc/GetASINProductDetailList?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&Title=" + Title + "&ASIN=" + ASIN + "&limit=" + limit + "&offset=" + offset);
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
            List<ASINDetailViewModel> responses = JsonConvert.DeserializeObject<List<ASINDetailViewModel>>(strResponse);
            return responses;
        }



        public List<ASINDetailViewModel> GetASINProductDetail(string ApiURL, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetASINProductDetail/");
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
                List<ASINDetailViewModel> responses = JsonConvert.DeserializeObject<List<ASINDetailViewModel>>(strResponse);
                return responses;
            }
        }

        public bool DeleteZincASINByID(string ApiURL, string token, int ID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/DeleteZincDetailByID/" + ID);
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
        public int SendToZincProduct(string ApiURL, string token, SendToZincProductViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/SendToZincProduct");
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
            if (strResponse != null)
            {
                return Convert.ToInt16(strResponse);
            }
            else
            {
                return 0;
            }
        }
        public int UpdateReqIDafterOrderOnZinc(string ApiURL, string token, RequestIdUpdateViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/UpdateReqIDafterOrderOnZinc");
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
            if (strResponse != null)
            {
                return Convert.ToInt16(strResponse);
            }
            else
            {
                return 0;
            }
        } 
        public int SaveZincOrderBeforeCreating(string ApiURL, string token, SendToZincProductViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/SaveZincOrderBeforeCreating");
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
            if (strResponse != null)
            {
                return Convert.ToInt16(strResponse);
            }
            else
            {
                return 0;
            }
        }
        public int SendToZinzProduct(string ApiURL, string token, SendToZincProductViewModel model)
        {
            var data = JsonConvert.SerializeObject(model);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/SendToZinzProduct");
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
            if (strResponse != null)
            {
                return Convert.ToInt16(strResponse);
            }
            else
            {
                return 0;
            }
        }
        public List<GetAddressViewModel> GetAddress(string ApiURL, string token)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetAddress/");
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
                List<GetAddressViewModel> rootObject = JsonConvert.DeserializeObject<List<GetAddressViewModel>>(strResponse);
                return rootObject;
            }
        }

        public int GetSendToZincCount(string ApiURL, string token,string Sku,string asin ,string StartDate, string EndDate)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetSendToZincOrderCount?SKU=" + Sku + "&asin="  + asin + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate);
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
                Count = JsonConvert.DeserializeObject<int>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return Count;
        }
        public List<GetSendToZincOrderViewModel> GetSendToZincOrder(string ApiURL, string token, int _offset, string Sku="", string Asin="", string FromDate = "", string ToDate = "")
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetSendToZincOrder?_offset=" + _offset+ "&Sku="+ Sku + "&Asin="+ Asin + "&FromDate=" + FromDate + "&ToDate=" + ToDate);
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

                List<GetSendToZincOrderViewModel> responses = JsonConvert.DeserializeObject<List<GetSendToZincOrderViewModel>>(strResponse);

                return responses;
            }

        }

        //public bool UpdateZincOrder(string ApiURL, string token, UpdateZincOrderViewModel model)
        //{
        //    var data = JsonConvert.SerializeObject(model);
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/UpdateZincOrder");
        //    request.Method = "POST";
        //    request.Accept = "application/json;";
        //    request.ContentType = "application/json";
        //    request.Headers["Authorization"] = "Bearer " + token;

        //    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        //    {
        //        streamWriter.Write(data);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }
        //    var response = (HttpWebResponse)request.GetResponse();
        //    string strResponse = "";
        //    using (var sr = new StreamReader(response.GetResponseStream()))
        //    {
        //        strResponse = sr.ReadToEnd();
        //    }
        //    return Convert.ToBoolean(strResponse);
        //}

        public bool UpdateZincOrder(string ApiURL, string token, UpdateZincOrderViewModel ViewModel)
        {
            bool Status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/UpdateZincOrder");
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
                   
                    Status = Convert.ToBoolean(strResponse);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Status;
        }

        public string UpdateZincOrderInternalStatus(string ApiURL, string token, int internalStatus, int orderId)
        {
            string status = "";
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/UpdateZincOrderInternalStatus?orderId=" + orderId + "&internalStatus=" + internalStatus);
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {

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
        public GetResponceFromZincViewModel GetZincResponce(string ApiURL, string token, string ASIN, string productSKU)
        {
            GetResponceFromZincViewModel model = new GetResponceFromZincViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Zinc/GetZincResponce/" + ASIN + "/" + productSKU);
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
                model = JsonConvert.DeserializeObject<GetResponceFromZincViewModel>(strResponse);
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
         
        }

    }
}
