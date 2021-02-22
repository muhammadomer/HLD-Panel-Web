using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagedList.Core;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ZincWatchlistController : Controller

    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        string s3BucketURL = "";
        string s3BucketURL_large = "";

        ZincWatchlistAPIAccess _ApiAccess = null;


        public ZincWatchlistController(IConfiguration configuration)
        {

            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _ApiAccess = new ZincWatchlistAPIAccess();
            s3BucketURL = _configuration.GetValue<string>("S3BucketURL:URL");
            s3BucketURL_large = _configuration.GetValue<string>("S3BucketURL:L_URL");

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddWatchlist(string Asin, string ProductSKU)
        {

            token = Request.Cookies["Token"];
            SaveWatchlistViewModel saveWatchlistViewModel = new SaveWatchlistViewModel();
            saveWatchlistViewModel = _ApiAccess.GetWatchList(ApiURL, token, Asin);
            if (saveWatchlistViewModel == null)
            {
                saveWatchlistViewModel = new SaveWatchlistViewModel();
                saveWatchlistViewModel.ASIN = Asin;
                saveWatchlistViewModel.ProductSKU = ProductSKU;
            }

            return PartialView("~/Views/ZincWatchlist/_AddZincWatchList.cshtml", saveWatchlistViewModel);

        }
        public bool Watchlist(SaveWatchlistViewModel saveWatchlistViewModel)
        {
            token = Request.Cookies["Token"];
            bool status = _ApiAccess.AddASINToWatchList(ApiURL, token, saveWatchlistViewModel);
            return status;
        }
        public bool AddToWatchlist(SaveWatchlistViewModel saveWatchlistViewModel)
        {
            token = Request.Cookies["Token"];
            bool status = _ApiAccess.AddASINToWatchListNew(ApiURL, token, saveWatchlistViewModel);
            return status;
        }

        public IActionResult GetWatchListSummary(int page = 0)
        {
            IPagedList<ZincWatchListSummaryViewModal> data = null;
            string token = Request.Cookies["Token"];
            int pageNumber = page;
            int offset = 0;
            int pageSize = 25;


            offset = (pageNumber - 1) * 25;

            List<ZincWatchListSummaryViewModal> listmodel = new List<ZincWatchListSummaryViewModal>();
            listmodel = _ApiAccess.GetWatchListSummary(ApiURL, token, offset);
            data = new StaticPagedList<ZincWatchListSummaryViewModal>(listmodel, pageNumber, pageSize, listmodel.Count);
            return PartialView("~/Views/ZincWatchlist/GetWatchListSummary.cshtml", data);
        }

        public IActionResult WatchListSummary()
        {
            string token = Request.Cookies["Token"];
            int listmodel = 0;
            listmodel = _ApiAccess.GetWatchListSummaryCount(ApiURL, token);
            ViewBag.Records = listmodel;
            return View();
        }
        [HttpGet]
        public ZincWatchListSummaryViewModal GetWatchListSummaryByJobID(int jobid)
        {
            string token = Request.Cookies["Token"];
            ZincWatchListSummaryViewModal listmodel = new ZincWatchListSummaryViewModal();
            listmodel = _ApiAccess.GetWatchListSummaryByID(ApiURL, token, jobid);
            return listmodel;
        }
        public IActionResult MainViewLogs(string ASIN, string available, string jobID)
        {
            string token = Request.Cookies["Token"];
            int Count = 0;

            ZincWatchLogsSearchViewModel searchViewModel = new ZincWatchLogsSearchViewModel();
            searchViewModel.ASIN = ASIN;
            searchViewModel.JobID = jobID;
            searchViewModel.Available = available;

            Count = _ApiAccess.GetWatchListLogsCount(ApiURL, token, searchViewModel);
            ZincWatchlistCountViewModel obj = new ZincWatchlistCountViewModel();

            obj = _ApiAccess.GetCount(ApiURL, token, searchViewModel);
            ViewBag.Total = obj.Total;
            ViewBag.TotalCount = obj.TotalCount;
            ViewBag.AvailableCount = obj.Available;
            ViewBag.UnAvailableCount = obj.UnAvailable;
            ViewBag.ListingRemovedCount = obj.ListingRemoved;
            ViewBag.logsRecords = Count;
            ViewBag.JobID = jobID;

            if (ASIN != null)
            {
                return View("~/Views/ZincWatchlist/MainViewLogsASIN.cshtml");

            }
            return View();
        }
        public IActionResult ZincWatchListLogs(string ASIN, string available, string jobID, int page = 0)
        {
            string token = Request.Cookies["Token"];
            List<ZincWatchlistLogsViewModel> listmodel = new List<ZincWatchlistLogsViewModel>();
            IPagedList<ZincWatchlistLogsViewModel> data = null;

            int pageNumber = page;
            int offset = 0;
            int pageSize = 100;


            offset = (pageNumber - 1) * 100;
            ZincWatchLogsSearchViewModel searchViewModel = new ZincWatchLogsSearchViewModel();
            searchViewModel.ASIN = ASIN == "undefined" ? "" : ASIN;
            searchViewModel.JobID = jobID == "undefined" ? "" : jobID;
            searchViewModel.Available = available == "undefined" ? "" : available;
            searchViewModel.Offset = offset;
            listmodel = _ApiAccess.GetWatchListLogs(ApiURL, token, searchViewModel);
            ViewBag.Compress_image_URL = s3BucketURL;
            ViewBag.image_name_URL = s3BucketURL_large;

            ViewBag.No_availavle = listmodel.Where(e => e.ZincResponse == "Available").Count();
            ViewBag.No_Unavailavle = listmodel.Where(e => e.ZincResponse == "Currently Unavailable").Count();
            ViewBag.No_Removed = listmodel.Where(e => e.ZincResponse == "Listing Removed").Count();
            ViewBag.No_Total = listmodel.Count();
            data = new StaticPagedList<ZincWatchlistLogsViewModel>(listmodel, pageNumber, pageSize, listmodel.Count);
            if (ASIN != null && ASIN != "undefined")
            {
                return PartialView("~/Views/ZincWatchlist/ZincWatchListLogsASIN.cshtml", data);
            }
            return PartialView("~/Views/ZincWatchlist/ZincWatchListLogs.cshtml", data);
        }

        public bool GetActiveStatus(string ASIN, int Active)
        {
            token = Request.Cookies["Token"];
            ASINActiveStatusViewModel saveWatchlistViewModel = new ASINActiveStatusViewModel();
            saveWatchlistViewModel.ASIN = ASIN;
            saveWatchlistViewModel.Active = Active;
            bool status = _ApiAccess.GetWatchListStatus(ApiURL, token, saveWatchlistViewModel);
            return status;
        }

        public bool UpdateStatus(bool value)
        {
            string val = "";
            if (value == true)
            {
                val = "1";
            }
            else
            {
                val = "0";
            }

            token = Request.Cookies["Token"];


            _ApiAccess.GetUpdateStatus(ApiURL, token, val);
            return true;
        }
        public bool UpdateZincJobSwitch(bool value)
        {
            string val = "";
            if (value == true)
            {
                val = "1";
            }
            else
            {
                val = "0";
            }

            token = Request.Cookies["Token"];


            _ApiAccess.UpdateZincJobSwitch(ApiURL, token, val);
            return true;
        }

        public ZincWatchListJobViewModel GetZincWatchListStatus()
        {

            ZincWatchListJobViewModel obj = new ZincWatchListJobViewModel();
            token = Request.Cookies["Token"];


            obj = _ApiAccess.GetWatchListStatusZinc(ApiURL, token);
            return obj;
        }
       
        [HttpPost]
        public  int UpdateBestBuyForSelectPages(List<ZincWatchlistLogsViewModel> data)
        {
            string token = Request.Cookies["Token"];
            int ID = _ApiAccess.SaveBestBuyUpdateList(ApiURL, token, data);
            return ID;
        }
        public int UpdateBestBuyForAllPages(string ASIN, string available, string jobID)
        {
            string token = Request.Cookies["Token"];
            List<ZincWatchlistLogsViewModel> listmodel = new List<ZincWatchlistLogsViewModel>();
            ZincWatchLogsSearchViewModel searchViewModel = new ZincWatchLogsSearchViewModel();
            searchViewModel.ASIN = ASIN == "undefined" ? "" : ASIN;
            searchViewModel.JobID = jobID == "undefined" ? "" : jobID;
            searchViewModel.Available = available == "undefined" ? "" : available;
            listmodel = _ApiAccess.UpdateBestBuyForAllPages(ApiURL, token, searchViewModel);

            foreach(var item in listmodel)
            {
                item.UnitOriginPrice_Max = (Math.Floor((item.UnitOriginPrice_Max / 100)) + 0.95)*100;
                item.ZincJobId = Convert.ToInt32(jobID);
            }

            int ID = _ApiAccess.SaveBestBuyUpdateList(ApiURL, token, listmodel);
            return ID;
        }

        public int GetWatchlistLogsSelectAllCount(string ASIN, string available, string jobID)
        {
            string token = Request.Cookies["Token"];
            ZincWatchLogsSearchViewModel searchViewModel = new ZincWatchLogsSearchViewModel();
            searchViewModel.ASIN = ASIN == "undefined" ? "" : ASIN;
            searchViewModel.JobID = jobID == "undefined" ? "" : jobID;
            searchViewModel.Available = available == "undefined" ? "" : available;
            int count  = _ApiAccess.GetWatchlistLogsSelectAllCount(ApiURL, token, searchViewModel);
            return count;
        }

        public bool UpdatePriceMax(string ASIN, double MaxPrice)
        {

            bool status = false;
            token = Request.Cookies["Token"];


            status = _ApiAccess.UpdatePriceMax(ApiURL, token, ASIN, MaxPrice);
            return status;
        }
        public IActionResult ZincOrdersLog(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string SC_Order_ID = "", string Amazon_AcName = "", string Zinc_Status = "", string EmptyFirstTime = "")
        {

            token = Request.Cookies["Token"];
            ZincOrdersLogViewModel ZincOrdersLog = new ZincOrdersLogViewModel();

            string CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
            string PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");

            if ("0001-01-01" == orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = "";
                PreviousDate = "";
            }



            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {

            }
            else
            {
                var count = _ApiAccess.GetZincCount(ApiURL, token, CurrentDate, PreviousDate, SC_Order_ID, Amazon_AcName, Zinc_Status);
                ViewBag.TotalCount = count;
                ZincOrdersLog.SC_Order_ID = SC_Order_ID;
                ZincOrdersLog.Amazon_AcName = Amazon_AcName;
                ZincOrdersLog.Zinc_Status = Zinc_Status;

                //return View(BestBuyDropshipQty);
            }
            return View(ZincOrdersLog);
        }
        [HttpPost]
        public IActionResult ZincOrdersLogList(int? page, DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string SC_Order_ID = "", string Amazon_AcName = "", string Zinc_Status = "", string EmptyFirstTime = "")
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
            IPagedList<ZincOrdersLogViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }
            List<ZincOrdersLogViewModel> _viewModel = new List<ZincOrdersLogViewModel>();
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                _viewModel = _ApiAccess.ZincOrdersLogList(ApiURL, token, CurrentDate, PreviousDate, 0, 0, SC_Order_ID, Amazon_AcName, Zinc_Status);
                data = new StaticPagedList<ZincOrdersLogViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                return PartialView("~/Views/ZincWatchlist/ZincOrdersLogPartialView.cshtml", data);
            }

            _viewModel = _ApiAccess.ZincOrdersLogList(ApiURL, token, CurrentDate, PreviousDate, startlimit, endLimit, SC_Order_ID, Amazon_AcName, Zinc_Status);
            data = new StaticPagedList<ZincOrdersLogViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            return PartialView("~/Views/ZincWatchlist/ZincOrdersLogPartialView.cshtml", data);


        }

        public IActionResult ZincWatchlistLogForJob(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string ASIN = "", string ProductSKU = "", string available = "", string jobID = "", string EmptyFirstTime = "")
        {

            token = Request.Cookies["Token"];
            ZincWatchlistLogsForJobViewModel ZincWatchlistLogsForJob = new ZincWatchlistLogsForJobViewModel();

            string CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
            string PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");

            if ("0001-01-01" == orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = "";
                PreviousDate = "";
            }
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {

            }
            else
            {

                var count = _ApiAccess.GetWatchListLogsCountForJob(ApiURL, token, CurrentDate, PreviousDate, ASIN, ProductSKU, available, jobID);
                ViewBag.TotalCount = count;
                ZincWatchlistLogsForJob.ASIN = ASIN;
                ZincWatchlistLogsForJob.ProductSKU = ProductSKU;
            }
            return View(ZincWatchlistLogsForJob);
        }

        [HttpPost]
        public IActionResult ZincWatchlistLogForJobList(int? page, DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string ASIN = "", string ProductSKU = "", string available = "", string jobID = "", string EmptyFirstTime = "")
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
            IPagedList<ZincWatchlistLogsForJobViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }
            List<ZincWatchlistLogsForJobViewModel> _viewModel = new List<ZincWatchlistLogsForJobViewModel>();
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                _viewModel = _ApiAccess.GetWatchListLogsForJob(ApiURL, token, CurrentDate, PreviousDate, 0, 0, ASIN, ProductSKU, available,jobID);
                ViewBag.Compress_image_URL = s3BucketURL;
                ViewBag.image_name_URL = s3BucketURL_large;
                data = new StaticPagedList<ZincWatchlistLogsForJobViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                return PartialView("~/Views/ZincWatchlist/ZinWatchlistLogsForJobPartialView.cshtml", data);
            }

            _viewModel = _ApiAccess.GetWatchListLogsForJob(ApiURL, token, CurrentDate, PreviousDate, startlimit, endLimit, ASIN, ProductSKU, available, jobID);
            ViewBag.Compress_image_URL = s3BucketURL;
            ViewBag.image_name_URL = s3BucketURL_large;
            data = new StaticPagedList<ZincWatchlistLogsForJobViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            return PartialView("~/Views/ZincWatchlist/ZinWatchlistLogsForJobPartialView.cshtml", data);


        }
        //COUNT FUNCTION
        public IActionResult ZincWatchList(DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string ProductSKU = "", string ASIN = "", string Active_Inactive = "", string Enabled_Disabled = "", string EmptyFirstTime = "")
        {

            token = Request.Cookies["Token"];
            ZincWatclistViewModel ZincWatchlist = new ZincWatclistViewModel();

            string CurrentDate = orderDateTimeTo.ToString("yyyy-MM-dd");
            string PreviousDate = orderDateTimeFrom.ToString("yyyy-MM-dd");

            if ("0001-01-01" == orderDateTimeFrom.ToString("yyyy-MM-dd"))
            {

                CurrentDate = "";
                PreviousDate = "";
            }



            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {

            }
            else
            {
                var count = _ApiAccess.GetZincWatchlistCount(ApiURL, token, CurrentDate, PreviousDate, ProductSKU, ASIN, Active_Inactive, Enabled_Disabled);
                ViewBag.TotalCount = count;
                ZincWatchlist.ProductSKU = ProductSKU;
                ZincWatchlist.ASIN = ASIN;

            }
            return View(ZincWatchlist);
        }

        //LIST FUNCTION
        [HttpPost]
        public IActionResult ZincWatchListDetail(int? page, DateTime orderDateTimeFrom, DateTime orderDateTimeTo, string ProductSKU = "", string ASIN = "", string Active_Inactive = "", string Enabled_Disabled = "", string EmptyFirstTime = "", string update_status = "")
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
            IPagedList<ZincWatclistViewModel> data = null;
            int pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 25;
            int endLimit = 0;
            int startlimit = pageSize;
            if (page.HasValue)
            {
                endLimit = (pageNumber - 1) * pageSize;
            }
            List<ZincWatclistViewModel> _viewModel = new List<ZincWatclistViewModel>();
            if ((string.IsNullOrEmpty(EmptyFirstTime) || EmptyFirstTime == "undefined"))
            {
                _viewModel = _ApiAccess.ZincWatchListDetail(ApiURL, token, CurrentDate, PreviousDate, 0, 0, ProductSKU, ASIN, Active_Inactive, Enabled_Disabled);
                ViewBag.Compress_image_URL = s3BucketURL;
                ViewBag.image_name_URL = s3BucketURL_large;
                data = new StaticPagedList<ZincWatclistViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
                return PartialView("~/Views/ZincWatchlist/ZincWatclistPartialView.cshtml", data);
            }

            _viewModel = _ApiAccess.ZincWatchListDetail(ApiURL, token, CurrentDate, PreviousDate, startlimit, endLimit, ProductSKU, ASIN, Active_Inactive, Enabled_Disabled);
            ViewBag.Compress_image_URL = s3BucketURL;
            ViewBag.image_name_URL = s3BucketURL_large;
            data = new StaticPagedList<ZincWatclistViewModel>(_viewModel, pageNumber, pageSize, _viewModel.Count);
            return PartialView("~/Views/ZincWatchlist/ZincWatclistPartialView.cshtml", data);


        }
        [HttpGet]
        public IActionResult logHistory(string ProductSKU, string ASIN)
        {
            token = Request.Cookies["Token"];
            List<ZincWatclistLogHistoryViewModel> modelist = new List<ZincWatclistLogHistoryViewModel>();
            ZincWatclistLogsViewModel model = new ZincWatclistLogsViewModel();
            model = _ApiAccess.logHistory(ApiURL, token, ProductSKU, ASIN);          
            ViewBag.ASIN = model.ASIN;
            ViewBag.ProductSKU = model.ProductSKU;
            ViewBag.ValidStatus = model.ValidStatus;
            ViewBag.NextUpdateDate = model.NextUpdateDate;
            ViewBag.image_name = model.image_name;
            ViewBag.Compress_image = model.Compress_image;           
            ViewBag.Compress_image_URL = s3BucketURL;
            ViewBag.image_name_URL = s3BucketURL_large;
            return View(model);
        }
        
    }
}