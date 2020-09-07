using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceReference1;

namespace Hld.WebApplication.Controllers
{
    public class ImportSCOrderController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";

        ImportMissingOrderApiAccess ImportMissingOrderApiAccess = null;
        BestBuyOrdersApiAccess bestBuyOrdersApiAccess = null;
        SellerCloudApiAccess sellerCloudApiAccess = null;
        OrderRelationAPIAccess orderRelationAPIAccess = null;
        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        GetChannelCredViewModel _bestbuy = null;
        ServiceReference1.AuthHeader authHeader = null;

        string s3BucketURL = "";

        public static IHostingEnvironment _environment;
        public ImportSCOrderController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ImportMissingOrderApiAccess = new ImportMissingOrderApiAccess();
            bestBuyOrdersApiAccess = new BestBuyOrdersApiAccess();
            sellerCloudApiAccess = new SellerCloudApiAccess();
            orderRelationAPIAccess = new OrderRelationAPIAccess();
            _encDecChannel = new EncDecChannel();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ImportMissing()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ImportChildOrder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ImportChildOrder(MissingOrderViewModel missingOrderViewModel)
        {
            token = Request.Cookies["Token"];

            List<CheckMissingOrderViewModel> OrderList = new List<CheckMissingOrderViewModel>();
            List<string> missingSkuList = new List<string>();
            string newlist = missingOrderViewModel.Order;
            var lines = newlist.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Trim() != string.Empty)
                {
                    OrderList.Add(new CheckMissingOrderViewModel
                    {
                        OrderID = Convert.ToInt32(line.Trim())
                    });


                }
            }
            MissingOrderReturnViewModel detail = ImportMissingOrderApiAccess.CheckOrderINDB(ApiURL, token, OrderList);
            var sessionMissingSku = JsonConvert.SerializeObject(detail.MissingOrder);
            HttpContext.Session.SetString("MissingChildOrders", sessionMissingSku);
            ViewBag.MissingChildOrderCount = detail.MissingOrderCount;
            ViewBag.ExistingChildOrderCount = detail.ExistingOrderCount;
            ViewBag.ExistingChildOrder = detail.ExistingOrder;
            ViewBag.MissingChildOrder = detail.MissingOrder;

            ViewData["existingChildOrder"] = detail.ExistingOrder;

            return View(missingOrderViewModel);
        }
        [HttpPost]
        public IActionResult ImportMissing(MissingOrderViewModel missingOrderViewModel)
        {
            token = Request.Cookies["Token"];

            List<CheckMissingOrderViewModel> OrderList = new List<CheckMissingOrderViewModel>();
            List<string> missingSkuList = new List<string>();
            string newlist = missingOrderViewModel.Order;
            var lines = newlist.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Trim() != string.Empty)
                {
                    OrderList.Add(new CheckMissingOrderViewModel
                    {
                        OrderID = Convert.ToInt32(line.Trim())
                    });


                }
            }
            MissingOrderReturnViewModel detail = ImportMissingOrderApiAccess.CheckOrderINDB(ApiURL, token, OrderList);
            var sessionMissingSku = JsonConvert.SerializeObject(detail.MissingOrder);
            HttpContext.Session.SetString("MissingOrders", sessionMissingSku);
            ViewBag.MissingOrderCount = detail.MissingOrderCount;
            ViewBag.ExistingOrderCount = detail.ExistingOrderCount;
            ViewBag.ExistingOrder = detail.ExistingOrder;
            ViewBag.MissingOrder = detail.MissingOrder;

            ViewData["existingOrder"] = detail.ExistingOrder;

            return View(missingOrderViewModel);
        }

        public async Task<IActionResult> ImportMissingOrdersFromSessionAsync()
        {
            var key = HttpContext.Session.GetString("MissingOrders");
            List<int> addedList = new List<int>();
            List<int> ErrorList = new List<int>();


            List<int> missingOrderList = JsonConvert.DeserializeObject<List<int>>(key);


            foreach (var item in missingOrderList)
            {
                List<int> orderid = new List<int>();
                orderid.Add(item);
                bool status = await GetMIssingOrdersFromSellerCloud(orderid);
                if (status == true)
                {
                    addedList.Add(item);
                }
                else
                {
                    ErrorList.Add(item);
                }
            }


            return Json(new { AddedOrder = addedList, ErrorOrder = ErrorList, });

        }

        public IActionResult ImportChildOrdersFromSessionAsync()
        {
            token = Request.Cookies["Token"];
            var key = HttpContext.Session.GetString("MissingChildOrders");

            List<int> missingOrderList = JsonConvert.DeserializeObject<List<int>>(key);

            List<OrderRelationToSaveViewModel> relationlist = new List<OrderRelationToSaveViewModel>();

            foreach (var item in missingOrderList)
            {
                OrderRelationToSaveViewModel relationViewModel = new OrderRelationToSaveViewModel();
                relationViewModel.SC_ChildID = item;
                relationlist.Add(relationViewModel);
            }
            JobIdReturnViewModel status = new JobIdReturnViewModel();

            status = orderRelationAPIAccess.SaveOrderRelationForJobs(ApiURL, token, relationlist);

            return Json(new { data = status });
        }


        public async Task<bool> GetMIssingOrdersFromSellerCloud(List<int> missingOrderList)
        {
            List<string> bbOrderIds = new List<string>();
            bool status = false;
            int scorderID = 0;
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

                SerializableDictionaryOfStringString filters = new SerializableDictionaryOfStringString();
                filters.Keys = keys.ToArray();
                filters.Values = values.ToArray();

                List<SellerCloudOrder_CustomerViewModel> _mainOrderDetailCustomerList = new List<SellerCloudOrder_CustomerViewModel>();

                var ordersDetail = await sCServiceSoap.Orders_GetDatasAsync(authHeader, null, missingOrderList.ToArray());


                List<ImagesClass> imagesList = new List<ImagesClass>();

                //Prepare complete order and order detail object
                foreach (var item in ordersDetail.Orders_GetDatasResult)
                {
                    SellerCloudOrderViewModel sellerCloudOrder = new SellerCloudOrderViewModel();
                    SellerCloudCustomerDetail sellerCloudCustomer = new SellerCloudCustomerDetail();
                    SellerCloudOrder_CustomerViewModel order_orderDetail_customer = new SellerCloudOrder_CustomerViewModel();
                    List<SellerCloudOrderDetailViewModel> sellerCloudOrderDetailList = new List<SellerCloudOrderDetailViewModel>();


                    SerializableDictionaryOfStringString stringString = item.GalleryImagesURL;
                    //List<string> tempKeys = stringString.Keys.ToList();
                    //List<string> tempValues = stringString.Values.ToList();

                    sellerCloudOrder.totalCount = item.Order.ItemCount;
                    sellerCloudOrder.dropShipStatus = System.Enum.GetName(typeof(DropShipStatusType2), item.Order.DropShipStatus);
                    sellerCloudOrder.currencyRateFromUSD = item.Order.CurrencyRateFromUSD;
                    sellerCloudOrder.lastUpdate = item.Order.LastUpdated;
                    sellerCloudOrder.timeOfOrder = item.Order.TimeOfOrder;
                    sellerCloudOrder.taxTotal = item.Order.TaxTotal;

                    //sellerCloudOrder.shippingStatus = System.Enum.GetName(typeof(OrderShippingStatus2), item.Order.ShippingStatus);  
                    sellerCloudOrder.shippingStatus = System.Enum.GetName(typeof(OrderShippingStatus2), item.Order.ShippingStatus);

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
                        string statuscode = "5";
                        switch (System.Enum.GetName(typeof(OrderItemStatusCode), itemDetail.StatusCode))
                        {
                            case "InProcess":
                                statuscode = "5";
                                break;
                            case "Canceled":
                                statuscode = "11";
                                break;
                            case "Completed":
                                statuscode = "6";
                                break;
                            case "ShoppingCart":
                                statuscode = "4";
                                break;
                            case "ProblemOrder":
                                statuscode = "7";
                                break;
                            case "OnHold":
                                statuscode = "8";
                                break;
                            case "Quote":
                                statuscode = "9";
                                break;
                            case "Void":
                                statuscode = "10";
                                break;
                            case "InProcess or Completed":
                                statuscode = "1";
                                break;
                            case "InProcess or Hold":
                                statuscode = "2";
                                break;


                        }

                        sellerCloudOrderDetail.StatusCode = statuscode;
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
                    bbOrderIds.Add(sellerCloudOrder.orderSourceOrderId);
                    scorderID = sellerCloudOrder.sellerCloudID;
                }

                // add data into seller cloud tables like order ,order detail ,customer detail 
                List<ImagesSaveToDatabaseWithURLViewMOdel> listImagesUrl = new List<ImagesSaveToDatabaseWithURLViewMOdel>();

                if (_mainOrderDetailCustomerList.Count > 0)
                {
                    sellerCloudApiAccess.AddSellerCloudOrder(ApiURL, token, _mainOrderDetailCustomerList);

                }

                //getting all those seller cloud order ids whcih are not present in Best Buy Table(To get orders from best Buy )

                //  List<string> OrderIds = bestBuyOrdersApiAccess.GetBestBuyOrderIdsToImportData(ApiURL, token);
                if (bbOrderIds.Count > 0)
                {

                    _bestbuy = new GetChannelCredViewModel();
                    _bestbuy = _encDecChannel.DecryptedData(ApiURL, token, "bestbuy");

                    BestBuyRootObject bestBuyRootObject = bestBuyOrdersApiAccess.GetBestBuyOrdersAgainstIDs("https://marketplace.bestbuy.ca/api/orders?paginate=false&order_ids=", _bestbuy.Key, bbOrderIds);

                    List<BestBuyOrdersImportMainViewModel> listBestBuyOrders = new List<BestBuyOrdersImportMainViewModel>();

                    if (bestBuyRootObject.orders.Count > 0)
                    {
                        foreach (var item in bestBuyRootObject.orders)
                        {
                            BestBuyOrderImportViewModel OrderViewModel = new BestBuyOrderImportViewModel();
                            List<BestBuyOrderDetailImportViewModel> ListorderDetailViewModel = new List<BestBuyOrderDetailImportViewModel>();
                            BestBuyCustomerDetailImportViewModel customerDetailOrderViewModel = new BestBuyCustomerDetailImportViewModel();
                            BestBuyOrdersImportMainViewModel mainModel = new BestBuyOrdersImportMainViewModel();
                            if (item.customer.shipping_address != null && item.customer.billing_address != null)
                            {
                                OrderViewModel.order_id = item.order_id;
                                OrderViewModel.commercial_id = item.commercial_id;
                                OrderViewModel.customer_id = item.customer.customer_id;
                                OrderViewModel.can_cancel = item.can_cancel;
                                OrderViewModel.order_state = item.order_state;
                                OrderViewModel.acceptance_decision_date = item.acceptance_decision_date;
                                OrderViewModel.created_date = item.created_date;
                                OrderViewModel.total_price = item.total_price;
                                OrderViewModel.total_commission = item.total_commission;
                                OrderViewModel.sellerCloudID = scorderID;
                                OrderViewModel.shipping_price = item.shipping_price;

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
                    }

                    //insert best buy order and there detail

                    if (listBestBuyOrders.Count > 0)
                    {
                        // Get single order against order id to send email
                        IEnumerable<BestBuyOrderDetailImportViewModel> result = listBestBuyOrders.SelectMany(e => e.orderDetailViewModel);


                        // sum quantity for specifu sku to update quantity on bb
                        var finalResult = result.GroupBy(e => e.offer_sku, (x, y) => new
                        {
                            totalQty = y.Sum(r => int.Parse(r.quantity)),
                            offersku = x,
                            date = y.Max(e => e.received_date)
                        }).ToList();

                        foreach (var item in finalResult)
                        {
                            BestBuyDropShipQtyMovementViewModel model = new BestBuyDropShipQtyMovementViewModel();
                            model.ProductSku = item.offersku;
                            model.OrderQuantity = item.totalQty.ToString();
                            model.OrderDate = DateTime.Now;
                            bestBuyOrdersApiAccess.SaveBestBuyOrderDropShipMovement(ApiURL, token, model);
                        }
                        // save Order in bestBuy table order and orderslines

                        bestBuyOrdersApiAccess.SaveBestBuyOrders(ApiURL, token, listBestBuyOrders);
                        status = true;

                    }
                }


                //insert product detail from seller cloud to product table
                sellerCloudApiAccess.InsertProductDataToProductTable(ApiURL, token);
                //model.Status = true;
                //model.StatusMessage = "Update Successfully. . .";
                //model.ModelName = "Seller Cloud Orders";
                ////commented code for save seller cloud images
                ////sellerCloudApiAccess.SaveSellerCloudImages(ApiURL, token, listImagesUrl);


            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }


        public async Task<bool> GetChildOrdersFromSellerCloud(List<int> missingOrderList, string ParentSC, string ParentBB)
        {
            List<string> bbOrderIds = new List<string>();
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

                SerializableDictionaryOfStringString filters = new SerializableDictionaryOfStringString();
                filters.Keys = keys.ToArray();
                filters.Values = values.ToArray();

                List<SellerCloudOrder_CustomerViewModel> _mainOrderDetailCustomerList = new List<SellerCloudOrder_CustomerViewModel>();

                var ordersDetail = await sCServiceSoap.Orders_GetDatasAsync(authHeader, null, missingOrderList.ToArray());


                List<ImagesClass> imagesList = new List<ImagesClass>();
                List<BestBuyOrdersImportMainViewModel> listBestBuyOrders = new List<BestBuyOrdersImportMainViewModel>();
                //Prepare complete order and order detail object
                foreach (var item in ordersDetail.Orders_GetDatasResult)
                {
                    SellerCloudOrderViewModel sellerCloudOrder = new SellerCloudOrderViewModel();
                    SellerCloudCustomerDetail sellerCloudCustomer = new SellerCloudCustomerDetail();
                    SellerCloudOrder_CustomerViewModel order_orderDetail_customer = new SellerCloudOrder_CustomerViewModel();
                    List<SellerCloudOrderDetailViewModel> sellerCloudOrderDetailList = new List<SellerCloudOrderDetailViewModel>();


                    // bestbuy Order
                    BestBuyOrderImportViewModel OrderViewModel = new BestBuyOrderImportViewModel();
                    List<BestBuyOrderDetailImportViewModel> ListorderDetailViewModel = new List<BestBuyOrderDetailImportViewModel>();
                    BestBuyCustomerDetailImportViewModel customerDetailOrderViewModel = new BestBuyCustomerDetailImportViewModel();
                    BestBuyOrdersImportMainViewModel mainModel = new BestBuyOrdersImportMainViewModel();


                    OrderViewModel.order_id = item.Order.OrderSourceOrderId;
                    OrderViewModel.commercial_id = item.Order.OrderSourceOrderId.Replace("-A", "");
                    OrderViewModel.customer_id = "";
                    OrderViewModel.can_cancel = false;
                    OrderViewModel.order_state = "Shipping";
                    OrderViewModel.acceptance_decision_date = item.Order.LastUpdated;
                    OrderViewModel.created_date = item.Order.LastUpdated;
                    OrderViewModel.total_price = Convert.ToDouble(item.Order.SubTotal);// verify
                    OrderViewModel.total_commission = 0;// verify
                    OrderViewModel.sellerCloudID = item.Order.ID; //seller cloud order id

                    mainModel.OrderViewModel = OrderViewModel;

                    SerializableDictionaryOfStringString stringString = item.GalleryImagesURL;
                    //List<string> tempKeys = stringString.Keys.ToList();
                    //List<string> tempValues = stringString.Values.ToList();

                    sellerCloudOrder.totalCount = item.Order.ItemCount;
                    sellerCloudOrder.dropShipStatus = System.Enum.GetName(typeof(DropShipStatusType2), item.Order.DropShipStatus);
                    sellerCloudOrder.currencyRateFromUSD = item.Order.CurrencyRateFromUSD;
                    sellerCloudOrder.lastUpdate = item.Order.LastUpdated;
                    sellerCloudOrder.timeOfOrder = item.Order.TimeOfOrder;
                    sellerCloudOrder.taxTotal = item.Order.TaxTotal;

                    //sellerCloudOrder.shippingStatus = System.Enum.GetName(typeof(OrderShippingStatus2), item.Order.ShippingStatus);  
                    sellerCloudOrder.shippingStatus = System.Enum.GetName(typeof(OrderShippingStatus2), item.Order.ShippingStatus);

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
                    // add best by 
                    customerDetailOrderViewModel.firstname = address.FirstName;
                    customerDetailOrderViewModel.lastname = address.LastName;
                    customerDetailOrderViewModel.state = address.StateName;
                    customerDetailOrderViewModel.street_1 = address.StreetLine1;
                    customerDetailOrderViewModel.street_2 = address.StreetLine2;
                    customerDetailOrderViewModel.zip_code = address.PostalCode;
                    customerDetailOrderViewModel.phone = address.PhoneNumber;
                    customerDetailOrderViewModel.phone_secondary = "";
                    customerDetailOrderViewModel.city = address.City;
                    customerDetailOrderViewModel.country = address.CountryName;

                    mainModel.customerDetailOrderViewModel = customerDetailOrderViewModel;

                    foreach (var itemDetail in item.Order.Items)
                    {
                        SellerCloudOrderDetailViewModel sellerCloudOrderDetail = new SellerCloudOrderDetailViewModel();
                        sellerCloudOrderDetail.DropShippedOn = itemDetail.DropShippedOn;
                        sellerCloudOrderDetail.DropShippedStatus = System.Enum.GetName(typeof(DropShipStatusType), itemDetail.DropShippedStatus);
                        sellerCloudOrderDetail.OrderId = itemDetail.OrderID;
                        sellerCloudOrderDetail.MinQTY = itemDetail.MinimumQty;
                        sellerCloudOrderDetail.SKU = itemDetail.ProductID;
                        string statuscode = "5";
                        switch (System.Enum.GetName(typeof(OrderItemStatusCode), itemDetail.StatusCode))
                        {
                            case "InProcess":
                                statuscode = "5";
                                break;
                            case "Canceled":
                                statuscode = "11";
                                break;
                            case "Completed":
                                statuscode = "6";
                                break;
                            case "ShoppingCart":
                                statuscode = "4";
                                break;
                            case "ProblemOrder":
                                statuscode = "7";
                                break;
                            case "OnHold":
                                statuscode = "8";
                                break;
                            case "Quote":
                                statuscode = "9";
                                break;
                            case "Void":
                                statuscode = "10";
                                break;
                            case "InProcess or Completed":
                                statuscode = "1";
                                break;
                            case "InProcess or Hold":
                                statuscode = "2";
                                break;


                        }

                        sellerCloudOrderDetail.StatusCode = statuscode;
                        sellerCloudOrderDetail.Qty = itemDetail.Qty;
                        sellerCloudOrderDetail.ProductTitle = itemDetail.DisplayName;
                        sellerCloudOrderDetail.AdjustedSitePrice = itemDetail.AdjustedSitePrice;
                        sellerCloudOrderDetail.AverageCost = itemDetail.AverageCost;
                        sellerCloudOrderDetail.PricePerCase = itemDetail.PricePerCase;
                        sellerCloudOrderDetail.unitPrice = itemDetail.UnitPrice;
                        sellerCloudOrderDetail.UPC = itemDetail.UPC;

                        sellerCloudOrderDetailList.Add(sellerCloudOrderDetail);

                        // best buy
                        BestBuyOrderDetailImportViewModel orderDetailViewModel = new BestBuyOrderDetailImportViewModel();
                        orderDetailViewModel.order_line_id = itemDetail.eBayTransactionId;//eBayTransactionId
                        orderDetailViewModel.offer_sku = itemDetail.ProductID;
                        orderDetailViewModel.quantity = itemDetail.Qty.ToString();
                        orderDetailViewModel.total_priceOrerLine = Convert.ToDouble(itemDetail.PricePerCase);
                        orderDetailViewModel.total_commissionOrderLine = 0;
                        orderDetailViewModel.order_line_state = "";
                        orderDetailViewModel.received_date = item.Order.LastUpdated;
                        orderDetailViewModel.shipped_date = item.Order.LastUpdated;
                        orderDetailViewModel.product_title = itemDetail.DisplayName;
                        orderDetailViewModel.GST = 0;
                        orderDetailViewModel.PST = 0;
                        ListorderDetailViewModel.Add(orderDetailViewModel);
                    }
                    mainModel.orderDetailViewModel = ListorderDetailViewModel;

                    listBestBuyOrders.Add(mainModel);

                    //assign object to main object
                    order_orderDetail_customer.Customer = sellerCloudCustomer;
                    order_orderDetail_customer.Order = sellerCloudOrder;
                    order_orderDetail_customer.orderDetail = sellerCloudOrderDetailList;

                    //main object list
                    _mainOrderDetailCustomerList.Add(order_orderDetail_customer);
                    bbOrderIds.Add(sellerCloudOrder.orderSourceOrderId);

                }

                // add data into seller cloud tables like order ,order detail ,customer detail 
                List<ImagesSaveToDatabaseWithURLViewMOdel> listImagesUrl = new List<ImagesSaveToDatabaseWithURLViewMOdel>();

                if (_mainOrderDetailCustomerList.Count > 0)
                {
                    sellerCloudApiAccess.AddSellerCloudOrder(ApiURL, token, _mainOrderDetailCustomerList);

                }


                if (listBestBuyOrders.Count > 0)
                {
                    // Get single order against order id to send email
                    IEnumerable<BestBuyOrderDetailImportViewModel> result = listBestBuyOrders.SelectMany(e => e.orderDetailViewModel);


                    // sum quantity for specifu sku to update quantity on bb
                    var finalResult = result.GroupBy(e => e.offer_sku, (x, y) => new
                    {
                        totalQty = y.Sum(r => int.Parse(r.quantity)),
                        offersku = x,
                        date = y.Max(e => e.received_date)
                    }).ToList();

                    foreach (var item in finalResult)
                    {
                        BestBuyDropShipQtyMovementViewModel model = new BestBuyDropShipQtyMovementViewModel();
                        model.ProductSku = item.offersku;
                        model.OrderQuantity = item.totalQty.ToString();
                        model.OrderDate = DateTime.Now;
                        bestBuyOrdersApiAccess.SaveBestBuyOrderDropShipMovement(ApiURL, token, model);
                    }
                    // save Order in bestBuy table order and orderslines

                    bestBuyOrdersApiAccess.SaveBestBuyOrders(ApiURL, token, listBestBuyOrders);
                    OrderRelationViewModel relationViewModel = new OrderRelationViewModel();
                    relationViewModel.SC_ChildID = missingOrderList[0];
                    relationViewModel.SC_ParentID = Convert.ToInt32(ParentSC);
                    relationViewModel.BB_OrderID = ParentBB;


                    orderRelationAPIAccess.SaveOrderRelation(ApiURL, token, relationViewModel);

                    status = true;

                }


                sellerCloudApiAccess.InsertProductDataToProductTable(ApiURL, token);



            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }
    }
}