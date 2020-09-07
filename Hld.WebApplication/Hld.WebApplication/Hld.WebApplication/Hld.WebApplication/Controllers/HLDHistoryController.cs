using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Hld.WebApplication.Enum;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    [Authorize(Policy = "Access to dashboard")]
    public class HLDHistoryController : Controller
    {
        private readonly IConfiguration _configuration = null;
        string ApiURL = "";
        string token = "";
        string connectionString = "";
        HLDSalesApiAccess _hldApiAccess = null;
        ProductApiAccess _productApiAccess = null;
        BBProductApiAccess _bBProductApiAccess = null;
        public HLDHistoryController(IConfiguration configuration)
        {

            this._configuration = configuration;
            connectionString = _configuration.GetValue<string>("ConnectionString:bb2HldNet");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _hldApiAccess = new HLDSalesApiAccess();
            _productApiAccess = new ProductApiAccess();
            _bBProductApiAccess = new BBProductApiAccess();
        }

        public IActionResult GetSalesHistoryOfSKU(string ProductSku)
        {
            return View("~/Views/HLDHistory/_GetSalesHistoryOfSKU.cshtml");
        }


        public IActionResult DashBoard()
        {
            token = Request.Cookies["Token"];
            List<Order_SKU_ProfitHistory_CalculationViewmodel> listModel = _hldApiAccess.GetSlaesHistoryForDashBoardByDate(ApiURL, token);
            return View(listModel);
        }

        public IActionResult CustomDateRange(string dateFrom, string dateTo)
        {
            token = Request.Cookies["Token"];
            SalesHistoryDashboardSearchViewModel model = new SalesHistoryDashboardSearchViewModel();
            model.DateFrom = Convert.ToDateTime(dateFrom);
            model.DateTo = Convert.ToDateTime(dateTo);

            Order_SKU_ProfitHistory_CalculationViewmodel modelData = _hldApiAccess.GetSlaesHistoryForDashBoardCustomRange(ApiURL, token, model);
            modelData.GrossRevnue_str = string.Format("{0:n}", modelData.GrossRevnue).ToString();
            modelData.ItemCost_str = string.Format("{0:n}", modelData.ItemCost).ToString();
            modelData.Profit_str = string.Format("{0:n0}", modelData.Profit).ToString();
            modelData.ProfitPercentage_str = string.Format("{0:n0}%", modelData.ProfitPercentage).ToString();
            modelData.TaxesPercentage_str = string.Format("{0:n}%", modelData.TaxesPercentage).ToString();
            modelData.UnitsSold_str = string.Format("{0:n0}", modelData.UnitsSold).ToString();
            modelData.OrderQuantity_str = string.Format("{0:n0}", modelData.OrderQuantity).ToString();
            modelData.SellingFeePercentage_str = string.Format("{0:n}%", modelData.SellingFeePercentage).ToString();
            return Json(modelData);
        }

        public IActionResult GetSalesHistoryOfSKUJsonData(string ProductSku)
        {
            token = Request.Cookies["Token"];


            var querystring = Request.QueryString;
            string dynamicQueryGeneration = "";
            List<SKUSalesHistoryFromOrders> listModel = null;
            NameValueCollection query = HttpUtility.ParseQueryString(Request.QueryString.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            if (filtersCount > 0)
            {
                dynamicQueryGeneration = BuildQuery(querystring, ProductSku);
                listModel = GetSKUHistoryDataDynamicQuery(dynamicQueryGeneration);
            }
            else
            {
                listModel = _hldApiAccess.GetSKU_OrderHistoryBy_SKU(ApiURL, token, ProductSku);
            }

            return Json(listModel);
        }

        public IActionResult PropertyPage(String ProductSKU)
        {
            token = Request.Cookies["Token"];
            int productID = 0;

            productID = _productApiAccess.GetProductIDBySKU(ApiURL, token, ProductSKU);

            if (productID == 0)
            {
                Response.StatusCode = 404;
                return View("~/Views/Product/404NotFound.cshtml", ProductSKU);
            }


            token = Request.Cookies["Token"];
            BBProductViewModel model = _bBProductApiAccess.GetBestBuyProductDetail_ProductID(ApiURL, token, ProductSKU);
            if (model.ShopSKU_OfferSKU != null)
            {
                model.DiscountEndDate = model.DiscountEndDate.Value != DateTime.MinValue ? model.DiscountEndDate.Value : DateTime.Now;
                model.DiscountStartDate = model.DiscountStartDate.Value != DateTime.MinValue ? model.DiscountStartDate.Value : DateTime.Now;

            }

            return View(model);
        }

        public IActionResult GetSKUSalesHistory(string ProductSku)
        {
            token = Request.Cookies["Token"];
            List<Order_SKU_ProfitHistory_CalculationViewmodel> listModel = _hldApiAccess.GetSkuProfitHistory(ApiURL, token, ProductSku);
            return PartialView("~/Views/HLDHistory/_GetSKUSalesHistory.cshtml", listModel);
        }


        private string BuildQuery(QueryString queryStringField, string sku)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(queryStringField.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            var queryString = @"  SET @row_number = 0; 
                                set @dollarRate =( select usd_to_cad from CurrencyExchange where `status`=true  order by currency_date desc limit 1);

select (@row_number:=@row_number+1) TotalRecords,abc.* from (
select  orders.sellerCloudID,inSellerCloud,orderLines.offer_sku,orderLines.quantity,
product.avg_cost as ProductAvgCost,
orderLines.total_price,orderLines.total_commission,orders.order_id,
TaxGST,TaxPST,Round( product.avg_cost * @dollarRate ,2) as avg_cost 
 from orders inner join orderLines on orders.order_id=orderLines.order_id
 inner join product on product.sku=orderLines.offer_sku
 where orderLines.offer_sku='" + sku + "' order by inSellerCloud desc ) abc ";
            var tmpDataField = "";
            var tmpFilterOperator = "";
            var where = "";
            if (filtersCount > 0)
            {
                where = " WHERE (";
            }
            for (var i = 0; i < filtersCount; i += 1)
            {
                var filterValue = query.GetValues("filtervalue" + i)[0];
                var filterCondition = query.GetValues("filtercondition" + i)[0];
                var filterDataField = query.GetValues("filterdatafield" + i)[0];
                var filterOperator = query.GetValues("filteroperator" + i)[0];
                if (tmpDataField == "")
                {
                    tmpDataField = filterDataField;
                }
                else if (tmpDataField != filterDataField)
                {
                    where += ") AND (";
                }
                else if (tmpDataField == filterDataField)
                {
                    if (tmpFilterOperator == "0")
                    {
                        where += " AND ";
                    }
                    else
                    {
                        where += " OR ";
                    }
                }
                // build the "WHERE" clause depending on the filter's condition, value and datafield.
                where += this.GetFilterCondition(filterCondition, filterDataField, filterValue);
                if (i == filtersCount - 1)
                {
                    where += ")";
                }
                tmpFilterOperator = filterOperator;
                tmpDataField = filterDataField;
            }
            queryString += where;
            return queryString;
        }

        private string GetFilterCondition(string filterCondition, string filterDataField, string filterValue)
        {
            if (filterDataField == "MPID")
            {
                filterDataField = "order_id";
            }
            switch (filterCondition)
            {
                case "NOT_EMPTY":
                case "NOT_NULL":
                    return " " + filterDataField + " NOT LIKE '" + "" + "'";
                case "EMPTY":
                case "NULL":
                    return " " + filterDataField + " LIKE '" + "" + "'";
                case "CONTAINS_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '%" + filterValue + "%'" + "  ";
                case "CONTAINS":
                    return " " + filterDataField + " LIKE '%" + filterValue + "%'";
                case "DOES_NOT_CONTAIN_CASE_SENSITIVE":
                    return " " + filterDataField + " NOT LIKE '%" + filterValue + "%'" + "  "; ;
                case "DOES_NOT_CONTAIN":
                    return " " + filterDataField + " NOT LIKE '%" + filterValue + "%'";
                case "EQUAL_CASE_SENSITIVE":
                    return " " + filterDataField + " = '" + filterValue + "'" + "  "; ;
                case "EQUAL":
                    return " " + filterDataField + " = '" + filterValue + "'";
                case "NOT_EQUAL_CASE_SENSITIVE":
                    return " BINARY " + filterDataField + " <> '" + filterValue + "'";
                case "NOT_EQUAL":
                    return " " + filterDataField + " <> '" + filterValue + "'";
                case "GREATER_THAN":
                    return " " + filterDataField + " > '" + filterValue + "'";
                case "LESS_THAN":
                    return " " + filterDataField + " < '" + filterValue + "'";
                case "GREATER_THAN_OR_EQUAL":
                    if (filterDataField == "InSellerCloud")
                    {

                        return "convert( " + filterDataField + ",date) >= '" + filterValue + "'";
                    }
                    else
                    {
                        return " " + filterDataField + " >= '" + filterValue + "'";
                    }
                case "LESS_THAN_OR_EQUAL":
                    if (filterDataField == "InSellerCloud")
                    {
                        return "convert( " + filterDataField + ",date) <= '" + filterValue + "'";
                    }
                    else
                    {
                        return " " + filterDataField + " <= '" + filterValue + "'";
                    }

                case "STARTS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '" + filterValue + "%'" + "  ";
                case "STARTS_WITH":
                    return " " + filterDataField + " LIKE '" + filterValue + "%'";
                case "ENDS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '%" + filterValue + "'" + "  ";
                case "ENDS_WITH":
                    return " " + filterDataField + " LIKE '%" + filterValue + "'";
            }
            return "";
        }

        private List<SKUSalesHistoryFromOrders> GetSKUHistoryDataDynamicQuery(string query)
        {
            List<SKUSalesHistoryFromOrders> listSkuSlaesHistory = null;
            try
            {
                //using (MySqlConnection conn = new MySqlConnection(connectionString))
                //{
                //    conn.Open();
                //    MySqlCommand cmd = new MySqlCommand(query, conn);
                //    cmd.CommandType = System.Data.CommandType.Text;



                //    using (var reader = cmd.ExecuteReader())
                //    {
                //        if (reader.HasRows)
                //        {
                //            listSkuSlaesHistory = new List<SKUSalesHistoryFromOrders>();
                //            while (reader.Read())
                //            {
                //                SKUSalesHistoryFromOrders model = new SKUSalesHistoryFromOrders();
                //                model.SellerCloudID = Convert.ToString(reader["sellerCloudID"] != DBNull.Value ? reader["sellerCloudID"] : string.Empty);
                //                model.MPID = Convert.ToString(reader["order_id"] != DBNull.Value ? reader["order_id"] : string.Empty);
                //                model.InSellerCloud = Convert.ToDateTime(reader["inSellerCloud"] != DBNull.Value ? reader["inSellerCloud"] : DateTime.Now);
                //                model.TotalQuantity = Convert.ToInt32(reader["quantity"] != DBNull.Value ? reader["quantity"] : 0);
                //                model.TotalPrice = Convert.ToDecimal(reader["total_price"] != DBNull.Value ? reader["total_price"] : 0);
                //                model.TotalComission = Convert.ToDecimal(reader["total_commission"] != DBNull.Value ? reader["total_commission"] : 0);
                //                model.TaxGST = Convert.ToDecimal(reader["TaxGST"] != DBNull.Value ? reader["TaxGST"] : 0);
                //                model.TaxPst = Convert.ToDecimal(reader["TaxPST"] != DBNull.Value ? reader["TaxPST"] : 0);
                //                model.AverageCost = Convert.ToDecimal(reader["avg_cost"] != DBNull.Value ? reader["avg_cost"] : 0);
                //                model.RowID = Convert.ToInt32(reader["TotalRecords"] != DBNull.Value ? reader["TotalRecords"] : 0);
                //                model.ProductAvgCost = Convert.ToDecimal(reader["avg_cost"] != DBNull.Value ? reader["avg_cost"] : 0);

                //                model.UnitPrice = Math.Round(model.TotalPrice / model.TotalQuantity, 2);

                //                model.calculation_TotalAmountOfUnitPrice = model.TotalQuantity * model.UnitPrice;
                //                model.calculation_TotalAvgCost = model.TotalQuantity * model.ProductAvgCost;
                //                model.calculation_TotalTax = model.TaxGST + model.TaxPst;
                //                model.calculation_TotalTacPercentage = Math.Round((model.calculation_TotalTax / model.calculation_TotalAmountOfUnitPrice) * 100, 2);
                //                model.calculation_Comission = Math.Round((model.TotalComission) / (1 + model.calculation_TotalTacPercentage / 100), 2);
                //                model.caculation_TotalAvgCost = Math.Round(Math.Round(Convert.ToDecimal(model.AverageCost), 2) * model.TotalQuantity, 2);
                //                model.calculation_SumTotal = Math.Round(model.calculation_TotalTax + model.calculation_TotalAmountOfUnitPrice, 2);
                //                model.calculation_comissionPercentage = Math.Round(((model.calculation_Comission / model.calculation_TotalAmountOfUnitPrice) * 100), 2);
                //                model.calculation_ProfitLoss = Math.Round(model.calculation_TotalAmountOfUnitPrice - 0 - model.caculation_TotalAvgCost - model.calculation_Comission, 2);
                //                model.calculation_ProfitLossPercentage = Math.Round((model.calculation_ProfitLoss / model.calculation_TotalAmountOfUnitPrice) * 100, 2);

                //                listSkuSlaesHistory.Add(model);
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
            }
            return listSkuSlaesHistory;
        }
    }
}