using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.Enum;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList.Core;
using ServiceReference1;
using static Hld.WebApplication.ViewModel.SaveZincOrders;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ZincController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string ZincUserName = "";
        string request_succeeded = "";
        string request_failed = "";
        string tracking_obtained = "";
        string s3BucketURL = "";
        string s3BucketURL_large = "";

        ServiceReference1.AuthHeader authHeader = null;
        BBProductApiAccess _bBProductApiAccess = null;
        ProductApiAccess _productApiAccess = null;
        ZincApiAccess _zincApiAccess = null;
        public static IHostingEnvironment _environment;
        ZincOrderLogAndDetailApiAccess _zincOrderLogAndDetailApiAccess = null;

        SellerCloudApiAccess _sellerCloudApiAccess = null;
        EncDecChannel _encDecChannel = null;

        GetChannelCredViewModel _Selller = null;
        GetChannelCredViewModel _Zinc = null;
        GetChannelCredViewModel ZincDays = null;

        public ZincController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");
            _bBProductApiAccess = new BBProductApiAccess();
            _productApiAccess = new ProductApiAccess();
            _sellerCloudApiAccess = new SellerCloudApiAccess();
            _zincApiAccess = new ZincApiAccess();
            _zincOrderLogAndDetailApiAccess = new ZincOrderLogAndDetailApiAccess();
            request_succeeded = _configuration.GetValue<string>("WebhooksUrl:request_succeeded");
            request_failed = _configuration.GetValue<string>("WebhooksUrl:request_failed");
            tracking_obtained = _configuration.GetValue<string>("WebhooksUrl:tracking_obtained");

            _encDecChannel = new EncDecChannel();

        }


        public IActionResult Index(string sku)
        {
            token = Request.Cookies["Token"];
            List<ZincProductSaveViewModel> listViewModel = _zincApiAccess.GetZincASINBySKU(ApiURL, token, sku);
            return PartialView("~/Views/Zinc/_Index.cshtml", listViewModel);
        }
        public IActionResult ProductZinc(string ProductSKU)
        {
            token = Request.Cookies["Token"];
            TempData["ProductSKU"] = ProductSKU;
            int productID = 0;
            productID = _productApiAccess.GetProductIDBySKU(ApiURL, token, ProductSKU);
            BBProductViewModel model = _bBProductApiAccess.GetBestBuyProductDetail_ProductID(ApiURL, token, ProductSKU);
            return View(model);

        }

        public int ZincJobExecute()
        {
            token = Request.Cookies["Token"];
            _productApiAccess.ExecuteJobs(ApiURL, token);
            return 0;
        }

        [HttpGet]
        public IActionResult ZincSetting()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ZincSetting(string info)
        {
            return View();
        }
        //Mueen
        [HttpPost]
        public async Task<IActionResult> SubmitOrderToZinc(string ZincOrderdata, string orderDateTimeShip, int CreditCardId, int ZincAccountId, int DeliveryDays)
        {
            CreditCardApiAccess creditCardApiAccess = new CreditCardApiAccess();
            
            SaveZincOrders.RootObject ZincOrder = new SaveZincOrders.RootObject();
            ZincOrder = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(ZincOrderdata);
            //if (!string.IsNullOrEmpty(orderDateTimeShip))
            //{
            //    ZincOrder.shipping.max_days = Convert.ToInt32(((Convert.ToDateTime(orderDateTimeShip) - DateTime.Now).TotalDays) + 1);
            //}

            if (DeliveryDays > 0)
            {
                ZincOrder.shipping.max_days = DeliveryDays;
            }

            int _zincOrderLogID = 0;
            int SellerCloudOrderID = 0;
            string RequestID = "";
            token = Request.Cookies["Token"];

            var CreditCardDetail = creditCardApiAccess.GetCreditCardDetailById(ApiURL, token, CreditCardId);
            var AccountDetail = creditCardApiAccess.GetAccountDetailById(ApiURL, token, ZincAccountId);
          


            _Zinc = new GetChannelCredViewModel();
            //_Zinc = _encDecChannel.DecryptedData(ApiURL, token, "AmzZinc");
            //DecryptStringFromBytes_Aes
            ZincOrder.retailer_credentials = new SaveZincOrders.RetailerCredentials();
          

            ZincOrder.retailer_credentials.email = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.UserName);
            ZincOrder.retailer_credentials.password = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.Password);
            ZincOrder.retailer_credentials.totp_2fa_key = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.Key);
            ZincOrder.payment_method = new SaveZincOrders.PaymentMethod();
            ZincOrder.payment_method.use_gift = false;
        
            ZincOrder.payment_method.name_on_card = CreditCardDetail.first_name + " " + CreditCardDetail.last_name;
            ZincOrder.payment_method.number = _encDecChannel.DecryptStringFromBytes_Aes(CreditCardDetail.number);
            ZincOrder.payment_method.security_code = _encDecChannel.DecryptStringFromBytes_Aes(CreditCardDetail.security_code);
            ZincOrder.payment_method.expiration_month = Convert.ToInt32(CreditCardDetail.expiration_month);
            ZincOrder.payment_method.expiration_year = Convert.ToInt32(CreditCardDetail.expiration_year);

            ZincOrder.billing_address = new SaveZincOrders.BillingAddress();

            ZincOrder.billing_address.address_line1 = CreditCardDetail.address_line1;
            ZincOrder.billing_address.address_line2 = CreditCardDetail.address_line2;
            ZincOrder.billing_address.city = CreditCardDetail.city;
            ZincOrder.billing_address.country = CreditCardDetail.country;
            ZincOrder.billing_address.first_name = CreditCardDetail.first_name;
            ZincOrder.billing_address.last_name = CreditCardDetail.last_name;
            ZincOrder.billing_address.phone_number = CreditCardDetail.PhoneNo;
            ZincOrder.billing_address.state = CreditCardDetail.state;
            ZincOrder.billing_address.zip_code = CreditCardDetail.zip_code;
            ZincOrder.webhooks = new Webhooks();
            ZincOrder.webhooks.request_succeeded = request_succeeded;
            ZincOrder.webhooks.request_failed = request_failed;

            ZincOrder.webhooks.tracking_obtained = tracking_obtained;
            //  ZincOrder.webhooks = _webhooks;
            token = Request.Cookies["Token"];

            ///////Commented By me

            SellerCloudOrderID = ZincOrder.client_notes.our_internal_order_id;
            RequestID = SubmitOrderToZincForSave(ZincOrder);



            if (RequestID != string.Empty)
            {
                bool status = false;
                UpdateSCDropshipStatusViewModel updateSCViewModel = new UpdateSCDropshipStatusViewModel();
                updateSCViewModel.StatusName = "Requested";
                updateSCViewModel.SCOrderID = ZincOrder.client_notes.our_internal_order_id;
                updateSCViewModel.LogDate = DatetimeExtension.ConvertToEST(DateTime.Now);

                status = await UpdateDropShipStatusInZinc(ZincOrder.client_notes.our_internal_order_id, "Requested");
                status = _sellerCloudApiAccess.UpdateSellerCloudOrderDropShipStatus(ApiURL, token, updateSCViewModel);

                if (status)
                {
                    ZincOrderLogViewModel zincOrderLogViewModel = new ZincOrderLogViewModel();
                    zincOrderLogViewModel.SellerCloudOrderId = Convert.ToString(SellerCloudOrderID);
                    zincOrderLogViewModel.RequestIDOfZincOrder = RequestID;
                    zincOrderLogViewModel.OrderDatetime = DatetimeExtension.ConvertToEST(DateTime.Now); ;
                    _zincOrderLogID = _zincOrderLogAndDetailApiAccess.SaveZincOrderLog(ApiURL, token, zincOrderLogViewModel);

                    ZincOrderLogDetailViewModel model = new ZincOrderLogDetailViewModel();
                    model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.OrderRequestSent.ToString();
                    model.ZincOrderLogID = _zincOrderLogID;
                    int ZincOrderLogDetailID = _zincOrderLogAndDetailApiAccess.SaveZincOrderLogDetail(ApiURL, token, model);
                    _zincOrderLogAndDetailApiAccess.UpdateAccounts(ApiURL, token, Convert.ToInt32(zincOrderLogViewModel.SellerCloudOrderId), ZincAccountId, CreditCardId);
                }
            }
            return Json(new { requestid = RequestID, zincorderlogid = _zincOrderLogID, zincrequestid = RequestID });
            //return Json(new { requestid = "1234", zincorderlogid = "1324", zincrequestid = "1234" });
        }

        [HttpGet]
        public IActionResult ReteriveOrderStatusFromZinc(string zincOrderLogID, string zincRequestID, string sellerCloudID)
        {
            ZincOrderLogDetailViewModel model = new ZincOrderLogDetailViewModel();
            int ZincOrderLogDetailID = 0;
            string zincStatus = "";
            string zincCode = "";
            string zincMessage = "";
            bool status = false;
            token = Request.Cookies["Token"];
            _Zinc = new GetChannelCredViewModel();
            _Zinc = _encDecChannel.DecryptedData(ApiURL, token, "Zinc");
            ZincUserName = _Zinc.Key;
            try
            {
                string uri = "https://api.zinc.io/v1/orders/" + zincRequestID;
                string response = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Credentials = new NetworkCredential(ZincUserName, "");

                using (var webResponse = request.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        response = new StreamReader(responseStream).ReadToEnd();
                    }
                }
                var X = JObject.Parse(response);
                var type = X["_type"];
                var code = X["code"];
                var message = X["message"];
                var extra = X["extra"];
                string commonMessage = "";
                var delivery_dates = X["delivery_dates"];
                var tracking = X["tracking"];

                if (delivery_dates != null && delivery_dates.Count() != 0)
                {
                    commonMessage = delivery_dates[0].Value<string>("delivery_date").ToString();

                }

                if (message != null)
                {
                    commonMessage = message.ToString();
                }



                model.Message = commonMessage;

                //setting internal zinc order status
                if (type != null)
                {
                    if (type.ToString() == "order_response" && delivery_dates != null)
                    {
                        model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.InProgressSuccess.ToString();
                    }
                    else if (type.ToString() == "order_response")
                    {
                        model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.InProcess.ToString();
                    }
                    else
                    {
                        model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.Error.ToString();
                    }

                    model.Type = type.ToString();
                }


                if (type != null && code != null)
                {
                    if (type.ToString() == "error" && code.ToString() == "request_processing")
                    {
                        model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.InProcess.ToString();
                    }
                    else
                    {
                        model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.Error.ToString();
                    }

                    if (code.ToString() == "zma_temporarily_overloaded")
                    {
                        commonMessage = "We are experiencing a very high volume of orders and are temporarily unable to process this order. Please retry in a few minutes.";
                    }


                    model.Type = type.ToString();
                    model.Code = code.ToString();
                }


                //checking tracking of an zinc order
                if (tracking != null)
                {
                    if (!string.IsNullOrEmpty(tracking[0].Value<string>("tracking_number")) && tracking[0].Value<string>("delivery_status") != "Canceled")
                    {
                        if (!string.IsNullOrEmpty(tracking[0].Value<string>("obtained_at")))
                        {
                            model.ShppingDate = tracking[0].Value<string>("obtained_at").ToString();
                        }
                        if (!string.IsNullOrEmpty(tracking[0].Value<string>("tracking_number")))
                        {
                            model.TrackingNumber = tracking[0].Value<string>("tracking_number").ToString();
                        }
                        if (!string.IsNullOrEmpty(tracking[0].Value<string>("carrier")))
                        {
                            model.Carrier = tracking[0].Value<string>("carrier").ToString();
                        }
                        if (!string.IsNullOrEmpty(tracking[0].Value<string>("merchant_order_id")))
                        {
                            model.MerchantOrderId = tracking[0].Value<string>("merchant_order_id").ToString();
                        }
                        if (!string.IsNullOrEmpty(tracking[0].Value<string>("tracking_url")))
                        {
                            model._17Tracking = tracking[0].Value<string>("tracking_url").ToString();
                        }
                        if (!string.IsNullOrEmpty(tracking[0].Value<string>("retailer_tracking_url")))
                        {
                            model.AmazonTracking = tracking[0].Value<string>("retailer_tracking_url").ToString();
                        }
                        if (type != null)
                        {
                            if (type.ToString() == "order_response")
                            {
                                model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.Shipped.ToString();
                            }
                        }
                    }
                    else if (tracking[0].Value<string>("delivery_status") == "Canceled")
                    {
                        model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.Canceled.ToString();
                    }
                }

                model.OrderDatetime = DatetimeExtension.ConvertToEST(DateTime.Now);
                model.ZincOrderLogID = Convert.ToInt32(zincOrderLogID);
                zincStatus = model.ZincOrderStatusInternal;
                zincCode = model.Code;
                zincMessage = model.Message;
                ZincOrderLogDetailID = _zincOrderLogAndDetailApiAccess.SaveZincOrderLogDetail(ApiURL, token, model);
                status = true;
            }
            catch (Exception ex)
            {

            }

            return Json(new { success = status, zincOrderLogDetailID = ZincOrderLogDetailID, ZincStatus = zincStatus, ZincCode = zincCode, ZincMessage = zincMessage });
        }

        public IActionResult SendOrderToZinc(string ASIN, string MaxPrice, string orderid, string SellerOrderID, string orderDetailID)
        {
            token = Request.Cookies["Token"];

            //SaveZincOrders.RootObject rootObject = _zincApiAccess.GetCustomerDetailForSendOrderToZinc(ApiURL, token,ASIN,MaxPrice,orderid,SellerOrderID,orderDetailID);
            string rootObject = _zincApiAccess.GetCustomerDetailForSendOrderToZinc(ApiURL, token, ASIN, MaxPrice, orderid, SellerOrderID, orderDetailID);
            return Json(rootObject);
        }

        public IActionResult ConfirmationOfZincOrder(string JsonData, decimal profitlose, decimal percentage)
        {
            token = Request.Cookies["Token"];
            ViewBag.profitloseorderdetail = profitlose;
            ViewBag.percentageorderdetail = percentage;

            SaveZincOrders.RootObject rootObject = JsonConvert.DeserializeObject<SaveZincOrders.RootObject>(JsonData);

            var Object = _zincApiAccess.GetActiveZincAccountsList(ApiURL, token);
            rootObject.CreditCardDetail = Object.CreditCardDetail;
            rootObject.ZincAccounts = Object.ZincAccounts;
            
            return PartialView("~/Views/Zinc/ConfirmationOfZincOrder.cshtml", rootObject);
        }
        [HttpGet]
        public IActionResult SendToZincProduct(GetSKUAndASINViewModel Data)
        {
            token = Request.Cookies["Token"];

            SendToZincProductViewModel rootObject = new SendToZincProductViewModel();
            var Object = _zincApiAccess.GetActiveZincAccountsList(ApiURL, token);
            rootObject.getaddress = _zincApiAccess.GetAddress(ApiURL, token);
            rootObject.CreditCardDetail = Object.CreditCardDetail;
            rootObject.ZincAccounts = Object.ZincAccounts;
            rootObject.Sku = Data.SKU;
            rootObject.Asin = Data.ASIN;
            rootObject.max_price = Data.max_price;
            return PartialView("~/Views/Zinc/SendToZincProduct.cshtml", rootObject);
        }
        [HttpGet]
        public IActionResult UpdateZincOrder(GetZincOrderViewModel Data)
        {
            token = Request.Cookies["Token"];

            GetSendToZincOrderViewModel rootObject = new GetSendToZincOrderViewModel();
            //var Object = _zincApiAccess.GetActiveZincAccountsList(ApiURL, token);
            //rootObject.getaddress = _zincApiAccess.GetAddress(ApiURL, token);
            //rootObject.CreditCardDetail = Object.CreditCardDetail;
            //rootObject.ZincAccounts = Object.ZincAccounts;
            rootObject.Sku = Data.SKU;
            rootObject.Asin = Data.ASIN;
            rootObject.Price = Data.max_price;
            rootObject.OrderId = Data.OrderId;
            return PartialView("~/Views/Zinc/UpdateZincOrder.cshtml", rootObject);
        }

        public IActionResult GetZincProduct(string sku)
        {
            token = Request.Cookies["Token"];
            List<ZincProductSaveViewModel> listViewModel = _zincApiAccess.GetZincASINBySKU(ApiURL, token, sku);
            return PartialView("~/Views/Zinc/_GetModelData.cshtml", listViewModel);
        }

        [HttpPost]
        public IActionResult SubmitZincProduct(SendToZincProductViewModel model)
        {
            SaveZincOrders.RootObject ZincOrder = new SaveZincOrders.RootObject();
            CreditCardApiAccess creditCardApiAccess = new CreditCardApiAccess();
            token = Request.Cookies["Token"];
            string RequestID = "";
            var CreditCardDetail = creditCardApiAccess.GetCreditCardDetailById(ApiURL, token, model.CreditCardId);
            var AccountDetail = creditCardApiAccess.GetAccountDetailById(ApiURL, token, model.ZincAccountId);

         

            _Zinc = _encDecChannel.DecryptedData(ApiURL, token, "Zinc");
            ZincDays = _encDecChannel.DecryptedData(ApiURL, token, "ZincDays");
         
            ZincOrder.retailer = "amazon_ca";
            ZincOrder.max_price = Convert.ToInt32(model.max_price * model.Qty);
            ZincOrder.shipping_address = new SaveZincOrders.ShippingAddress();

            ZincOrder.shipping_address.first_name = CreditCardDetail.first_name;
            ZincOrder.shipping_address.last_name = CreditCardDetail.last_name;
            ZincOrder.shipping_address.phone_number = CreditCardDetail.PhoneNo;
            ZincOrder.shipping_address.state = CreditCardDetail.state;
            ZincOrder.shipping_address.zip_code = CreditCardDetail.zip_code;
            ZincOrder.shipping_address.city = CreditCardDetail.city;
            ZincOrder.shipping_address.address_line1 = CreditCardDetail.address_line1;
            ZincOrder.shipping_address.address_line2 = CreditCardDetail.address_line2;
            ZincOrder.shipping_address.country = "CA";
            ZincOrder.is_gift = false;
            ZincOrder.gift_message = "";
            ZincOrder.shipping = new SaveZincOrders.Shipping();
        
            if (!model.Shipdays.Equals("0"))
            {
                ZincOrder.shipping.max_days = Convert.ToInt32(model.Shipdays);
            }
            else {
                ZincOrder.shipping.max_days = Convert.ToInt32(ZincDays.password);
            }

            model.Shipdays = Convert.ToString(ZincOrder.shipping.max_days);
            model.FirstName = CreditCardDetail.first_name;
            model.LastName = CreditCardDetail.last_name;
            model.Phone = CreditCardDetail.PhoneNo;
            model.State = CreditCardDetail.state;
          
            model.City = CreditCardDetail.city;
            model.Address1 = CreditCardDetail.country;
            model.Address2 = CreditCardDetail.address_line2;
            model.Country = "CA";
            int OrderID = _zincApiAccess.SaveZincOrderBeforeCreating(ApiURL, token, model);
            ZincOrder.shipping.max_price = 0;
            ZincOrder.shipping.order_by = "price";
            ZincOrder.client_notes = new SaveZincOrders.ClientNotes();
            ZincOrder.client_notes.our_internal_order_id = OrderID;
            ZincOrder.products = new List<SaveZincOrders.Product>();
            ZincOrder.products.Add(new SaveZincOrders.Product()
            {
                product_id = model.Asin,
                quantity = model.Qty
            });

            ZincOrder.retailer_credentials = new SaveZincOrders.RetailerCredentials();
            ZincOrder.retailer_credentials.email = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.UserName);
            ZincOrder.retailer_credentials.password = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.Password);
            ZincOrder.retailer_credentials.totp_2fa_key = _encDecChannel.DecryptStringFromBytes_Aes(AccountDetail.Key);
            ZincOrder.payment_method = new SaveZincOrders.PaymentMethod();
            ZincOrder.payment_method.use_gift = false;

            ZincOrder.payment_method.name_on_card = CreditCardDetail.first_name + " " + CreditCardDetail.last_name;
            ZincOrder.payment_method.number = _encDecChannel.DecryptStringFromBytes_Aes(CreditCardDetail.number);
            ZincOrder.payment_method.security_code = _encDecChannel.DecryptStringFromBytes_Aes(CreditCardDetail.security_code);
            ZincOrder.payment_method.expiration_month = Convert.ToInt32(CreditCardDetail.expiration_month);
            ZincOrder.payment_method.expiration_year = Convert.ToInt32(CreditCardDetail.expiration_year);

            ZincOrder.billing_address = new SaveZincOrders.BillingAddress();

            ZincOrder.billing_address.address_line1 = CreditCardDetail.address_line1;
            ZincOrder.billing_address.address_line2 = CreditCardDetail.address_line2;
            ZincOrder.billing_address.city = CreditCardDetail.city;
            ZincOrder.billing_address.country = CreditCardDetail.country;
            ZincOrder.billing_address.first_name = CreditCardDetail.first_name;
            ZincOrder.billing_address.last_name = CreditCardDetail.last_name;
            ZincOrder.billing_address.phone_number = CreditCardDetail.PhoneNo;
            ZincOrder.billing_address.state = CreditCardDetail.state;
            ZincOrder.billing_address.zip_code = CreditCardDetail.zip_code;
            ZincOrder.webhooks = new Webhooks();
            ZincOrder.webhooks.request_succeeded = "http://apitest-prod.us-east-2.elasticbeanstalk.com/api/ProductWebhooks/Success";
            ZincOrder.webhooks.request_failed = "http://apitest-prod.us-east-2.elasticbeanstalk.com/api/ProductWebhooks/failure";
            ZincOrder.webhooks.tracking_obtained = "http://apitest-prod.us-east-2.elasticbeanstalk.com/api/ProductWebhooks/tracking";

      
            RequestID = "ABC";
            //RequestID = SubmitOrderToZincForSave(ZincOrder);
            model.ReqId = RequestID;

            //int status = _zincApiAccess.SendToZincProduct(ApiURL, token, model);
            RequestIdUpdateViewModel requestIdUpdate = new RequestIdUpdateViewModel();
            requestIdUpdate._OrderId = OrderID;
            requestIdUpdate._ReqId = RequestID;
            requestIdUpdate._order_message = "";
            int status = _zincApiAccess.UpdateReqIDafterOrderOnZinc(ApiURL, token, requestIdUpdate);
           

            return Json(new {RequestID=RequestID,OrderID=OrderID, success = true, message = "Save successfully" });
        }
        [HttpPost]
        public IActionResult UpdateZincOrder(UpdateZincOrderViewModel data)
        {
            token = Request.Cookies["Token"];
            _zincApiAccess.UpdateZincOrder(ApiURL, token, data);
            return Json(new {success = true, message = "Save successfully" });
        }
        [HttpGet]
        public IActionResult GetAddress()
        {
            token = Request.Cookies["Token"];
            GetAddressViewModel rootObject = new GetAddressViewModel();
            var Object = _zincApiAccess.GetAddress(ApiURL, token);
            return PartialView("~/Views/Zinc/SendToZincProduct.cshtml", rootObject);
        }

        [HttpGet]
        public IActionResult AddUpdateZinc(int id = 0)
        {
            token = Request.Cookies["Token"];
            ZincProductSaveViewModel zincProductSaveViewModel = new ZincProductSaveViewModel(); ;

            if (id > 0)
            {
                zincProductSaveViewModel = _zincApiAccess.GetZincASINByID(ApiURL, token, id);
                zincProductSaveViewModel.max_price_limit = Convert.ToString(Convert.ToDecimal(zincProductSaveViewModel.max_price_limit) / 100);
                zincProductSaveViewModel.itemprice = zincProductSaveViewModel.itemprice / 100;

                return PartialView("~/Views/Zinc/_UpdateZinc.cshtml", zincProductSaveViewModel);
            }
            else
            {
                return PartialView("~/Views/Zinc/_UpdateZinc.cshtml", zincProductSaveViewModel);
            }
        }

        public string SubmitOrderToZincForSave(SaveZincOrders.RootObject ZincOrder)
        {
           


            string requestID = "";
            try
            {
                token = Request.Cookies["Token"];
                _Zinc = new GetChannelCredViewModel();
                _Zinc = _encDecChannel.DecryptedData(ApiURL, token, "Zinc");
                ZincUserName = _Zinc.Key;

                var data = JsonConvert.SerializeObject(ZincOrder);
                string uri = "https://api.zinc.io/v1/orders/";
                string response = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Credentials = new NetworkCredential(ZincUserName, "");

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (var webResponse = request.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        response = new StreamReader(responseStream).ReadToEnd();
                    }
                }

                var requestIDFromZinc = JObject.Parse(response)["request_id"].ToString();
                if (requestIDFromZinc != null)
                {
                    requestID = requestIDFromZinc;
                }
                else
                {
                    requestID = "";
                }

            }
            catch (Exception ex)
            {

            }
            return requestID;
        }



        /// <summary>
        /// ASIN offer detail 
        /// </summary>
        /// <param name="orderid">ASIN</param>
        /// <param name="ProductSKU">it can be null or empty in case of getting zinc detail from other controllers</param>
        /// <returns></returns>
        public ZincProductSaveViewModel GetASINDetailFromZinc(string orderid, string ProductSKU)
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


        public ASINDetailViewModel ExtractProductDetailThroughASINProduct(ASINDetail.RootObject rootObject)
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

        public ASINDetail.RootObject GetProductDetailFromZinc(string ASIN)
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


        


        public IActionResult GetProductOfferDataFromZinc(string orderid, string ProductSKU)
        {
            token = Request.Cookies["Token"];
            ZincProductSaveViewModel zincProductSaveViewModel = null;
            int count = _zincApiAccess.GetProductASINAlreadyExists(ApiURL, token, ProductSKU, orderid);
            if (count == 0)
            {
                zincProductSaveViewModel = GetASINDetailFromZinc(orderid, ProductSKU);
                zincProductSaveViewModel.max_price_limit = Convert.ToString(Convert.ToInt32(zincProductSaveViewModel.itemprice) * 1.25);
                return PartialView("~/Views/Zinc/_AddUpdateZinc.cshtml", zincProductSaveViewModel);
            }
            else
            {
                ViewBag.ASIN = orderid;
                ViewBag.ProductSKU = ProductSKU;
                return PartialView("~/Views/Zinc/ZincASINAlreadyExists.cshtml");
            }

        }


        [HttpPost]
        public IActionResult AddUpdateZinc(ZincProductSaveViewModel model, [FromQuery(Name = "sku")] string sku)
        {
            token = Request.Cookies["Token"];
            if (model.bb_product_zinc_id > 0)
            {
                bool status = _zincApiAccess.UpdateZincProductASIN(ApiURL, token, model);
                return Json(new { success = true, message = "Update successfully" });
            }
            else
            {
                model.Product_sku = sku.Trim();
                int status = _zincApiAccess.SaveZincProductASIN(ApiURL, token, model);
            }

            return Json(new { success = true, message = "Save successfully" });
        }
        [HttpPost]
        public IActionResult AddUpdateZincWithdollerPrice(ZincProductSaveViewModel model, [FromQuery(Name = "sku")] string sku)
        {
            token = Request.Cookies["Token"];
            model.max_price_limit = Convert.ToString(Convert.ToDecimal(model.max_price_limit) * 100);
            model.itemprice = model.itemprice * 100;
            if (model.bb_product_zinc_id > 0)
            {
                bool status = _zincApiAccess.UpdateZincProductASIN(ApiURL, token, model);
                return Json(new { success = true, message = "Update successfully" });
            }
            else
            {
                model.Product_sku = sku.Trim();
                int status = _zincApiAccess.SaveZincProductASIN(ApiURL, token, model);
            }

            return Json(new { success = true, message = "Save successfully" });
        }

        [HttpPost]
        public IActionResult DeleteProductASINZincDetail(int id)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            if (id > 0)
            {
                status = _zincApiAccess.DeleteZincASINByID(ApiURL, token, id);
            }
            return Json(new { success = status, message = "Deleted Successfully" });
        }


        public IActionResult GetZincOrderLogDetailByID(string zincOrderLogDetailID, string type)
        {
            ZincOrderLogDetailViewModel model = new ZincOrderLogDetailViewModel();
            token = Request.Cookies["Token"];
            if (!string.IsNullOrEmpty(zincOrderLogDetailID) && type == "Shipped")
            {
                model = _zincOrderLogAndDetailApiAccess.GetZincOrderLogDetailById(ApiURL, token, zincOrderLogDetailID);
            }
            return PartialView("~/Views/Zinc/_ZincOrderLogDetail.cshtml", model);
        }

        public IActionResult ProcessedOutSideZinc(string zincOrderLogID)
        {
            token = Request.Cookies["Token"];
            ZincOrderLogDetailViewModel model = new ZincOrderLogDetailViewModel();
            model.ZincOrderLogID = Convert.ToInt32(zincOrderLogID);
            model.ZincOrderStatusInternal = ZincOrderLogInternalStatus.Processed_OutSideZinc.ToString();
            _zincOrderLogAndDetailApiAccess.SaveZincOrderLogDetail(ApiURL, token, model);
            return Json(new { message = "Processed_OutSideZinc" });
        }

        public async Task<bool> UpdateDropShipStatusInZinc(int orderid, string DropShipStatusName)
        {
            bool status = false;
            try
            {
                token = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                authHeader = new AuthHeader();
                authHeader.ValidateDeviceID = false;
                authHeader.UserName = _Selller.UserName;
                authHeader.Password = _Selller.Key;
                if (DropShipStatusName == "Requested")
                {
                    ServiceReference1.SCServiceSoapClient sCServiceSoap =
                                           new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                    var request = await sCServiceSoap.UpdateOrderDropShipStatusAsync(authHeader, null, orderid, DropShipStatusType.Requested);
                    bool response = request.UpdateOrderDropShipStatusResult;
                    status = response;
                }
                if (DropShipStatusName == "None")
                {
                    ServiceReference1.SCServiceSoapClient sCServiceSoap =
                                           new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                    var request = await sCServiceSoap.UpdateOrderDropShipStatusAsync(authHeader, null, orderid, DropShipStatusType.None);
                    bool response = request.UpdateOrderDropShipStatusResult;
                    status = response;
                }
            }


            catch (Exception ex) { }
            return status;
        }

        public IActionResult GetSendToZincCount(string Sku, string Asin, DateTime FromDate , DateTime ToDate)
        {
            GetSendToZincOrderViewModel getSendTo = new GetSendToZincOrderViewModel();
            getSendTo.Sku = Sku;
            getSendTo.Asin = Asin;
            string token = Request.Cookies["Token"];
            int count = 0;
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != FromDate.ToString("yyyy-MM-dd"))
            {

                CurrentDate = FromDate.ToString("yyyy-MM-dd");
                PreviousDate = ToDate.ToString("yyyy-MM-dd");
            }
            

            count = _zincApiAccess.GetSendToZincCount(ApiURL,token,Sku,Asin, CurrentDate, PreviousDate);
            ViewBag.Records = count;
            return View(getSendTo);
        }

        public IActionResult GetSendToZincOrder( string Sku, string Asin, DateTime FromDate, DateTime ToDate,int page = 0)
        {
            IPagedList<GetSendToZincOrderViewModel> data = null;
            string token = Request.Cookies["Token"];
            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");

            if ("0001-01-01" != FromDate.ToString("yyyy-MM-dd"))
            {

                CurrentDate = FromDate.ToString("yyyy-MM-dd");
                PreviousDate = ToDate.ToString("yyyy-MM-dd");
            }

            int pageNumber = page;
            int _offset = 0;
            int pageSize = 25;

            _offset = (pageNumber - 1) * 25;
            List<GetSendToZincOrderViewModel> listViewModel = new List<GetSendToZincOrderViewModel>();
            listViewModel = _zincApiAccess.GetSendToZincOrder(ApiURL, token, _offset,Sku,Asin, CurrentDate, PreviousDate);
            
            
            data = new StaticPagedList<GetSendToZincOrderViewModel>(listViewModel, pageNumber, pageSize, listViewModel.Count);
            foreach (var item in listViewModel)
            {
                item.name_on_card = _encDecChannel.DecryptStringFromBytes_Aes(item.name_on_card);
                item.UserName = _encDecChannel.DecryptStringFromBytes_Aes(item.UserName);
            }
            ViewBag.S3BucketURL = s3BucketURL;
            ViewBag.S3BucketURL_large = s3BucketURL_large;
            return PartialView("~/Views/Zinc/_ZendToZincPartialView.cshtml", data);

        }

    }
}