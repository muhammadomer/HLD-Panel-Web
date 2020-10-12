using DataAccess.ViewModels;
using Hld.WebApplication.ViewModel;
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
    public class PurchaseOrderApiAccess
    {
        public PurchaseOrderViewModel.PurchaseOrderData GetPurchaseOrderByIdFromSellerCloud(string token, string ApiURL, string OrderID, string LocalAPI, string LocalToken)
        {
            PurchaseOrderViewModel.PurchaseOrderData responses = new PurchaseOrderViewModel.PurchaseOrderData();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/PurchaseOrders/" + OrderID);
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


                responses = JsonConvert.DeserializeObject<PurchaseOrderViewModel.PurchaseOrderData>(strResponse);

            }
            catch (WebException ex)
            {
                var message = ex.Message;
                if (message == "The remote server returned an error: (500) Internal Server Error.")
                {
                    DeletePO(LocalAPI, LocalToken, Convert.ToInt32(OrderID));
                }
                return responses;
            }
            return responses;
        }

        public bool DeletePO(string ApiURL, string token, int Id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/DeletePOByPOId?Id=" + Id);
            request.Method = "Delete";
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
            var responses = Convert.ToBoolean(strResponse);
            return responses;
        }

        public bool SavepurchaseOrder(string token, string ApiURL, PurchaseOrderDataViewModel purchaseOrderDataViewModel)
        {
            bool status = false;

            try
            {
                var data = JsonConvert.SerializeObject(purchaseOrderDataViewModel);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/save");
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
            catch (WebException ex)
            {

            }
            return status;
        }
        public bool UpdatepurchaseOrder(string token, string ApiURL, PurchaseOrderDataViewModel purchaseOrderDataViewModel)
        {
            bool status = false;

            try
            {
                var data = JsonConvert.SerializeObject(purchaseOrderDataViewModel);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/Update");
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
            catch (WebException ex)
            {

            }
            return status;
        }


        public int GetAllPurchaseOrdersCount(string ApiURL, string token, string StartDate, string EndDate, int VendorId, int POID = 0, string OpenItem = "", string ReceivedItem = "", string OrderdItem = "",string NotShipped="")
        {
            int responses = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/GetPOCount?VendorId=" + VendorId + "&POID=" + POID + "&OpenItem=" + OpenItem + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&NotShipped=" + NotShipped);
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
                responses = Convert.ToInt32(strResponse);
            }
            catch (Exception ex)
            {

                throw;
            }
            return responses;
        }

        public GetSummaryForPOViewModel GetAllPurchaseOrdersForPO(string CurrentDate, string PreviousDate, string ApiURL, string token, int VendorId, int POID = 0, string OpenItem = "", string ReceivedItem = "", string OrderdItem = "",string NotShipped="")
        {
            GetSummaryForPOViewModel obj = new GetSummaryForPOViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/GetTableHeaderForPO?VendorId=" + VendorId + "&POID=" + POID + "&OpenItem=" + OpenItem + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem + "&CurrentDate=" + CurrentDate + "&PreviousDate=" + PreviousDate + "&NotShipped=" + NotShipped);
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
                obj = JsonConvert.DeserializeObject<GetSummaryForPOViewModel>(strResponse);
            }
            catch (Exception ex)
            {

                throw;
            }
            return obj;
        }

        public List<PurchaseOrdersViewModel> GetAllPurchaseOrders(string apiurl, string token, string StartDate, string EndDate, int VendorId, int Limit, int OffSet, int POID = 0, string OpenItem = "", string ReceivedItem = "", string OrderdItem = "",string NotShipped = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
            .Create(apiurl + "/api/PurchaseOrder/GetPO?VendorId=" + VendorId + "&POID=" + POID + "&limit=" + Limit + "&offSet=" + OffSet + "&OpenItem=" + OpenItem + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&NotShipped=" + NotShipped);
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
                }
            }
            catch (Exception exp) { }
            //List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            List<PurchaseOrdersViewModel> responses = JsonConvert.DeserializeObject<List<PurchaseOrdersViewModel>>(strResponse);
            return responses;
        }

        public PurchaseOrderModel GetAllPurchaseOrdersItems(string ApiURL, string token, searchPOitemViewModel ViewModel)
        {
            var data = JsonConvert.SerializeObject(ViewModel);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/GetPOitems");
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
            PurchaseOrderModel responses = JsonConvert.DeserializeObject<PurchaseOrderModel>(strResponse);

            return responses;
        }

        public bool SavePOAsAccepted(string token, string ApiURL, UpdatePOAcceptedViewModel updatePOAccepted)
        {
            bool status = false;

            try
            {
                var data = JsonConvert.SerializeObject(updatePOAccepted);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/POAsAccepted");
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
            catch (WebException ex)
            {

            }
            return status;
        }
        public bool UpdatePOShipDate(string token, string ApiURL, UpdatePOAcceptedViewModel updatePOAccepted)
        {
            bool status = false;

            try
            {
                var data = JsonConvert.SerializeObject(updatePOAccepted);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/UpdatePOShipDate");
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
            catch (WebException ex)
            {

            }
            return status;
        }
        public bool SaveCurrencyExchange(string token, string ApiURL, UpdatePOExchangeViewModel exchangeViewModel)
        {
            bool status = false;

            try
            {
                var data = JsonConvert.SerializeObject(exchangeViewModel);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/POcurrExchange");
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
            catch (WebException ex)
            {

            }
            return status;
        }

        public UpdatePOAcceptedViewModel GetPOShiDatestoupdate(string ApiURL, string token, int POid)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/GetShipdates/" + POid);
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
            UpdatePOAcceptedViewModel responses = JsonConvert.DeserializeObject<UpdatePOAcceptedViewModel>(strResponse);

            return responses;
        }

        public MissingOrderReturnViewModel CheckPOOrderINDB(string ApiURL, string token, List<CheckMissingOrderViewModel> OrderList)
        {
            try
            {
                var data = JsonConvert.SerializeObject(OrderList);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/CheckPOOrder");
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

                MissingOrderReturnViewModel status = JsonConvert.DeserializeObject<MissingOrderReturnViewModel>(strResponse);
                return status;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public List<PurchaseOrderItemsViewModel> GetPOProductsList(string apiurl, string token, string StartDate, string EndDate, int VendorId, int Limit, int OffSet, int POID = 0, string SKU = "", string title = "", string OpenItem = "", string ReceivedItem = "", string OrderdItem = "", string NotShipped = "",string ShippedButNotReceived="")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/PurchaseOrder/GetPOProductDetails?VendorId=" + VendorId + "&limit=" + Limit + "&offSet=" + OffSet + "&POID=" + POID + "&sku=" + SKU + "&title=" + title + "&OpenItem=" + OpenItem + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&NotShipped=" + NotShipped + "&ShippedButNotReceived=" + ShippedButNotReceived);
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
                }
            }
            catch (Exception exp) { }
            //List<ApprovedPriceViewModel> responses = JsonConvert.DeserializeObject<List<ApprovedPriceViewModel>>(strResponse);
            List<PurchaseOrderItemsViewModel> responses = JsonConvert.DeserializeObject<List<PurchaseOrderItemsViewModel>>(strResponse);
            return responses;
        }

        public GetSummaryandCountPOViewModel GetCounter(string ApiURL, string token, string StartDate, string EndDate, int VendorId, int POID = 0, string SKU = "", string title = "", string OpenItem = "", string ReceivedItem = "", string OrderdItem = "", string NotShipped = "", string ShippedButNotReceived = "")
        {
            GetSummaryandCountPOViewModel responses = new GetSummaryandCountPOViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/GetCount?VendorId=" + VendorId + "&POID=" + POID + "&SKU=" + SKU + "&title=" + title + "&OpenItem=" + OpenItem + "&ReceivedItem=" + ReceivedItem + "&OrderItem=" + OrderdItem + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&NotShipped=" + NotShipped + "&ShippedButNotReceived=" + ShippedButNotReceived);
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
                responses = JsonConvert.DeserializeObject<GetSummaryandCountPOViewModel>(strResponse);
            }
            catch (Exception ex)
            {

                throw;
            }
            return responses;
        }

        public List<ProductSKUViewModel> GetAllSKUByNameForPO(string apiurl, string token, string name, int POID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/PurchaseOrder/SKU/" + name + "/" + POID);
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


        public List<ProductSKUViewModel> GetAllSKUByName(string apiurl, string token, string name, int VendorId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/PurchaseOrder/GetAllSKUByName/" + name + "/" + VendorId);
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
        public List<POIdViewModel> GetPOIdBySku(string apiurl, string token, string name, int VendorId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiurl + "/api/PurchaseOrder/GetPOIdBySku/" + name + "/" + VendorId);
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
            List<POIdViewModel> responses = JsonConvert.DeserializeObject<List<POIdViewModel>>(strResponse);
            return responses;
        }
        public int UpdateCurrency(string ApiURL, string token, int POID, int CurrencyCode)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/UpdateCurrencyCode?POId=" + POID + "&CurrencyCode=" + CurrencyCode);
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
            int responses = Convert.ToInt32(strResponse);

            return responses;
        }


        public bool DeleteItem(string token, string ApiURL, int ID)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/deleteItem/" + ID);
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
            bool responses = Convert.ToBoolean(strResponse);

            return responses;
        }

        public bool UpdateTitle(string ApiURL, string token, string Sku, string Title)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PurchaseOrder/UpdateTitle?Sku=" + Sku + "&Title=" + Title);
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


    }
}
