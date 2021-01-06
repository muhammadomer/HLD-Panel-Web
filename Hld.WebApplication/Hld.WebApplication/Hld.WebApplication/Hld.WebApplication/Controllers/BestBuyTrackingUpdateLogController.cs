using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PagedList;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class BestBuyTrackingUpdateLogController : Controller
    {
        private readonly IConfiguration configuration = null;
        string connectionString = "";
        string ApiURL = "";
        string token = "";

        BestBuyTrackingUpdateLogApiAccess _bestBuyTrackingUpdateLogApiAccess = null;
        public BestBuyTrackingUpdateLogController(IConfiguration configuration)
        {
            this.configuration = configuration;
            ApiURL = configuration.GetValue<string>("WebApiURL:URL");
            connectionString = this.configuration.GetValue<string>("ConnectionString:bbe2HldPHP");
            _bestBuyTrackingUpdateLogApiAccess = new BestBuyTrackingUpdateLogApiAccess();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetTracking()
       {
            var querystring = Request.QueryString;
            string dynamicQueryGeneration = "";
            List<BestBuyTrackingUpdate> listModel = null;
            NameValueCollection query = HttpUtility.ParseQueryString(Request.QueryString.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            if (filtersCount > 0)
            {
                dynamicQueryGeneration = BuildTrackingQuery(querystring);
                listModel = GetTrackingDynamicQuery(dynamicQueryGeneration);
                if (listModel != null && listModel.Count > 0)
                {
                    string TrackingList = JsonConvert.SerializeObject(listModel);

                    HttpContext.Session.SetString("exportTrackingData", TrackingList);
                }
                else
                {

                    listModel = new List<BestBuyTrackingUpdate>();
                    string TrackingList = JsonConvert.SerializeObject(listModel);
                    HttpContext.Session.SetString("exportTrackingData", TrackingList);
                }
            }
            else
            {

                listModel = TrackingData();
                if (listModel != null && listModel.Count > 0)
                {
                    string TrackingList = JsonConvert.SerializeObject(listModel);


                    HttpContext.Session.SetString("exportTrackingData", TrackingList);
                }
                else
                {
                    listModel = new List<BestBuyTrackingUpdate>();
                    string TrackingList = JsonConvert.SerializeObject(listModel);
                    HttpContext.Session.SetString("exportTrackingData", TrackingList);
                }


            }

            return Json(listModel);
        }

        private string BuildTrackingQuery(QueryString queryStringField)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(queryStringField.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            var queryString = @"SELECT shipDate,scOrderID,bbOrderID,trackingNumber,shippingServiceCode,case when inBestbuy=1 then 'Update' else 'Pending' end as BBStatus FROM bestBuyE2.trackingExport ";
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
                where += this.GetFilterTrackingCondition(filterCondition, filterDataField, filterValue);
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

        private string GetFilterTrackingCondition(string filterCondition, string filterDataField, string filterValue)
        {
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
                    if (filterDataField == "shipDate")
                    {
                        return "convert( " + filterDataField + ",date) >= '" + filterValue + "'";
                    }
                    else
                    {
                        return " " + filterDataField + " >= '" + filterValue + "'";
                    }
                case "LESS_THAN_OR_EQUAL":
                    if (filterDataField == "shipDate")
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

        public List<BestBuyTrackingUpdate> TrackingData()
        {
            token = Request.Cookies["Token"];
            List<BestBuyTrackingUpdate> listModel = new List<BestBuyTrackingUpdate>();
            listModel = _bestBuyTrackingUpdateLogApiAccess.GetTrackingData(ApiURL, token);
            //try
            //{
            //    using (MySqlConnection conn = new MySqlConnection(connectionString))
            //    {
            //        conn.Open();
            //        MySqlCommand cmd = new MySqlCommand(@"SELECT shipDate,scOrderID,bbOrderID,trackingNumber,shippingServiceCode,case when inBestbuy=1 then 'Update' else 'Pending' end as BBStatus FROM bestBuyE2.trackingExport
            //                           order by shipDate desc limit 500; ", conn);
            //        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //        DataTable dataTable = new DataTable();
            //        da.Fill(dataTable);
            //        if (dataTable.Rows.Count > 0)
            //        {
            //            listModel = new List<BestBuyTrackingUpdate>();
            //            foreach (DataRow dataRow in dataTable.Rows)
            //            {
            //                BestBuyTrackingUpdate model = new BestBuyTrackingUpdate();
            //                model.trackingNumber = Convert.ToString(dataRow["trackingNumber"] != DBNull.Value ? dataRow["trackingNumber"] : "0");
            //                model.BBStatus = Convert.ToString(dataRow["BBStatus"] != DBNull.Value ? dataRow["BBStatus"] : "0");
            //                model.shipDate = Convert.ToDateTime(dataRow["shipDate"] != DBNull.Value ? dataRow["shipDate"] : "0");
            //                model.scOrderID = Convert.ToString(dataRow["scOrderID"] != DBNull.Value ? dataRow["scOrderID"] : "0");
            //                model.shippingServiceCode = Convert.ToString(dataRow["shippingServiceCode"] != DBNull.Value ? dataRow["shippingServiceCode"] : "0");
            //                model.bbOrderID = Convert.ToString(dataRow["bbOrderID"] != DBNull.Value ? dataRow["bbOrderID"] : "0");
            //                listModel.Add(model);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            return listModel;
        }

        private List<BestBuyTrackingUpdate> GetTrackingDynamicQuery(string query)
        {
            List<BestBuyTrackingUpdate> listModel = new List<BestBuyTrackingUpdate>();
            token = Request.Cookies["Token"];
            listModel = _bestBuyTrackingUpdateLogApiAccess.GetTrackingDynamicQuery(ApiURL, token, query);
            //try
            //{
            //    using (MySqlConnection conn = new MySqlConnection(connectionString))
            //    {
            //        conn.Open();
            //        MySqlCommand cmd = new MySqlCommand(query, conn);
            //        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //        DataTable dataTable = new DataTable();
            //        da.Fill(dataTable);
            //        if (dataTable.Rows.Count > 0)
            //        {
            //            listModel = new List<BestBuyTrackingUpdate>();
            //            foreach (DataRow dataRow in dataTable.Rows)
            //            {
            //                BestBuyTrackingUpdate model = new BestBuyTrackingUpdate();
            //                model.trackingNumber = Convert.ToString(dataRow["trackingNumber"] != DBNull.Value ? dataRow["trackingNumber"] : "0");
            //                model.BBStatus = Convert.ToString(dataRow["BBStatus"] != DBNull.Value ? dataRow["BBStatus"] : "0");
            //                model.shipDate = Convert.ToDateTime(dataRow["shipDate"] != DBNull.Value ? dataRow["shipDate"] : "0");
            //                model.scOrderID = Convert.ToString(dataRow["scOrderID"] != DBNull.Value ? dataRow["scOrderID"] : "0");
            //                model.shippingServiceCode = Convert.ToString(dataRow["shippingServiceCode"] != DBNull.Value ? dataRow["shippingServiceCode"] : "0");
            //                model.bbOrderID = Convert.ToString(dataRow["bbOrderID"] != DBNull.Value ? dataRow["bbOrderID"] : "0");
            //                listModel.Add(model);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            return listModel;
        }


        public IActionResult BestBuyTracking(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string scOrderID = "", string bbOrderID = "", string trackingNumber = "", string BBStatus = "", string EmptyFirstTime = "")
        {
           
            token = Request.Cookies["Token"];
            BestBuyTrackingUpdate trackingUpdate = new BestBuyTrackingUpdate();
            string CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
            string PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");

            if ("0001-01-01"== orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate ="";
                PreviousDate ="";
            }



            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {

            }
            else
            {
                var count = _bestBuyTrackingUpdateLogApiAccess.GetCount(ApiURL, token, CurrentDate, PreviousDate, scOrderID, bbOrderID, trackingNumber, BBStatus);
                ViewBag.TotalCount = count;
               
            }
                return View(trackingUpdate);
        }
        [HttpPost]
        public IActionResult BuyTrackingUpdateLogList(int? page, DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string scOrderID = "", string bbOrderID = "", string trackingNumber = "", string BBStatus = "", string EmptyFirstTime = "")
        {

            //string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            //string PreviousDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            string CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
            string PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");
            if ("0001-01-01" == orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = "";
                PreviousDate = "";
            }
            token = Request.Cookies["Token"];
            
            if (page.HasValue)
            {
                ViewBag.pageNumber = page;
            }
            IPagedList<BestBuyTrackingUpdate> data = null; 
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }          
            List<BestBuyTrackingUpdate> _viewModel = new List<BestBuyTrackingUpdate>();
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                _viewModel = _bestBuyTrackingUpdateLogApiAccess.BuyTrackingUpdateLogList(ApiURL, token, CurrentDate, PreviousDate, 0, 0,scOrderID, bbOrderID, trackingNumber, BBStatus);
                data = new StaticPagedList<BestBuyTrackingUpdate>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                return PartialView("~/Views/BestBuyTrackingUpdateLog/BestBuyTrackingUpdateParitalView.cshtml", data);
            }

            _viewModel = _bestBuyTrackingUpdateLogApiAccess.BuyTrackingUpdateLogList(ApiURL, token, CurrentDate, PreviousDate, startlimit, endLimit, scOrderID, bbOrderID, trackingNumber, BBStatus);
            data = new StaticPagedList<BestBuyTrackingUpdate>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            return PartialView("~/Views/BestBuyTrackingUpdateLog/BestBuyTrackingUpdateParitalView.cshtml", data);

               
        }
    }
}