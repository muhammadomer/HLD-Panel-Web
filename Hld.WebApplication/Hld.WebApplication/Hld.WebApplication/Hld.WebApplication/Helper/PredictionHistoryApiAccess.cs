using DataAccess.ViewModels;
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
    public class PredictionHistoryApiAccess
    {
        public int SavePO(string ApiURL, string token, PurchaseOrderDataViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/SavePO");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Id;
        }

        public int SavePOItem(string ApiURL, string token, PredictPOItemViewModel ViewModel)
        {
            int Id = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/SavePOItem");
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
                    Id = Convert.ToInt32(strResponse);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Id;
        }

        public List<PredictionHistroyViewModel> GetPredictionDetail(string ApiURL, string token, int startLimit, int offset, int VendorId, string SKU, string Title, bool Approved, bool Excluded, string Sort, string SortedType, int Type = 0)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy?startLimit=" + startLimit + "&offset=" + offset + "&VendorId=" + VendorId + "&SKU=" + SKU + "&Title=" + Title + "&Approved=" + Approved + "&Excluded=" + Excluded + "&Sort=" + Sort + "&SortedType=" + SortedType + "&Type=" + Type);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            //request.Timeout = Timeout.Infinite;
            //request.KeepAlive = true;
            string strResponse = "";


            using (WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<PredictionHistroyViewModel> responses = JsonConvert.DeserializeObject<List<PredictionHistroyViewModel>>(strResponse);

            return responses;
        }
        public int PredictionSummaryCount(string ApiURL, string token, int VendorId, string SKU, string Title, bool Approved, bool Excluded, int Type = 0)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionSummaryCount?VendorId=" + VendorId + "&SKU=" + SKU + "&Title=" + Title + "&Approved=" + Approved + "&Excluded=" + Excluded + "&Type=" + Type);
                request.Method = "GET"; ;
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
        public PredictionPOViewModel GetPO(string ApiURL, string token, int Id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/Item?Id=" + Id);
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
            var responses = JsonConvert.DeserializeObject<PredictionPOViewModel>(strResponse);

            return responses;
        }

        public int SCSavePO(string ApiURL, string token, SCPOViewModel ViewModel)
        {
            int status = 0;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lp.api.sellercloud.com/rest/api/PurchaseOrders");
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
                    status = Convert.ToInt32(JObject.Parse(strResponse)["Id"]);
                }
            }
            catch (Exception ex)
            {
                status = 0;
                throw;
            }
            return status;
        }

        public List<PredictionInternalPOList> GetInternalPOIdBySku(string ApiURL, string token, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/GetInternalPOIdBySku?SKU=" + SKU);
            request.Method = "GET";
            request.Accept = "application/json;";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;
            string strResponse = "";


            using ( WebResponse webResponse = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponse = stream.ReadToEnd();
                }
            }
            List<PredictionInternalPOList> responses = JsonConvert.DeserializeObject<List<PredictionInternalPOList>>(strResponse);

            return responses;
        }

        public bool GetOnDemand(string ApiURL, string token, string SKU)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/GetCatalog/" + SKU);
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
                // Count = JsonConvert.DeserializeObject<int>(strResponse);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeletePO(string ApiURL, string token, int Id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/DeletePO?Id=" + Id);
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

        public bool DeletePOItem(string ApiURL, string token, int Id)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/DeletePOItem?Id=" + Id);
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

        public SoldQtyDaysViewModel GetSoldQtyDays(string ApiURL, string token, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/GetSoldQtyDays?SKU=" + SKU);
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
            SoldQtyDaysViewModel responses = JsonConvert.DeserializeObject<SoldQtyDaysViewModel>(strResponse);

            return responses;
        }

        public List<SKUDetailModel> GetSKUDetailBySku(string ApiURL, string token, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/GetSKUDetailBySku?SKU=" + SKU);
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
            List<SKUDetailModel> responses = JsonConvert.DeserializeObject<List<SKUDetailModel>>(strResponse);

            return responses;
        }

        public List<DraftPOViewModel> DraftPOList(string ApiURL, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/DraftPOList");
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
            List<DraftPOViewModel> responses = JsonConvert.DeserializeObject<List<DraftPOViewModel>>(strResponse);

            return responses;
        }

        public List<WareHouseProductQuantitylistViewModel> GetWareHouseProductQuantitylistBySku(string ApiURL, string token, string SKU)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/GetWareHouseProductQuantitylistBySku?SKU=" + SKU);
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
            List<WareHouseProductQuantitylistViewModel> responses = JsonConvert.DeserializeObject<List<WareHouseProductQuantitylistViewModel>>(strResponse);

            return responses;
        }
        public string PredictIncludedExcluded(string ApiURL, string token, List<PredictIncludedExcludedViewModel> viewModel)
        {
            string status = "";
            try
            {
                var data = JsonConvert.SerializeObject(viewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/PredictIncludedExcluded/");
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
                status = JsonConvert.DeserializeObject<string>(strResponse);
            }
            catch (Exception ex)
            {
            }
            return status;
        }

        public PredictionPOViewModel GetDataForPOCreation(string ApiURL, string token, List<PredictionInternalSKUList> ViewModel)
        {
            PredictionPOViewModel model = new PredictionPOViewModel();
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/PredictionHistroy/GetDataForPOCreation");
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
                    model = JsonConvert.DeserializeObject<PredictionPOViewModel>(strResponse);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return model;
        }

    }


}
