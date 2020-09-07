using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.ViewModel;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Hld.WebApplication.Controllers
{

    [TokenExpires]
    public class BestBuyDropshipQtyMovementController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string connectionString = "";


        BestBuyDropshipQtyMovementApiAccess _dropshipQtyMovementApiAccess = null;
        public BestBuyDropshipQtyMovementController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            connectionString = _configuration.GetValue<string>("ConnectionString:bb2HldNet");
            _dropshipQtyMovementApiAccess = new BestBuyDropshipQtyMovementApiAccess();

        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult AllDropshipQtyMovementList()
        {

            return View();
        }
        public IActionResult GetQtyLogs()
        {
            var querystring = Request.QueryString;
            string dynamicQueryGeneration = "";
            List<BestBuyQTYLogsDetailViewModel> listModel = null;
            NameValueCollection query = HttpUtility.ParseQueryString(Request.QueryString.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            if (filtersCount > 0)
            {
                dynamicQueryGeneration = BuildLOgsQuery(querystring);
                listModel = GetQtyLogsDynamicQuery(dynamicQueryGeneration);


            }
            else
            {

                listModel = LogsData();


            }

            return Json(listModel);
        }

        private string BuildLOgsQuery(QueryString queryStringField)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(queryStringField.Value);
            var filtersCount = int.Parse(query.GetValues("filterscount")[0]);
            var queryString = @"SELECT 
`BestBuyDropsShipQtyMovement`.`product_sku`,ds_qty,ifnull(order_qty,0) ,ifnull(BestBuyProducts.product_sku,'') BBProductID,
case when ds_status=false then 'Dropship None'  else 'DropShip' End  as ds_status ,    
`BestBuyDropsShipQtyMovement`.`order_date`,
case when `BestBuyDropsShipQtyMovement`.`is_ds_status_updated_id`=1 then 'Updated' else 'Pending' end as update_status,
`BestBuyDropsShipQtyMovement`.`bb_import_id` ,ifnull(dropship_comments,'') as comments
FROM `bestBuyE2`.`BestBuyDropsShipQtyMovement`
inner join BestBuyProducts  
on BestBuyProducts.shop_sku_offer_sku=`bestBuyE2`.`BestBuyDropsShipQtyMovement`.product_sku";
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
                    if (filterDataField == "update_status")
                    {
                        filterDataField = "is_ds_status_updated_id";
                    }
                    if (filterDataField == "product_sku")
                    {
                        filterDataField = "`BestBuyDropsShipQtyMovement`.`product_sku`";
                    }
                    if (filterDataField == "comments")
                    {
                        filterDataField = "`dropship_comments`";
                    }
                    tmpDataField = filterDataField;
                }
                else if (tmpDataField != filterDataField)
                {
                    where += ") AND (";
                    if (filterDataField == "update_status")
                    {
                        filterDataField = "is_ds_status_updated_id";
                    }
                    if (filterDataField == "product_sku")
                    {
                        filterDataField = "`BestBuyDropsShipQtyMovement`.`product_sku`";
                    }
                    if (filterDataField == "comments")
                    {
                        filterDataField = "`dropship_comments`";
                    }
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
                where += this.GetFilterQtyCondition(filterCondition, filterDataField, filterValue);
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

        private string GetFilterQtyCondition(string filterCondition, string filterDataField, string filterValue)
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
                    if (filterDataField == "order_date")
                    {
                        return "convert( " + filterDataField + ",date) >= '" + filterValue + "'";
                    }
                    else
                    {
                        return " " + filterDataField + " >= '" + filterValue + "'";
                    }
                case "LESS_THAN_OR_EQUAL":
                    if (filterDataField == "order_date")
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

        private List<BestBuyQTYLogsDetailViewModel> GetQtyLogsDynamicQuery(string query)
        {


            token = Request.Cookies["Token"];
            List<BestBuyQTYLogsDetailViewModel> listModel = new List<BestBuyQTYLogsDetailViewModel>();
            listModel = _dropshipQtyMovementApiAccess.GetBestBuyDropshipQty(ApiURL, token, query);
            //List<BestBuyQTYLogsDetailViewModel> listModel = null;
            //try
            //{
            //    using (MySqlConnection conn = new MySqlConnection(connectionString))
            //    {
            //        conn.Open();
            //        MySqlCommand cmd = new MySqlCommand(query+ " Order by order_date desc", conn);
            //        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //        DataTable dataTable = new DataTable();
            //        da.Fill(dataTable);
            //        if (dataTable.Rows.Count > 0)
            //        {
            //            listModel = new List<BestBuyQTYLogsDetailViewModel>();
            //            foreach (DataRow dataRow in dataTable.Rows)
            //            {
            //                BestBuyQTYLogsDetailViewModel ViewModel = new BestBuyQTYLogsDetailViewModel();
            //                ViewModel.product_sku = Convert.ToString(dataRow["product_sku"]);
            //                ViewModel.ds_qty = Convert.ToInt32(dataRow["ds_qty"]);
            //                ViewModel.ds_status = Convert.ToString(dataRow["ds_status"]);
            //                ViewModel.order_date = Convert.ToDateTime(dataRow["order_date"]);
            //                ViewModel.update_status = Convert.ToString(dataRow["update_status"]);
            //                ViewModel.bb_import_id = Convert.ToInt32(dataRow["bb_import_id"] != DBNull.Value ? dataRow["bb_import_id"] : 0);
            //                ViewModel.comments = Convert.ToString(dataRow["comments"] != DBNull.Value ? dataRow["comments"] : "");
            //                ViewModel.BBProductID = Convert.ToString(dataRow["BBProductID"] != DBNull.Value ? dataRow["BBProductID"] : "");
            //                listModel.Add(ViewModel);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            return listModel;
        }

        public List<BestBuyQTYLogsDetailViewModel> LogsData()
        {
            List<BestBuyQTYLogsDetailViewModel> listModel = new List<BestBuyQTYLogsDetailViewModel>();
            listModel = _dropshipQtyMovementApiAccess.GetBestBuyDropshipQtyMovement(ApiURL, token);
            //    List<BestBuyQTYLogsDetailViewModel> listModel = null;
            //    try
            //    {
            //        using (MySqlConnection conn = new MySqlConnection(connectionString))
            //        {
            //            conn.Open();
            //            MySqlCommand cmd = new MySqlCommand("p_GetBestBuyQtyMovementDetailToShowOnWebPage", conn);
            //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //            using (var reader = cmd.ExecuteReader())
            //            {
            //                if (reader.HasRows)
            //                {
            //                    listModel = new List<BestBuyQTYLogsDetailViewModel>();
            //                    while (reader.Read())
            //                    {
            //                        BestBuyQTYLogsDetailViewModel ViewModel = new BestBuyQTYLogsDetailViewModel();
            //                        ViewModel.product_sku = Convert.ToString(reader["product_sku"]);
            //                        ViewModel.ds_qty = Convert.ToInt32(reader["ds_qty"]);
            //                        ViewModel.ds_status = Convert.ToString(reader["ds_status"]);
            //                        ViewModel.order_date = Convert.ToDateTime(reader["order_date"]);
            //                        ViewModel.update_status = Convert.ToString(reader["update_status"]);
            //                        ViewModel.bb_import_id = Convert.ToInt32(reader["bb_import_id"] != DBNull.Value ? reader["bb_import_id"] : 0);
            //                        ViewModel.comments = Convert.ToString(reader["comments"] != DBNull.Value ? reader["comments"] : "");
            //                        ViewModel.BBProductID = Convert.ToString(reader["BBProductID"] != DBNull.Value ? reader["BBProductID"] : "");
            //                        listModel.Add(ViewModel);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            return listModel;
        }

    }
}