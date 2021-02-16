using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceReference1;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using Newtonsoft.Json;
using Hld.WebApplication.Enum;
using ImageMagick;
using ImageMagick.Web;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Controllers
{


    [TokenExpires]
    public class SellerCloudController : Controller
    {

        ServiceReference1.AuthHeader authHeader = null;
        SellerCloudApiAccess sellerCloudApiAccess = null;
        BestBuyOrdersApiAccess bestBuyOrdersApiAccess = null;
        UploadFilesToS3 uploadFiles = null;
        ProductApiAccess ProductapiAccess = null;
        ZincOrderLogAndDetailApiAccess _zincOrderLogAndDetailApiAccess = null;
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string ZincUserName = "";
        string SCRestURL = "";
        OrderNotesAPiAccess _OrderApiAccess = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        GetChannelCredViewModel _Zinc = null;
        GetChannelCredViewModel _bestbuy = null;

        public static IHostingEnvironment _environment;




        public SellerCloudController(IConfiguration configuration, IHostingEnvironment environment)
        {

            _environment = environment;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
            _encDecChannel = new EncDecChannel();
            sellerCloudApiAccess = new SellerCloudApiAccess();
            bestBuyOrdersApiAccess = new BestBuyOrdersApiAccess();
            _zincOrderLogAndDetailApiAccess = new ZincOrderLogAndDetailApiAccess();
            ProductapiAccess = new ProductApiAccess();
            uploadFiles = new UploadFilesToS3(_environment, _configuration);
            _OrderApiAccess = new OrderNotesAPiAccess();
        }

        public async Task<IActionResult> GetProductImagesFromSellerCloud()
        {
            bool status = false;
            int _successCounter = 0;
            int _failureCounter = 0;
            try
            {
                token = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                authHeader = new AuthHeader();
                authHeader.ValidateDeviceID = false;
                authHeader.UserName = _Selller.UserName;
                authHeader.Password = _Selller.Key;

                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                       new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                List<string> skuList = sellerCloudApiAccess.GetSKUWhichImagesNotExists(ApiURL, token);
                List<ImagesSaveToDatabaseWithURLViewMOdel> listImagesUrl = new List<ImagesSaveToDatabaseWithURLViewMOdel>();
                foreach (var item in skuList)
                {

                    var imageURL = await sCServiceSoap.GetGalleryImageURLAsync(authHeader, null, item.Trim());
                    if (imageURL.GetGalleryImageURLResult != null && imageURL.GetGalleryImageURLResult != "")
                    {
                        Image img = DownloadImageFromUrl(imageURL.GetGalleryImageURLResult);
                        // save seller cloud order images to prorduct_images table                  
                        ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                        try
                        {
                            if (img != null)
                            {
                                string fileName = Guid.NewGuid().ToString() + "-" + item + Path.GetExtension(imageURL.GetGalleryImageURLResult);
                                databaseImagesURL.product_Sku = item;
                                databaseImagesURL.FileName = fileName;
                                databaseImagesURL.ImageURL = imageURL.GetGalleryImageURLResult;
                                databaseImagesURL.isImageExistInSC = true;
                                if (sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL))
                                {
                                    await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);


                                    await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);


                                }

                                ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, item, true);
                                _successCounter++;
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, item, false);
                        _failureCounter++;
                    }
                }

            }


            catch (Exception ex) { }
            return Json(new { success = status, failureCounter = _failureCounter, successCount = _successCounter, message = "Images Updated Successfully . . ." });
        }

        public async Task<bool> UpdateDropShipStatus(int orderid, string DropShipStatusName)
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

                    var request = await sCServiceSoap.UpdateOrderDropShipStatusAsync(authHeader, null, orderid, DropShipStatusType2.Requested);
                    bool response = request.UpdateOrderDropShipStatusResult;
                    status = response;
                }
                if (DropShipStatusName == "None")
                {
                    ServiceReference1.SCServiceSoapClient sCServiceSoap =
                                           new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                    var request = await sCServiceSoap.UpdateOrderDropShipStatusAsync(authHeader, null, orderid, DropShipStatusType2.None);
                    bool response = request.UpdateOrderDropShipStatusResult;
                    status = response;
                }
            }


            catch (Exception ex) { }
            return status;
        }

        public async Task<IActionResult> UpdateImageFromSellerCloudBySKU(string sku)
        {
            bool status = false;
            bool imageNotFound = false;
            bool imageAlreadyExist = false;
            int _successCounter = 0;
            int _failureCounter = 0;
            try
            {
                token = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                authHeader = new AuthHeader();
                authHeader.ValidateDeviceID = false;
                authHeader.UserName = _Selller.UserName;
                authHeader.Password = _Selller.Key;
                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                       new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);

                var Images = sellerCloudApiAccess.GetSCImagesBySKU(SCRestURL, authenticate.access_token, sku.Trim());

                foreach (var Image in Images)
                {
                   // var imageURL = Images.FirstOrDefault().Url;
                    var imageURL = Image.Url;


                    //var imageURL = await sCServiceSoap.GetGalleryImageURLAsync(authHeader, null, sku.Trim());

                    if (imageURL != null && imageURL != "")
                    {
                        Image img = DownloadImageFromUrl(imageURL);
                        // save seller cloud order images to prorduct_images table                  
                        ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                        try
                        {
                            if (img != null)
                            {

                                string fileName = Guid.NewGuid().ToString() + "-" + sku.Trim() + Path.GetExtension(imageURL);
                                databaseImagesURL.product_Sku = sku.Trim();
                                databaseImagesURL.FileName = fileName;
                                databaseImagesURL.ImageURL = imageURL;
                                databaseImagesURL.isImageExistInSC = true;
                                if (sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL))
                                {
                                    await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);

                                    await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);

                                    status = true;
                                    _successCounter++;
                                }
                                else
                                {
                                    imageAlreadyExist = true;
                                }
                                //ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, sku.Trim(), true);
                                //_successCounter++;                            
                            }

                        }
                        catch (Exception ex)
                        {
                            status = false;
                        }
                    }

                    //if (imageURL.GetGalleryImageURLResult != null && imageURL.GetGalleryImageURLResult != "")
                    //{
                    //    Image img = DownloadImageFromUrl(imageURL.GetGalleryImageURLResult);
                    //    // save seller cloud order images to prorduct_images table                  
                    //    ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                    //    try
                    //    {
                    //        if (img != null)
                    //        {

                    //            string fileName = Guid.NewGuid().ToString() + "-" + sku.Trim() + Path.GetExtension(imageURL.GetGalleryImageURLResult);
                    //            databaseImagesURL.product_Sku = sku.Trim();
                    //            databaseImagesURL.FileName = fileName;
                    //            databaseImagesURL.ImageURL = imageURL.GetGalleryImageURLResult;
                    //            databaseImagesURL.isImageExistInSC = true;
                    //            if (sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL))
                    //            {
                    //                await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);

                    //                await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);

                    //                status = true;
                    //                _successCounter++;
                    //            }
                    //            else
                    //            {
                    //                imageAlreadyExist = true;
                    //            }
                    //            //ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, sku.Trim(), true);
                    //            //_successCounter++;                            
                    //        }

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        status = false;
                    //    }
                    //}
                    else
                    {
                        imageNotFound = true;
                    }

                }
            }

            catch (Exception ex) { }
            return Json(new { success = status, ImageNotFound = imageNotFound, ImageAlreadyExist = imageAlreadyExist, failureCounter = _failureCounter, successCount = _successCounter, message = "Images Updated Successfully . . ." });
        }

        public async Task<IActionResult> SaveProductImageFromSellerCloudURL()
        {
            bool status = false;
            int _successCounter = 0;
            int _failureCounter = 0;
            try
            {
                token = Request.Cookies["Token"];


                List<SKUAndSellerCloudImageURLWhichImagesNotExistsViewModel> skuList = sellerCloudApiAccess.GetSKUAndSellerCloudImageURLWhichImagesNotExists(ApiURL, token);
                List<ImagesSaveToDatabaseWithURLViewMOdel> listImagesUrl = new List<ImagesSaveToDatabaseWithURLViewMOdel>();
                foreach (var item in skuList)
                {

                    if (item.ImageURL != null && item.ImageURL != "")
                    {
                        Image img = DownloadImageFromUrl(item.ImageURL);
                        // save seller cloud order images to prorduct_images table                  
                        ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                        try
                        {
                            if (img != null)
                            {
                                string fileName = Guid.NewGuid().ToString() + "-" + item.sku + Path.GetExtension(item.ImageURL);
                                await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);

                                await uploadFiles.uploadCompressedToS3(GetStreamOfReducedImage(img), fileName);

                                databaseImagesURL.product_Sku = item.sku;
                                databaseImagesURL.FileName = fileName;
                                databaseImagesURL.ImageURL = item.ImageURL;
                                databaseImagesURL.isImageExistInSC = true;
                                status = sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, databaseImagesURL);
                                ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, item.sku, true);
                                _successCounter++;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        ProductapiAccess.UpdateSCImageStatusInProductTable(ApiURL, token, item.sku, false);
                        _failureCounter++;
                    }
                }

            }


            catch (Exception ex) { }
            return Json(new { success = status, failureCounter = _failureCounter, successCount = _successCounter, message = "Images Updated Successfully . . ." });
        }


        public Stream GetStream(Image img, ImageFormat format)
        {
            var ms = new MemoryStream();
            img.Save(ms, format);
            return ms;
        }

        public async Task<IActionResult> SellerCloudSetting()
        {
            BootstrapModelStatus model = new BootstrapModelStatus();
            model.ModelName = "Seller Cloud Orders";
            model.Status = false;
            model.StatusMessage = "Error Occured";
            try
            {
                token = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                authHeader = new AuthHeader();
                authHeader.ValidateDeviceID = false;
                authHeader.UserName = _Selller.UserName;
                authHeader.Password = _Selller.Key;
                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                       new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                //ServiceReference1.UpdateOrderDropShipStatusRequest request = new UpdateOrderDropShipStatusRequest(authHeader, null, 2345, DropShipStatusType1.Requested);

                TimeSpan time1 = TimeSpan.FromHours(1); // my attempt to add 2 hours
                TimeSpan ts = DateTime.Now.TimeOfDay;
                var newTs = ts.Add(time1);
                sCServiceSoap.InnerChannel.OperationTimeout = newTs;
                ServiceOptions serviceOptions = new ServiceOptions();

                serviceOptions.AlwaysRecalculateWeight = false;
                serviceOptions.BulkDeleteShadows = false;
                serviceOptions.BulkWipeRelationships = false;
                serviceOptions.DontIncludePORMAImages = false;
                serviceOptions.FetchUserDefinedColumnsForOrder = false;
                serviceOptions.IncludeClientUserDocuments = false;
                serviceOptions.IncludePODetails = false;
                serviceOptions.SaveOrderPackageDimensions = false;
                serviceOptions.SkipBundleItemQtyUpdating = false;
                serviceOptions.UseCache = true;
                serviceOptions.DontNeedCompanyProfile = false;
                serviceOptions.FetchUserDefinedColumnsForProducts = false;
                serviceOptions.IncludeShippingSuggestions = false;
                serviceOptions.SkipCWAShippingRules = false;
                serviceOptions.AllowAnyProductShippingMethods = false;

                List<string> keys = new List<string>();
                List<string> values = new List<string>();
                //DateTime fromDate = new DateTime(2019, 5,15);
                //DateTime toDate = new DateTime(2019, 5, 16);
                //keys.Add("DateFrom");
                //values.Add(fromDate.ToBinary().ToString());

                //keys.Add("DateTo");
                //values.Add(toDate.ToBinary().ToString());

                SerializableDictionaryOfStringString filters = new SerializableDictionaryOfStringString();
                filters.Keys = keys.ToArray();
                filters.Values = values.ToArray();

                //get all those order from seller cloud which are unshipped

                var orderIds = await sCServiceSoap.Orders_GetAsync(authHeader, null, filters);

                string OrderIdToCheckInDatabase = string.Join(',', orderIds.Orders_GetResult);

                //check sellre cloud orders which exists in database
                List<int> orderWhichAreUnshippedDataExist = sellerCloudApiAccess.GetSellerCloudData(ApiURL, token, OrderIdToCheckInDatabase);



                //get distinct orders and those which are not exist in database
                List<int> distinctOrders = orderIds.Orders_GetResult.Except(orderWhichAreUnshippedDataExist).ToList();

                //orderIds.Orders_GetResult.ToArray())    new int[] { 6167199 }
                List<SellerCloudOrder_CustomerViewModel> _mainOrderDetailCustomerList = new List<SellerCloudOrder_CustomerViewModel>();

                var ordersDetail = await sCServiceSoap.Orders_GetDatasAsync(authHeader, null, distinctOrders.ToArray());


                List<ImagesClass> imagesList = new List<ImagesClass>();

                #region commented code for download all images
                // commented code for download all images

                //List<int> sellerCloudOrderIdsToImportImages = sellerCloudApiAccess.GetSellerCloudOrderIdsForImageImport(ApiURL, token);
                // var ordersDetailofImages = await sCServiceSoap.Orders_GetDatasAsync(authHeader, null, sellerCloudOrderIdsToImportImages.ToArray());
                //foreach (var item in ordersDetailofImages.Orders_GetDatasResult)
                //{

                //    SerializableDictionaryOfStringString stringString = item.GalleryImagesURL;
                //    List<string> tempKeys = stringString.Keys.ToList();
                //    List<string> tempValues = stringString.Values.ToList();
                //    for (int i = 0; i < tempKeys.Count; i++)
                //    {
                //        ImagesClass images = new ImagesClass();
                //        images.Key = tempKeys[i].ToString();
                //        images.Value = tempValues[i].ToString();
                //        imagesList.Add(images);
                //    }
                //}

                #endregion

                //Prepare complete order and order detail object
                foreach (var item in ordersDetail.Orders_GetDatasResult)
                {
                    SellerCloudOrderViewModel sellerCloudOrder = new SellerCloudOrderViewModel();
                    SellerCloudCustomerDetail sellerCloudCustomer = new SellerCloudCustomerDetail();
                    SellerCloudOrder_CustomerViewModel order_orderDetail_customer = new SellerCloudOrder_CustomerViewModel();
                    List<SellerCloudOrderDetailViewModel> sellerCloudOrderDetailList = new List<SellerCloudOrderDetailViewModel>();


                    SerializableDictionaryOfStringString stringString = item.GalleryImagesURL;
                    List<string> tempKeys = stringString.Keys.ToList();
                    List<string> tempValues = stringString.Values.ToList();
                    //for (int i = 0; i < tempKeys.Count; i++)
                    //{
                    //    ImagesClass images = new ImagesClass();
                    //    images.Key = tempKeys[i].ToString();
                    //    images.Value = tempValues[i].ToString();
                    //    imagesList.Add(images);
                    //}

                    sellerCloudOrder.totalCount = item.Order.ItemCount;
                    sellerCloudOrder.dropShipStatus = System.Enum.GetName(typeof(DropShipStatusType2), item.Order.DropShipStatus);
                    sellerCloudOrder.currencyRateFromUSD = item.Order.CurrencyRateFromUSD;
                    sellerCloudOrder.lastUpdate = item.Order.LastUpdated;
                    sellerCloudOrder.timeOfOrder = item.Order.TimeOfOrder;
                    sellerCloudOrder.taxTotal = item.Order.TaxTotal;

                    //sellerCloudOrder.shippingStatus = System.Enum.GetName(typeof(OrderShippingStatus2), item.Order.ShippingStatus);  
                    sellerCloudOrder.shippingStatus = "5";
                    sellerCloudOrder.shippingWeightTotalOz = item.Order.ShippingWeightTotalOz;
                    sellerCloudOrder.orderCurrencyCode = System.Enum.GetName(typeof(CurrencyCodeType2), item.Order.OrderCurrencyCode);
                    sellerCloudOrder.orderSourceOrderId = item.Order.OrderSourceOrderId;
                    sellerCloudOrder.paymentDate = item.Order.PaymentDate;//Order Date in our case
                    sellerCloudOrder.sellerCloudID = item.Order.ID; //seller cloud order id
                    sellerCloudOrder.ClientID = item.Order.ClientId;


                    Address address = item.Order.BillingAddress;

                    sellerCloudCustomer.countryName = address.CountryName;
                    sellerCloudCustomer.firstName = address.FirstName;
                    sellerCloudCustomer.lastName = address.LastName;
                    sellerCloudCustomer.phoneNumber = address.PhoneNumber;
                    sellerCloudCustomer.postalCode = address.PostalCode;
                    sellerCloudCustomer.stateCode = address.StateCode;
                    sellerCloudCustomer.stateName = address.StateName;
                    sellerCloudCustomer.streetLine1 = address.StreetLine1;
                    sellerCloudCustomer.streetLine2 = address.StreetLine2;
                    sellerCloudCustomer.city = address.City;



                    foreach (var itemDetail in item.Order.Items)
                    {
                        SellerCloudOrderDetailViewModel sellerCloudOrderDetail = new SellerCloudOrderDetailViewModel();
                        sellerCloudOrderDetail.DropShippedOn = itemDetail.DropShippedOn;
                        sellerCloudOrderDetail.DropShippedStatus = System.Enum.GetName(typeof(DropShipStatusType), itemDetail.DropShippedStatus);
                        sellerCloudOrderDetail.OrderId = itemDetail.OrderID;
                        sellerCloudOrderDetail.MinQTY = itemDetail.MinimumQty;
                        sellerCloudOrderDetail.SKU = itemDetail.ProductID;
                        sellerCloudOrderDetail.StatusCode = System.Enum.GetName(typeof(OrderItemStatusCode), itemDetail.StatusCode);
                        sellerCloudOrderDetail.Qty = itemDetail.Qty;
                        sellerCloudOrderDetail.ProductTitle = itemDetail.DisplayName;
                        sellerCloudOrderDetail.AdjustedSitePrice = itemDetail.AdjustedSitePrice;
                        sellerCloudOrderDetail.AverageCost = itemDetail.AverageCost;
                        sellerCloudOrderDetail.PricePerCase = itemDetail.PricePerCase;
                        sellerCloudOrderDetail.unitPrice = itemDetail.UnitPrice;
                        sellerCloudOrderDetail.UPC = itemDetail.UPC;
                        sellerCloudOrderDetailList.Add(sellerCloudOrderDetail);
                    }

                    //assign object to main object
                    order_orderDetail_customer.Customer = sellerCloudCustomer;
                    order_orderDetail_customer.Order = sellerCloudOrder;
                    order_orderDetail_customer.orderDetail = sellerCloudOrderDetailList;

                    //main object list
                    _mainOrderDetailCustomerList.Add(order_orderDetail_customer);
                }

                // add data into seller cloud tables like order ,order detail ,customer detail 
                List<ImagesSaveToDatabaseWithURLViewMOdel> listImagesUrl = new List<ImagesSaveToDatabaseWithURLViewMOdel>();



                if (_mainOrderDetailCustomerList.Count > 0)
                {
                    sellerCloudApiAccess.AddSellerCloudOrder(ApiURL, token, _mainOrderDetailCustomerList);
                    // save seller cloud order images to prorduct_images table    
                    //commented code for save images in seller cloud
                    //foreach (var item in imagesList)
                    //{
                    //    ImagesSaveToDatabaseWithURLViewMOdel databaseImagesURL = new ImagesSaveToDatabaseWithURLViewMOdel();
                    //    try
                    //    {


                    //        Image img = DownloadImageFromUrl(item.Value);
                    //        if (img != null)
                    //        {
                    //            string fileName = Guid.NewGuid().ToString() + "-" + item.Key + Path.GetExtension(item.Value);
                    //            await uploadFiles.uploadToS3(GetStream(img, ImageFormat.Jpeg), fileName);                                
                    //            databaseImagesURL.product_Sku = item.Key;
                    //            databaseImagesURL.FileName = fileName;

                    //            listImagesUrl.Add(databaseImagesURL);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //    }
                    //}
                }

                //getting all those seller cloud order ids whcih are not present in Best Buy Table(To get orders from best Buy )

                List<string> OrderIds = bestBuyOrdersApiAccess.GetBestBuyOrderIdsToImportData(ApiURL, token);
                if (OrderIds.Count > 0)
                {

                    _bestbuy = new GetChannelCredViewModel();
                    _bestbuy = _encDecChannel.DecryptedData(ApiURL, token, "bestbuy");

                    BestBuyRootObject bestBuyRootObject = bestBuyOrdersApiAccess.GetBestBuyOrdersAgainstIDs("https://marketplace.bestbuy.ca/api/orders?paginate=false&order_ids=", _bestbuy.Key, OrderIds);

                    List<BestBuyOrdersImportMainViewModel> listBestBuyOrders = new List<BestBuyOrdersImportMainViewModel>();

                    if (bestBuyRootObject.orders.Count > 0)
                    {
                        foreach (var item in bestBuyRootObject.orders)
                        {
                            BestBuyOrderImportViewModel OrderViewModel = new BestBuyOrderImportViewModel();
                            List<BestBuyOrderDetailImportViewModel> ListorderDetailViewModel = new List<BestBuyOrderDetailImportViewModel>();
                            BestBuyCustomerDetailImportViewModel customerDetailOrderViewModel = new BestBuyCustomerDetailImportViewModel();
                            BestBuyOrdersImportMainViewModel mainModel = new BestBuyOrdersImportMainViewModel();

                            OrderViewModel.order_id = item.order_id;
                            OrderViewModel.commercial_id = item.commercial_id;
                            OrderViewModel.customer_id = item.customer.customer_id;
                            OrderViewModel.can_cancel = item.can_cancel;
                            OrderViewModel.order_state = item.order_state;
                            OrderViewModel.acceptance_decision_date = item.acceptance_decision_date;
                            OrderViewModel.created_date = item.created_date;
                            OrderViewModel.total_price = item.total_price;
                            OrderViewModel.total_commission = item.total_commission;
                            //
                            mainModel.OrderViewModel = OrderViewModel;

                            foreach (var orderDetail in item.order_lines)
                            {
                                BestBuyOrderDetailImportViewModel orderDetailViewModel = new BestBuyOrderDetailImportViewModel();
                                orderDetailViewModel.order_line_id = orderDetail.order_line_id;
                                orderDetailViewModel.offer_sku = orderDetail.offer_sku;
                                orderDetailViewModel.quantity = orderDetail.quantity;
                                orderDetailViewModel.total_priceOrerLine = orderDetail.total_price;
                                orderDetailViewModel.total_commissionOrderLine = orderDetail.total_commission;
                                orderDetailViewModel.order_line_state = orderDetail.order_line_state;
                                orderDetailViewModel.received_date = orderDetail.received_date;
                                orderDetailViewModel.shipped_date = orderDetail.shipped_date;
                                orderDetailViewModel.product_title = orderDetail.product_title;
                                orderDetailViewModel.GST = Convert.ToDouble(orderDetail.taxes.Sum(e => e.amount).ToString());
                                orderDetailViewModel.PST = 0;
                                ListorderDetailViewModel.Add(orderDetailViewModel);
                            }

                            mainModel.orderDetailViewModel = ListorderDetailViewModel;


                            customerDetailOrderViewModel.firstname = item.customer.shipping_address.firstname;
                            customerDetailOrderViewModel.lastname = item.customer.shipping_address.lastname;
                            customerDetailOrderViewModel.state = item.customer.shipping_address.state;
                            customerDetailOrderViewModel.street_1 = item.customer.shipping_address.street_1;
                            customerDetailOrderViewModel.street_2 = item.customer.shipping_address.street_2;
                            customerDetailOrderViewModel.zip_code = item.customer.shipping_address.zip_code;
                            customerDetailOrderViewModel.phone = item.customer.shipping_address.phone;
                            customerDetailOrderViewModel.phone_secondary = item.customer.shipping_address.phone_secondary;
                            customerDetailOrderViewModel.city = item.customer.shipping_address.city;
                            customerDetailOrderViewModel.country = item.customer.shipping_address.country;

                            mainModel.customerDetailOrderViewModel = customerDetailOrderViewModel;
                            listBestBuyOrders.Add(mainModel);
                        }
                    }

                    //insert best buy order and there detail

                    if (listBestBuyOrders.Count > 0)
                    {

                        bestBuyOrdersApiAccess.SaveBestBuyOrders(ApiURL, token, listBestBuyOrders);
                    }
                }


                //insert product detail from seller cloud to product table
                sellerCloudApiAccess.InsertProductDataToProductTable(ApiURL, token);
                model.Status = true;
                model.StatusMessage = "Update Successfully. . .";
                model.ModelName = "Seller Cloud Orders";
                //commented code for save seller cloud images
                //sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, listImagesUrl);


            }
            catch (Exception ex)
            {
                model.Status = false;
                model.StatusMessage = "Some Error Occured";
                model.ModelName = "Seller Cloud Orders";
                return PartialView("~/Views/BootstrapModel/ShowModel.cshtml", model);
            }


            return PartialView("~/Views/BootstrapModel/ShowModel.cshtml", model);
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<JsonResult> GetOrderStatus(string orderid)
        {
            try
            {
                token = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                authHeader = new AuthHeader();
                authHeader.ValidateDeviceID = false;
                authHeader.UserName = _Selller.UserName;
                authHeader.Password = _Selller.Key;
                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                          new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);
                var data = await sCServiceSoap.Orders_GetOrderStateAsync(authHeader, null, Convert.ToInt32(orderid));


                ServiceReference1.OrderStatusCode orderStatusCode = data.Orders_GetOrderStateResult.StatusCode;
                ServiceReference1.DropShipStatusType2 dropshipStatus = data.Orders_GetOrderStateResult.DropShipStatus;
                ServiceReference1.OrderPaymentStatus2 paymentStatus = data.Orders_GetOrderStateResult.PaymentStatus;


                int status = 0;
                if (orderStatusCode.ToString() == "Canceled")
                {
                    status = (int)SellerCloudOrderStatusCategory.Canceled;
                }
                else if (orderStatusCode.ToString() == "ShoppingCart")
                {
                    status = (int)SellerCloudOrderStatusCategory.ShoppingCart;
                }
                else if (orderStatusCode.ToString() == "InProcess")
                {
                    status = (int)SellerCloudOrderStatusCategory.InProcess;
                }
                else if (orderStatusCode.ToString() == "ProblemOrder")
                {
                    status = (int)SellerCloudOrderStatusCategory.ProblemOrder;
                }
                else if (orderStatusCode.ToString() == "OnHold")
                {
                    status = (int)SellerCloudOrderStatusCategory.OnHold;
                }
                else if (orderStatusCode.ToString() == "Quote")
                {
                    status = (int)SellerCloudOrderStatusCategory.Quote;
                }
                else if (orderStatusCode.ToString() == "Void")
                {
                    status = (int)SellerCloudOrderStatusCategory.Void;
                }
                else if (orderStatusCode.ToString() == "InProcess or Completed")
                {
                    status = (int)SellerCloudOrderStatusCategory.InProcess_or_Completed;
                }
                else if (orderStatusCode.ToString() == "InProcess or Hold")
                {
                    status = (int)SellerCloudOrderStatusCategory.InProcess_or_Hold;
                }
                else if (orderStatusCode.ToString() == "Completed")
                {
                    status = (int)SellerCloudOrderStatusCategory.Completed;
                }
                sellerCloudApiAccess.SaveSellerCloudOrderStatus(ApiURL, token, orderid, status.ToString(), paymentStatus.ToString());




                sellerCloudApiAccess.UpdateSellerCloudOrderDropShipStatus(ApiURL, token, new UpdateSCDropshipStatusViewModel()
                {
                    IsTrackingUpdate = false,
                    LogDate = DateTime.Now,
                    SCOrderID = Convert.ToInt32(orderid),
                    StatusName = dropshipStatus.ToString()
                });

                return Json(new { status = true, scOrderStatus = orderStatusCode.ToString(), DropshipStatus = dropshipStatus.ToString(), PaymentStats = paymentStatus.ToString() });
            }
            catch (Exception)
            {

            }
            return Json(new { status = false, scOrderStatus = "" });
        }

        public OrderStatusViewModel GetOrdersFromSCRest(string orderid,string SCtokin)
        {
            OrderStatusViewModel orderRelationViewModel = null;
            try
            {
                SCtokin = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, SCtokin, "sellercloud");
                AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lp.api.sellercloud.com/rest/api/Orders/" + orderid);
                request.Method = "GET";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + authenticate.access_token;
                string strResponse = "";
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        strResponse = stream.ReadToEnd();
                    }
                }
                var X = JObject.Parse(strResponse);
                if (X != null)
                {
                    var RelatedOrders = X["Statuses"];
                    var count = RelatedOrders.Count();
                    if (RelatedOrders != null && RelatedOrders.Count() > 0)
                    {
                        orderRelationViewModel = new OrderStatusViewModel();
                        
                            orderRelationViewModel.OrderStatus = X["Statuses"].Value<string>("OrderStatus").ToString();
                            orderRelationViewModel.PaymentStatus = X["Statuses"].Value<string>("PaymentStatus").ToString();
                            orderRelationViewModel.DropshipStatus = X["Statuses"].Value<string>("DropshipStatus").ToString();

                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            return orderRelationViewModel;
        }



        [HttpGet]
        public async Task<IActionResult> SendTrackingToSC(string sellerCloudOrderId, string zincOrderLogDetailID, string itemQuantity, string productSku)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            authHeader = new AuthHeader();
            authHeader.ValidateDeviceID = false;
            authHeader.UserName = _Selller.UserName;
            authHeader.Password = _Selller.Key;
            ZincOrderLogDetailViewModel model = new ZincOrderLogDetailViewModel();
            if (!string.IsNullOrEmpty(zincOrderLogDetailID) && !string.IsNullOrEmpty(sellerCloudOrderId))
            {
                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                          new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                var request = await sCServiceSoap.UpdateOrderDropShipStatusAsync(authHeader, null, int.Parse(sellerCloudOrderId), DropShipStatusType2.Processed);
                bool response = request.UpdateOrderDropShipStatusResult;
                status = response;
                if (status)
                {
                    UpdateSCDropshipStatusViewModel updateSCViewModel = new UpdateSCDropshipStatusViewModel();
                    updateSCViewModel.StatusName = "Processed";
                    updateSCViewModel.SCOrderID = int.Parse(sellerCloudOrderId);
                    updateSCViewModel.LogDate = DateTime.Now;
                    updateSCViewModel.IsTrackingUpdate = true;


                    status = sellerCloudApiAccess.UpdateSellerCloudOrderDropShipStatus(ApiURL, token, updateSCViewModel);



                    model = _zincOrderLogAndDetailApiAccess.GetZincOrderLogDetailById(ApiURL, token, zincOrderLogDetailID);
                    OrdersUpdateShippingForOrderRequest req = new OrdersUpdateShippingForOrderRequest();



                    req.WarehouseName = "DropShip Canada";
                    req.TrackingNumber = model.TrackingNumber;

                    string[] splitString = model.ShppingDate.Split('/');
                    string[] yearSplit = splitString[2].Split(' ');
                    string[] splitStringtime = model.ShppingDate.Split(':');
                    string[] splitTime = splitString[0].Split(' ');

                    DateTime d = new DateTime(int.Parse(yearSplit[0]), int.Parse(splitString[0]), int.Parse(splitString[1]), int.Parse(splitTime[0]), int.Parse(splitStringtime[1]), int.Parse(splitStringtime[2]), 0);

                    d = DatetimeExtension.ConvertToEST(d);
                    req.ShipDate = d;
                    req.OrderID = int.Parse(sellerCloudOrderId);

                    var result = await sCServiceSoap.Orders_UpdateShippingForOrderAsync(authHeader, null, req);

                    bool shippingOrderStatus = result.Orders_UpdateShippingForOrderResult;
                    var warehouseAdjustment = await sCServiceSoap.ProductWarehouse_AdjustQtyAsync(authHeader, null, productSku, 364, int.Parse(itemQuantity), "Dropship Adj " + sellerCloudOrderId, "");
                    status = warehouseAdjustment.ProductWarehouse_AdjustQtyResult;
                }
            }
            return Json(new { success = status });
        }

        public JsonResult GetSellerCloudOrderStatusLastestUpdate(string orderid)
        {
            token = Request.Cookies["Token"];
            string status = sellerCloudApiAccess.SellerCloudOrderLatestUpdateStatus(ApiURL, token, orderid);
            return Json(new { OrderLatestUpdate = status });
        }

        public void GetProductDataFromZinc(string retailer, string orderid = "B07D6LZZHG")
        {
            _Zinc = new GetChannelCredViewModel();
            _Zinc = _encDecChannel.DecryptedData(ApiURL, token, "Zinc");
            ZincUserName = _Zinc.Key;
            string uri = " https://api.zinc.io/v1/products/" + orderid + "?retailer=" + retailer + "";
            string response = "";
            var webRequest = WebRequest.Create(uri);
            webRequest.Credentials = new NetworkCredential(ZincUserName, "");
            using (var webResponse = webRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    response = new StreamReader(responseStream).ReadToEnd();
                }
            }

            ZincProductDetailDataViewModel.RootObject model = JsonConvert.DeserializeObject<ZincProductDetailDataViewModel.RootObject>(response);

            var timepstamp = model.timestamp;
            var status = model.status;



        }



        public Stream DownloadImageFromUrlASStream(string imageUrl)
        {

            System.IO.Stream stream;
            try
            {
                Uri ImageUri = new Uri(imageUrl, UriKind.Absolute);
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ImageUri);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                stream = webResponse.GetResponseStream();
                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            return stream;
        }

        [HttpGet]
        public async Task<IActionResult> ActionMenu(string actionMenuName, List<string> sellerCloudIds)
        {
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            bool status = false;
            if (actionMenuName.Trim() == "DS_Requested_To_None")
            {
                foreach (var item in sellerCloudIds)
                {
                    
                    var getOrderId = new UpdateScOrderToDs()
                    {
                        Orders = new List<int>
                        {
                           Convert.ToInt32(item)
                            //updateSCViewModel.SCOrderID

                        },
                        DropshipStatus = "DS_Requested_To_None"
                    };


                    status = UpdateDropShipStatus(SCRestURL, authenticate.access_token, getOrderId);
                   // await UpdateDropShipStatusForActionMenu(Convert.ToInt32(item));
                    status = sellerCloudApiAccess.UpdateSellerCloudOrderDropShipStatus(ApiURL, token, new UpdateSCDropshipStatusViewModel()
                    {
                        IsTrackingUpdate = false,
                        LogDate = DateTime.Now,
                        SCOrderID = Convert.ToInt32(item),
                        StatusName = "None"
                    });
                }
            }

            return Json(new { DSStatus = status, message = "DS status update successfully" });
        }

        public async Task<bool> UpdateDropShipStatusForActionMenu(int orderid)
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

                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                       new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                var request = await sCServiceSoap.UpdateOrderDropShipStatusAsync(authHeader, null, orderid, DropShipStatusType2.None);
                bool response = request.UpdateOrderDropShipStatusResult;
                status = response;
            }
            catch (Exception ex) { }
            return status;
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public Stream GetStreamOfReducedImage(Image inputPath)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                var settings = new MagickReadSettings { Format = MagickFormat.Raw };
                byte[] vs = ImageToByteArray(inputPath);
                int size = 100;
                int quality = 75;
                using (var image = new MagickImage(vs))
                {
                    image.Resize(size, size);
                    image.Strip();
                    image.Quality = quality;
                    image.Write(memoryStream, MagickFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                memoryStream = null;
            }
            finally
            {

            }
            return memoryStream;
        }


        public System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                Uri ImageUri = new Uri(imageUrl, UriKind.Absolute);
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ImageUri);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }


        public async Task<List<ProductWarehouseQtyViewModel>> GetProductWarehouseQuantity(string productSku)
        {

            List<ProductWarehouseQtyViewModel> list = new List<ProductWarehouseQtyViewModel>();
            try
            {
                token = Request.Cookies["Token"];
                _Selller = new GetChannelCredViewModel();
                _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
                authHeader = new AuthHeader();
                authHeader.ValidateDeviceID = false;
                authHeader.UserName = _Selller.UserName;
                authHeader.Password = _Selller.Key;

                ServiceReference1.SCServiceSoapClient sCServiceSoap =
                       new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);

                var request = await sCServiceSoap.GetProductInventoryForALLWarehousesAsync(authHeader, null, productSku);
                GetProductInventoryForALLWarehousesResponseType[] result = request.GetProductInventoryForALLWarehousesResult;
                foreach (var item in result)
                {
                    ProductWarehouseQtyViewModel model = new ProductWarehouseQtyViewModel();

                    if (item.WarehouseName == "DropShip Canada")
                    {
                        model.WarehouseID = (int)WarehouseName.DropShipCanada;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }

                    else if (item.WarehouseName == "DropShip USA")
                    {
                        model.WarehouseID = (int)WarehouseName.DropShipUSA;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "FBA Canada")
                    {
                        model.WarehouseID = (int)WarehouseName.FBACanada;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "FBA USA")
                    {
                        model.WarehouseID = (int)WarehouseName.FBAUSA;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "HLD-CA1")
                    {
                        model.WarehouseID = (int)WarehouseName.HLDCA1;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "HLD-CA2")
                    {
                        model.WarehouseID = (int)WarehouseName.HLDCA2;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "HLD-CN1")
                    {
                        model.WarehouseID = (int)WarehouseName.HLDCN1;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "HLD-Interim")
                    {
                        model.WarehouseID = (int)WarehouseName.HLDInterim;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "HLD-Tech1")
                    {
                        model.WarehouseID = (int)WarehouseName.HLDTech1;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "Interim FBA CA")
                    {
                        model.WarehouseID = (int)WarehouseName.InterimFBACA;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "Interim FBA USA")
                    {
                        model.WarehouseID = (int)WarehouseName.InterimFBAUSA;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "NY-14305")
                    {
                        model.WarehouseID = (int)WarehouseName.NY14305;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }
                    else if (item.WarehouseName == "Shipito")
                    {
                        model.WarehouseID = (int)WarehouseName.Shipito;
                        model.AvailableQty = item.QtyAvailable;
                        model.ProductSku = productSku;
                        model.WarehouseName = item.WarehouseName;
                        list.Add(model);
                    }

                }
            }
            catch (Exception ex) { }
            return list;
        }

        public async Task<ProductInsetUpdateViewModel> GetProductInfoFromSellerCloud(string productSku)
        {
            token = Request.Cookies["Token"];
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            authHeader = new AuthHeader();
            authHeader.ValidateDeviceID = false;
            authHeader.UserName = _Selller.UserName;
            authHeader.Password = _Selller.Key;

            ServiceReference1.SCServiceSoapClient sCServiceSoap =
                      new ServiceReference1.SCServiceSoapClient(ServiceReference1.SCServiceSoapClient.EndpointConfiguration.SCServiceSoap12);
            var request = await sCServiceSoap.GetProductInfoAsync(authHeader, null, productSku);
            ProductInfo productInfo = request.GetProductInfoResult;
            ProductInsetUpdateViewModel model = new ProductInsetUpdateViewModel();
            model.ProductTitle = productInfo.ProductName;
            model.Upc = productInfo.UPC;
            model.ProductSKU = productInfo.ID;
            return model;
        }

        public string GetProductTitle(string sellercloudid, string productsku)
        {
            token = Request.Cookies["Token"];
            GetProductTitleViewModel saveWatchlistViewModel = new GetProductTitleViewModel();
            saveWatchlistViewModel.sellercloudid = sellercloudid;
            saveWatchlistViewModel.producktsku = productsku;
            string status = sellerCloudApiAccess.GetProductTitle(ApiURL, token, saveWatchlistViewModel);
            return status;
        }
        public bool UpdateDropShipStatus(string ApiURL, string token, UpdateScOrderToDs scOrderToDs)
        {
            bool status;
            try
            {


                var data = JsonConvert.SerializeObject(scOrderToDs);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/Orders/DropshipStatus");
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
                status = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return status;
        }

    }
}




