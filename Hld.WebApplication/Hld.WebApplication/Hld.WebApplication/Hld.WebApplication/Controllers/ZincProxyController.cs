using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using DataAccess.ViewModels;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class ZincProxyController : Controller
    {
        private readonly IConfiguration _configuration;
        ZincProxyApiAccess _zincApiAccess = new ZincProxyApiAccess();
        EncDecChannel encDecChannel = new EncDecChannel();
        string ApiURL = null;

        public ZincProxyController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
        }
        public IActionResult Index()
        {
            string token = Request.Cookies["Token"];
            List<SaveZincProxyEmailViewModel> listmodel = new List<SaveZincProxyEmailViewModel>();
            listmodel = _zincApiAccess.GetZincProxyEmail(ApiURL, token);
            ZincProxyViewModel viewmodel = new ZincProxyViewModel();
            viewmodel.List = listmodel;
            return View(viewmodel);
        }
        [HttpPost]
        public IActionResult Index(ZincProxyViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string token = Request.Cookies["Token"];
                    viewmodel.ProxyPasswordShort = viewmodel.ProxyPassword.Trim().Substring(viewmodel.ProxyPassword.Length - 4);
                    viewmodel.ProxyPassword = encDecChannel.EncryptStringToBytes_Aes(viewmodel.ProxyPassword.Trim());
                    _zincApiAccess.SaveZincProxyDetail(ApiURL, token, viewmodel);
                    return RedirectToAction("ZincProxyDetail", "ZincProxy");
                }
                catch (Exception ex)
                {
                    return View();
                }

            }
            else
                return View(viewmodel);

        }

        public IActionResult ZincProxyDetail()
        {
            string token = Request.Cookies["Token"];
            List<GetZincProxyViewModel> listmodel = new List<GetZincProxyViewModel>();
            listmodel = _zincApiAccess.ZincProxyList(ApiURL, token);
            return View(listmodel);
        }

        public int IsActive(int id, bool IsActive)
        {
            string token = Request.Cookies["Token"];
            ProxySettingViewModal Viewmodel = new ProxySettingViewModal();
            Viewmodel.Isactive = IsActive;
            Viewmodel.idZincProxy = id;
            _zincApiAccess.IsActiveZincProxy(ApiURL, token, Viewmodel);
            return 0;
        }

        public int IsDefault(int id, bool IsDefault)
        {
            string token = Request.Cookies["Token"];
            ProxySettingViewModal Viewmodel = new ProxySettingViewModal();
            Viewmodel.IsDefault = IsDefault;
            Viewmodel.idZincProxy = id;
            _zincApiAccess.IsDefaultZincPorxy(ApiURL, token, Viewmodel);
            return 0;
        }

        public ProxeyResponseViewModel CheckProxy()
        {
            ProxeyResponseViewModel item = new ProxeyResponseViewModel();
            int statusCode = 0;
            ZincProxySendViewModal proxySendViewModal = new ZincProxySendViewModal();
            string token = Request.Cookies["Token"];
            var ProxyDetail = _zincApiAccess.ZincProxyForZinc(ApiURL, token);
            //proxySendViewModal.email = "am_b1@homelivingdream.com";
            proxySendViewModal.email = ProxyDetail.ProxyEmail;
            proxySendViewModal.retailer = "amazon_ca";
            proxySendViewModal.proxy_url = "http://" + ProxyDetail.ProxyUser + ":" + encDecChannel.DecryptStringFromBytes_Aes(ProxyDetail.ProxyPassword) + "@" + ProxyDetail.ProxyIP + ":" + ProxyDetail.ProxyPort;
            GetChannelCredViewModel _Zinc = new GetChannelCredViewModel();
            _Zinc = encDecChannel.DecryptedData(ApiURL, token, "Zinc");
            string ZincUserName = _Zinc.Key;

            try
            {
                var data = JsonConvert.SerializeObject(proxySendViewModal);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.zinc.io/v1/proxies/byop");
                request.Method = "PUT";
                request.Accept = "application/json;";
                request.ContentType = "application/json";
                request.Credentials = new NetworkCredential(ZincUserName, "");
                string response = "";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                using (var webResponse = request.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        response = new StreamReader(responseStream).ReadToEnd();
                    }
                }
                //var X = JObject.Parse(response);
                item = JsonConvert.DeserializeObject<ProxeyResponseViewModel>(response);
                item.auth_password = item.auth_password.Trim().Substring(item.auth_password.Length - 4);
                statusCode = 1;
            }
            catch (Exception)
            {
                throw;
            }
            return item;
        }

        public async Task<ProxeyResponseViewModel> GetResponse()
        {
            ProxeyResponseViewModel item = new ProxeyResponseViewModel();

            string token = Request.Cookies["Token"];

            var ProxyDetail = _zincApiAccess.ZincProxyForZinc(ApiURL, token);

            GetChannelCredViewModel _Zinc = new GetChannelCredViewModel();
            _Zinc = encDecChannel.DecryptedData(ApiURL, token, "Zinc");
            string ZincUserName = _Zinc.Key;

            ZincProxyGetViewModal Obj = new ZincProxyGetViewModal();
            Obj.email = ProxyDetail.ProxyEmail;
            Obj.retailer = "amazon_ca";
            try
            {
                var data = JsonConvert.SerializeObject(Obj);
                HttpClient client = new HttpClient();
                var byteArray = Encoding.ASCII.GetBytes($"{ZincUserName}:{""}");
                //client.DefaultRequestHeaders.Add("Authorization", "Basic " + ZincUserName);

                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ZincUserName));
                //var _CredentialBase64 = "RWRnYXJTY2huaXR0ZW5maXR0aWNoOlJvY2taeno=";
                //client.DefaultRequestHeaders.Add("Authorization", String.Format("Basic {0}", byteArray));
                var _UserAgent = "d-fens HttpClient";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ZincUserName);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.zinc.io/v1/proxies/byop"),
                    Content = new StringContent(data, Encoding.UTF8, "application/json"),
                };

                var response = await client.SendAsync(request).ConfigureAwait(false);
                var resbody = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                item = JsonConvert.DeserializeObject<ProxeyResponseViewModel>(resbody);
                item.auth_password = item.auth_password.Trim().Substring(item.auth_password.Length - 4);


            }
            catch (Exception exp)
            {

                throw;
            }
            return item;
        }

        [HttpGet]
        public IActionResult SaveZincEmail()
        {
            return View();
        }


        public IActionResult SaveZincEmail(SaveZincProxyEmailViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string token = Request.Cookies["Token"];

                    _zincApiAccess.SaveZincEmail(ApiURL, token, viewmodel);
                    return RedirectToAction("Index", "ZincProxy");
                }
                catch (Exception ex)
                {
                    return View();
                }

            }
            else
                return View(viewmodel);

        }


        public IActionResult GetZincProxyEmail()
        {
            string token = Request.Cookies["Token"];
            List<SaveZincProxyEmailViewModel> listmodel = new List<SaveZincProxyEmailViewModel>();
            listmodel = _zincApiAccess.GetZincProxyEmail(ApiURL, token);
            return View(listmodel);
        }


        public ActionResult Delete(int id)
        {

            string token = Request.Cookies["Token"];
            SaveZincProxyEmailViewModel ViewModel = new SaveZincProxyEmailViewModel();
            ViewModel = _zincApiAccess.DeleteByid(ApiURL, token, id);
            return RedirectToAction("GetZincProxyEmail", "ZincProxy");
        }

        public ActionResult ProxyDelete(int id)
        {
            string token = Request.Cookies["Token"];
            SaveZincProxyEmailViewModel ViewModel = new SaveZincProxyEmailViewModel();
            ViewModel = _zincApiAccess.ProxyDeleteByid(ApiURL, token, id);
            return RedirectToAction("ZincProxyDetail", "ZincProxy");
        }

    }
}