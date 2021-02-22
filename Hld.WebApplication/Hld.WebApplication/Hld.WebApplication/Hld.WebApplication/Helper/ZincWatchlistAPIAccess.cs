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
    public class ZincWatchlistAPIAccess
    {
        public bool AddASINToWatchList(string ApiURL, string token, SaveWatchlistViewModel ViewModel)
        {
            bool status = false;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }
        public bool AddASINToWatchListNew(string ApiURL, string token, SaveWatchlistViewModel ViewModel)
        {
            bool status = false;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return status;
        }

        public SaveWatchlistViewModel GetWatchList(string ApiURL, string token, string ASIN)
        {
            SaveWatchlistViewModel ViewModel = new SaveWatchlistViewModel();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/" + ASIN);
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
                ViewModel = JsonConvert.DeserializeObject<SaveWatchlistViewModel>(strResponse);
                return ViewModel;

            }
            catch (Exception)
            {
                throw;
            }

        }


        public List<ZincWatchListSummaryViewModal> GetWatchListSummary(string ApiURL, string token, int offset)
        {
            List<ZincWatchListSummaryViewModal> listmodel = new List<ZincWatchListSummaryViewModal>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/getsummary/" + offset);
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
                listmodel = JsonConvert.DeserializeObject<List<ZincWatchListSummaryViewModal>>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return listmodel;
        }


        public ZincWatchListSummaryViewModal GetWatchListSummaryByID(string ApiURL, string token, int JobID)
        {
            ZincWatchListSummaryViewModal listmodel = new ZincWatchListSummaryViewModal();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetlogsbyID/" + JobID);
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
                listmodel = JsonConvert.DeserializeObject<ZincWatchListSummaryViewModal>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return listmodel;
        }
        public List<ZincWatchlistLogsViewModel> GetWatchListLogs(string ApiURL, string token, ZincWatchLogsSearchViewModel searchViewModel)
        {
            List<ZincWatchlistLogsViewModel> ViewModel = new List<ZincWatchlistLogsViewModel>();
            try
            {
                var data = JsonConvert.SerializeObject(searchViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/Getlogs");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<List<ZincWatchlistLogsViewModel>>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return ViewModel;
        }
        public List<ZincWatchlistLogsViewModel> UpdateBestBuyForAllPages(string ApiURL, string token, ZincWatchLogsSearchViewModel searchViewModel)
        {
            List<ZincWatchlistLogsViewModel> ViewModel = new List<ZincWatchlistLogsViewModel>();
            try
            {
                var data = JsonConvert.SerializeObject(searchViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/UpdateBestBuyForAllPages");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<List<ZincWatchlistLogsViewModel>>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return ViewModel;
        }

        public int SaveBestBuyUpdateList(string ApiURL, string token, List<ZincWatchlistLogsViewModel> searchViewModel)
        {
            int ID = 0;
            //List<ZincWatchlistLogsViewModel> ViewModel = new List<ZincWatchlistLogsViewModel>();
            try
            {
                var data = JsonConvert.SerializeObject(searchViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/SaveBestBuyUpdateList");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ID = Convert.ToInt32(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return ID;
        }

        public int GetWatchListLogsCount(string ApiURL, string token, ZincWatchLogsSearchViewModel searchViewModel)
        {
            int ViewModel = 0;
            try
            {
                var data = JsonConvert.SerializeObject(searchViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetlogsCount");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<int>(strResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ViewModel;
        }

        public int GetWatchListSummaryCount(string ApiURL, string token)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetSummaryCount");
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
        public bool GetWatchListStatus(string ApiURL, string token, ASINActiveStatusViewModel saveWatchlistViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(saveWatchlistViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/ChangeStatus");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                status = JsonConvert.DeserializeObject<bool>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return status;
        }

        public int GetUpdateStatus(string ApiURL, string token, string value)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/UpdateZincJobStatus/" + value);
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
        public int UpdateZincJobSwitch(string ApiURL, string token, string value)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/UpdateZincJobSwitch/" + value);
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
        public ZincWatchListJobViewModel GetWatchListStatusZinc(string ApiURL, string token)
        {
            ZincWatchListJobViewModel obj = new ZincWatchListJobViewModel();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetZincWatchListStatus");
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
                obj = JsonConvert.DeserializeObject<ZincWatchListJobViewModel>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return obj;
        }
        public bool UpdatePriceMax(string ApiURL, string token, string ASIN, double MaxPrice)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/UpdateASINMaxPrice/" + ASIN + "/" + MaxPrice);
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

        public int BestBuyUpdateLogView(string ApiURL, string token)
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetSummaryCount");
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

        public int GetWatchlistLogsSelectAllCount(string ApiURL, string token, ZincWatchLogsSearchViewModel searchViewModel)
        {
            int ViewModel = 0;
            try
            {
                var data = JsonConvert.SerializeObject(searchViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetWatchlistLogsSelectAllCount");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<int>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return ViewModel;
        }
        public ZincWatchlistCountViewModel GetCount(string ApiURL, string token, ZincWatchLogsSearchViewModel searchViewModel)
        {
            ZincWatchlistCountViewModel ViewModel = new ZincWatchlistCountViewModel();
            try
            {
                var data = JsonConvert.SerializeObject(searchViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetCount");
                request.Method = "POST";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;
                string strResponse = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = sr.ReadToEnd();
                }
                ViewModel = JsonConvert.DeserializeObject<ZincWatchlistCountViewModel>(strResponse);
            }
            catch (Exception)
            {
                throw;
            }
            return ViewModel;
        }
        public int GetZincCount(string ApiURL, string token, string StartDate, string EndDate, string SC_Order_ID = "", string Amazon_AcName = "", string Zinc_Status = "")
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetZincCount?SC_Order_ID=" + SC_Order_ID + "&Amazon_AcName=" + Amazon_AcName + "&Zinc_Status=" + Zinc_Status + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate);
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
                        Count = (int)JObject.Parse(strResponse)["counter"];
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }

        public List<ZincOrdersLogViewModel> ZincOrdersLogList(string apiurl, string token, string DateTo, string DateFrom, int limit, int offset, string SC_Order_ID = "", string Amazon_AcName = "", string Zinc_Status = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ZincWatchList/ZincOrdersLogList?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&SC_Order_ID=" + SC_Order_ID + "&Amazon_AcName=" + Amazon_AcName + "&Zinc_Status=" + Zinc_Status + "&limit=" + limit + "&offset=" + offset);
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
            List<ZincOrdersLogViewModel> responses = JsonConvert.DeserializeObject<List<ZincOrdersLogViewModel>>(strResponse);
            return responses;
        }

        public int GetWatchListLogsCountForJob(string ApiURL, string token, string StartDate, string EndDate, string ASIN, string ProductSKU, string available, string jobID)
        {
            int Count = 0;
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetlogsCountForJob?ASIN=" + ASIN  +"&SKU=" + ProductSKU + "&available=" + available + "&jobID=" + jobID+ "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate);
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
                        Count = (int)JObject.Parse(strResponse)["counter"];
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }

        public List<ZincWatchlistLogsForJobViewModel> GetWatchListLogsForJob(string apiurl, string token, string DateTo, string DateFrom, int limit, int offset, string ASIN, string SKU, string available, string jobID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ZincWatchList/ZincOrdersLogListForJob?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&ASIN=" + ASIN + "&SKU=" + SKU + "&available=" + available + "&jobID=" + jobID + "&limit=" + limit + "&offset=" + offset);
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
            List<ZincWatchlistLogsForJobViewModel> responses = JsonConvert.DeserializeObject<List<ZincWatchlistLogsForJobViewModel>>(strResponse);
            return responses;
        }
        public int GetZincWatchlistCount(string ApiURL, string token, string StartDate, string EndDate, string ProductSKU = "", string ASIN = "", string Active_Inactive = "", string Enabled_Disabled = "")
        {
            int Count = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/GetZincWatchlistCount?ProductSKU=" + ProductSKU + "&ASIN=" + ASIN + "&Active_Inactive=" + Active_Inactive + "&CurrentDate=" + StartDate + "&PreviousDate=" + EndDate + "&Enabled_Disabled=" + Enabled_Disabled);
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
                        Count = (int)JObject.Parse(strResponse)["counter"];
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Count;
        }

        public List<ZincWatclistViewModel> ZincWatchListDetail(string apiurl, string token, string DateTo, string DateFrom, int limit, int offset, string ProductSKU = "", string ASIN = "", string Active_Inactive = "", string Enabled_Disabled = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(apiurl + "/api/ZincWatchList/ZincWatchListDetail?DateFrom=" + DateFrom + "&DateTo=" + DateTo + "&ProductSKU=" + ProductSKU + "&ASIN=" + ASIN + "&Active_Inactive=" + Active_Inactive + "&limit=" + limit + "&offset=" + offset + "&Enabled_Disabled=" + Enabled_Disabled);
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
            List<ZincWatclistViewModel> responses = JsonConvert.DeserializeObject<List<ZincWatclistViewModel>>(strResponse);
            return responses;
        }
        public ZincWatclistLogsViewModel logHistory(string ApiURL, string token, string ProductSKU, string ASIN)
        {
            //List<ZincWatclistLogHistoryViewModel> model = new List<ZincWatclistLogHistoryViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincWatchList/logHistory?ProductSKU=" + ProductSKU + "&ASIN=" + ASIN);
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
                ZincWatclistLogsViewModel responses = JsonConvert.DeserializeObject<ZincWatclistLogsViewModel>(strResponse);
                return responses;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }

       
    }
}
