using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class ProductWarehouseQtyApiAccess
    {
        public bool SaveProductWarehouseQty(string ApiURL, string token, List<ProductWarehouseQtyViewModel> ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ProductWarehouseQty/SaveProductQty");
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
                    status = true;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return status;
        }

        public List<ProductWarehouseQtyViewModel> GetProductWarehouseQtyFromDatabase(string ApiURL, string token,  ProductWarehouseQtyViewModel  ViewModel)
        {
            List<ProductWarehouseQtyViewModel> responses = null;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ProductWarehouseQty/GetProductWarehouseQtyFromDatabase");
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
                 responses = JsonConvert.DeserializeObject<List<ProductWarehouseQtyViewModel>>(strResponse);

            }
            catch (Exception)
            {
                throw;
            }
            return responses;
        }
        
        public bool SaveBestBuyQtyMovementForDropship(string ApiURL, string token, BestBuyDropShipQtyMovementViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ProductWarehouseQty/SaveBestBuyQtyMovementForDropshipNone_SKU");
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
                    status = true;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return status;
        }
        public ProductWarehouseQuantityViewModel GetProductWarehouseFormSC(string ApiURL, string token, string sku,int wID)
        {
            ProductWarehouseQuantityViewModel listmodel = new ProductWarehouseQuantityViewModel();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/Inventory/"+ sku + "/Warehouses/" + wID);
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
                listmodel = JsonConvert.DeserializeObject<ProductWarehouseQuantityViewModel>(strResponse);


            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }


        public List<ProductWarehouseQtyViewModel> GetProductWarehouseQtyFromDatabaseInvent(string ApiURL, string token, string SKU)
        {
            List<ProductWarehouseQtyViewModel> responses = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Product/" + SKU);
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
                responses = JsonConvert.DeserializeObject<List<ProductWarehouseQtyViewModel>>(strResponse);
            }
            catch (Exception)
            {
            }
            return responses;
        }
    }


}
