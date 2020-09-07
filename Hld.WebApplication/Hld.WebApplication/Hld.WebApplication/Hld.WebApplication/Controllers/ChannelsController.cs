using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Auth.AccessControlPolicy;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    [Authorize(Policy = "Access to Setting Tab")]
    public class ChannelsController : Controller
    {
        EncDecChannel encDecChannel = new EncDecChannel();

        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string token = "";
        ChannelsApiAccess channelsApiAccess = null;
        GetChannelCredViewModel _Zinc = null;
        EncDecChannel _encDecChannel = null;
        public ChannelsController(IConfiguration configuration)
        {


            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");

            channelsApiAccess = new ChannelsApiAccess();
            _encDecChannel = new EncDecChannel();
        }



        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SellerCloud()
        {
            return View();
        }
        public IActionResult ZincKey()
        {
            return View();
        }
        public IActionResult AmazonZinc()
        {
            return View();
        }
        public IActionResult BestBuyCAKeyvalue()
        {
            return View();
        }
        public bool UpdateAmazonZincDays(string Days)
        {
            try
            {
                bool status = false;
                UpdateChannelsViewModel model = new UpdateChannelsViewModel();
                model.Method = "ZincDays";

                model.password = Days.Trim();

                string token = Request.Cookies["Token"];
                status = channelsApiAccess.UpdateChannelCred(ApiURL, token, model);

                return status;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult UpdateAmazonZincCredential(AmazonZincViewModel amazonZinc)
        {
            try
            {
                bool status = false;
                UpdateChannelsViewModel model = new UpdateChannelsViewModel();
                model.Method = "AmzZinc";
                model.Key = encDecChannel.EncryptStringToBytes_Aes(amazonZinc.FAKey.Trim());
                model.UserName = encDecChannel.EncryptStringToBytes_Aes(amazonZinc.UserName.Trim());
                model.password = encDecChannel.EncryptStringToBytes_Aes(amazonZinc.Password.Trim());
                model.passwordShort = amazonZinc.Password.Trim().Substring(amazonZinc.Password.Length - 4);
                model.KeyShort = amazonZinc.FAKey.Trim().Substring(amazonZinc.FAKey.Length - 4);
                model.UserNameShort = amazonZinc.UserName.Trim().Substring(0, 4);

                string token = Request.Cookies["Token"];
                status = channelsApiAccess.UpdateChannelCred(ApiURL, token, model);



                return RedirectToAction("ViewAmazonZincLogs", "Channels");
            }
            catch (Exception)
            {

                throw;
            }


        }
        //for update views
        public IActionResult UpdateSellerCredential(ChannelsViewModel seller)
        {
            try
            {
                bool status = false;
                UpdateChannelsViewModel model = new UpdateChannelsViewModel();
                model.Method = "sellercloud";
                model.Key = encDecChannel.EncryptStringToBytes_Aes(seller.Password.Trim());
                model.UserName = encDecChannel.EncryptStringToBytes_Aes(seller.UserName.Trim());
                model.KeyShort = seller.Password.Trim().Substring(seller.Password.Length - 4);
                model.UserNameShort = seller.UserName.Trim().Substring(0, 4);

                string token = Request.Cookies["Token"];
                status = channelsApiAccess.UpdateChannelCred(ApiURL, token, model);

                return RedirectToAction("ViewSellerLogs", "Channels");
            }
            catch (Exception)
            {

                throw;
            }


        }

        public IActionResult UpdateZincCredential(ZincKeyModel zinc)
        {
            try
            {
                UpdateChannelsViewModel model = new UpdateChannelsViewModel();
                bool status = false;
                model.Method = "Zinc";
                model.Key = encDecChannel.EncryptStringToBytes_Aes(zinc.ZincKeyValue.Trim());
                model.KeyShort = zinc.ZincKeyValue.Trim().Substring(zinc.ZincKeyValue.Length - 4);
                string token = Request.Cookies["Token"];
                status = channelsApiAccess.UpdateChannelCred(ApiURL, token, model);

                return RedirectToAction("ViewZincLogs", "Channels");
            }
            catch (Exception)
            {

                throw;
            }


        }

        public IActionResult UpdateBestBuyCredential(BestBuyCAviewModel best)
        {
            try
            {
                bool status = false;
                UpdateChannelsViewModel model = new UpdateChannelsViewModel();
                model.Method = "bestbuy";

                model.Key = encDecChannel.EncryptStringToBytes_Aes(best.BestBuyCAKeyValue.Trim());

                model.KeyShort = best.BestBuyCAKeyValue.Substring(best.BestBuyCAKeyValue.Trim().Length - 4);

                string token = Request.Cookies["Token"];
                status = channelsApiAccess.UpdateChannelCred(ApiURL, token, model);

                return RedirectToAction("ViewBestBuyLogs", "Channels");
            }
            catch (Exception)
            {

                throw;
            }

        }
        //for logs views
        public IActionResult ViewSellerLogs()
        {
            string token = Request.Cookies["Token"];
            List<ChannelLogs> channelLogs = new List<ChannelLogs>();
            channelLogs = channelsApiAccess.GetLogsOfCred(ApiURL, token, "sellercloud");
            return View(channelLogs);

        }

        public string GetZincMaxDays()
        {
            string token = Request.Cookies["Token"];
            GetChannelCredViewModel channelLogs = new GetChannelCredViewModel();
            channelLogs = channelsApiAccess.GetCred(ApiURL, token, "ZincDays");
            return channelLogs.password;

        }
        public IActionResult ViewAmazonZincLogs()
        {
            string token = Request.Cookies["Token"];
            List<ChannelLogs> channelLogs = new List<ChannelLogs>();
            channelLogs = channelsApiAccess.GetLogsOfCred(ApiURL, token, "AmzZinc");


            return View(channelLogs);

        }
        public IActionResult ViewZincLogs()
        {
            string token = Request.Cookies["Token"];
            List<ChannelLogs> channelLogs = new List<ChannelLogs>();
            channelLogs = channelsApiAccess.GetLogsOfCred(ApiURL, token, "Zinc");
            return View(channelLogs);
        }
        public IActionResult ViewBestBuyLogs()
        {
            string token = Request.Cookies["Token"];
            List<ChannelLogs> channelLogs = new List<ChannelLogs>();
            channelLogs = channelsApiAccess.GetLogsOfCred(ApiURL, token, "bestbuy");
            return View(channelLogs);
        }

        public IActionResult ZincMaxDays()
        {
            return View();
        }

    }
}