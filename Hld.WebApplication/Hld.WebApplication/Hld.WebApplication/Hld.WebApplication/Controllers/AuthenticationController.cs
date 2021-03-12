using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using Hld.WebApplication.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Hld.WebApplication.Models;
using MySqlX.XDevAPI;
using System.Linq;
using System.Collections.Generic;

namespace Hld.WebApplication.Controllers
{
    
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        ProductApiAccess ProductApiAccess = null;
        private SignInManager<AppUser> signInManager;
        public AuthenticationController(IConfiguration configuration, UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr, RoleManager<IdentityRole> _roleManager)
        {
            this._configuration = configuration;
            userManager = userMgr;
            roleManager = _roleManager;
               signInManager = signinMgr;
            ProductApiAccess = new ProductApiAccess();
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
       
        [HttpGet]
        //[Route("Authentication/save")]
        public IActionResult Authenticate()
        {
            return View();
        }
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> AuthenticateAlreadyLoginAsync()
        {
            int ischecked = 0;
            if(ischecked > 0)
            {
                //if (ischecked.FirstOrDefault().Checkboxstatus)
                //{
                //    Login login = new Login();

                //    login.Email = ischecked.FirstOrDefault().Email;

                //    string role = await Authenticated(login);

                //    if (role == "Vendor")
                //    {

                //        return RedirectToAction("PurchaseOrders", "PurchaseOrder");
                //    }
                //    if (role == "Receiver")
                //    {

                //        return RedirectToAction("Create", "Shipment");
                //    }
                //    if (role == "Admin")
                //    {

                //        return RedirectToAction("DashBoard", "HLDHistory");
                //    }
                //    else
                //    {
                //        return RedirectToAction("Authenticate", "Authentication");
                //    }
                //}
                //else
                //{
                    return RedirectToAction("Authenticate", "Authentication");
                //}
            }
           
            else
            {
                return RedirectToAction("Authenticate", "Authentication");
            }
           
        }


        [HttpPost]
        //[Route("Authentication/save")]
        public async System.Threading.Tasks.Task<IActionResult> Authenticate(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser appUser = await userManager.FindByEmailAsync(login.Email);
                    if (appUser != null)
                    {
                        await signInManager.SignOutAsync();
                        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
                        if (result.Succeeded)
                        {
                            AuthenciateViewModel authenticateView = new AuthenciateViewModel();
                            authenticateView.UserName = appUser.Email;
                            authenticateView.Password = appUser.Id;
                            var data = JsonConvert.SerializeObject(authenticateView);
                            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Configuration");
                            request.Method = "POST";
                            request.Accept = "application/json;";
                            request.ContentType = "application/json";
                            request.ContentLength = data.Length;
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
                            
                            AuthenciateViewModel responses = JsonConvert.DeserializeObject<AuthenciateViewModel>(strResponse);
                            var cookieOption = new CookieOptions();
                            cookieOption.Expires = DateTime.Now.AddMinutes(600);
                            Response.Cookies.Append("Token", responses.Token, cookieOption);
                            Response.Cookies.Append("UserId", responses.UserId.ToString(), cookieOption);
                            Response.Cookies.Append("UserName", responses.UserName, cookieOption);                          
                            Response.Cookies.Append("expiration", responses.expiration.ToString(), cookieOption);
                            Response.Cookies.Append("UserAlias", appUser.UserAlias.ToString(),cookieOption);
                            if (await userManager.IsInRoleAsync(appUser, "Vendor"))
                            {
                                Response.Cookies.Append("POMasterID", appUser.UserName, cookieOption);
                                return RedirectToAction("PurchaseOrders", "PurchaseOrder");
                            }
                            else if (await userManager.IsInRoleAsync(appUser, "Receiver"))
                            {
                                Response.Cookies.Append("POMasterID", "1278", cookieOption);
                                return RedirectToAction("Create", "Shipment");
                            }
                            else
                            {
                                Response.Cookies.Append("POMasterID", appUser.UserName, cookieOption);
                                return RedirectToAction("Dashboard", "HLDHistory");
                            }
                        
                        }

                    }
                    ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
                }
              
                    return View();
                
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("400"))
                {
                    ViewBag.Message = "ErrorOccured";
                    return View();
                }
                else
                    return View();
            }
        }

        [HttpPost]
        [Route("Authentications/save")]
        public async System.Threading.Tasks.Task<string> Authenticated(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser appUser = await userManager.FindByEmailAsync(login.Email);
                    if (appUser != null)
                    {
                        await signInManager.SignOutAsync();
                        /*Microsoft.AspNetCore.Identity.SignInResult result*/ await signInManager.SignInAsync(appUser,false,null);
                        
                        
                            AuthenciateViewModel authenticateView = new AuthenciateViewModel();
                            authenticateView.UserName = appUser.Email;
                            //authenticateView.Password = appUser.Id;
                            var data = JsonConvert.SerializeObject(authenticateView);
                            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Configurations");
                            request.Method = "POST";
                            request.Accept = "application/json;";
                            request.ContentType = "application/json";
                            request.ContentLength = data.Length;
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

                            AuthenciateViewModel responses = JsonConvert.DeserializeObject<AuthenciateViewModel>(strResponse);
                            var cookieOption = new CookieOptions();
                            cookieOption.Expires = DateTime.Now.AddMinutes(600);
                            Response.Cookies.Append("Token", responses.Token, cookieOption);
                            Response.Cookies.Append("UserId", responses.UserId.ToString(), cookieOption);
                            Response.Cookies.Append("UserName", responses.UserName, cookieOption);
                            Response.Cookies.Append("expiration", responses.expiration.ToString(), cookieOption);
                            Response.Cookies.Append("UserAlias", appUser.UserAlias.ToString(), cookieOption);
                            if (await userManager.IsInRoleAsync(appUser, "Vendor"))
                            {
                                Response.Cookies.Append("POMasterID", appUser.UserName, cookieOption);
                                return "Vendor";
                            }
                            else if (await userManager.IsInRoleAsync(appUser, "Receiver"))
                            {
                                Response.Cookies.Append("POMasterID", "1278", cookieOption);
                                return "Receiver";
                            }
                            else
                            {
                                Response.Cookies.Append("POMasterID", appUser.UserName, cookieOption);
                                return "Admin";
                            }
                        
                    }
                    ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
                }

                return "";

            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("400"))
                {
                    ViewBag.Message = "ErrorOccured";
                    return "";
                }
                else
                    return "";
            }
        }
        public IActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                
                if (cookie == "Token" || cookie == "POMasterID" || cookie == "UserName" || cookie== "UserAlias")
                {
                    Response.Cookies.Delete(cookie);
                    
                }
                
            }

            return RedirectToAction("Authenticate", "Authentication");

        }
        [HttpPut]
        public bool SaveCheckboxstatus(string Email, bool Checkboxstatus)
        {

            string token = Request.Cookies["Token"];
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            ProductApiAccess.SaveCheckboxstatus(ApiURL, Email, Checkboxstatus);

            return true;
        }
        [HttpGet]
        public List<Login> GetCheckboxstatus()
        {
            string token = Request.Cookies["Token"];
            string ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            List<Login> listmodel = new List<Login>();
            listmodel = ProductApiAccess.GetCheckboxstatus(ApiURL);
            return listmodel;
        }
    }
}