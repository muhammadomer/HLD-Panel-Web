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
            int pageSize = 25;


            offset = (pageNumber - 1) * 25;
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
            int ID = _ApiAccess.SaveBestBuyUpdateList(ApiURL, token, listmodel);
            return ID;
        }
        public bool UpdatePriceMax(string ASIN, double MaxPrice)
        {

            bool status = false;
            token = Request.Cookies["Token"];


            status = _ApiAccess.UpdatePriceMax(ApiURL, token, ASIN, MaxPrice);
            return status;
        }


    }
}