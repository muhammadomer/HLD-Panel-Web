using DataAccess.ViewModels;
using Hld.WebApplication.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class ZincProxyApiAccess
    {
        public bool SaveZincProxyDetail(string ApiURL, string token, ZincProxyViewModel ViewModel)
        {
            bool status = false;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy");
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

        public List<GetZincProxyViewModel> ZincProxyList(string ApiURL, string token)
        {
            List<GetZincProxyViewModel> listmodel = new List<GetZincProxyViewModel>();
            try
            {



                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy");
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
                listmodel = JsonConvert.DeserializeObject<List<GetZincProxyViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }
        public int IsActiveZincProxy(string ApiURL, string token, ProxySettingViewModal ViewModel)
        {
            int status = 0;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/SetActive");
                request.Method = "PUT";
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
                status = JsonConvert.DeserializeObject<int>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }

        public int IsDefaultZincPorxy(string ApiURL, string token, ProxySettingViewModal ViewModel)
        {
            int status = 0;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/SetDefault");
                request.Method = "PUT";
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
                status = JsonConvert.DeserializeObject<int>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return status;
        }

        public GetZincProxyViewModel ZincProxyForZinc(string ApiURL, string token,string email)
        {
            GetZincProxyViewModel listmodel = new GetZincProxyViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/GetForZinc?email=" + email);
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
                listmodel = JsonConvert.DeserializeObject<GetZincProxyViewModel>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }

        public bool SaveZincEmail(string ApiURL, string token, SaveZincProxyEmailViewModel ViewModel)
        {
            bool status = false;
            try
            {
                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/SaveProxyEmail");
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

        public List<SaveZincProxyEmailViewModel> GetZincProxyEmail(string ApiURL, string token)
        {
            List<SaveZincProxyEmailViewModel> listmodel = new List<SaveZincProxyEmailViewModel>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/GetZincProxyEmail");
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
                listmodel = JsonConvert.DeserializeObject<List<SaveZincProxyEmailViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }

        public SaveZincProxyEmailViewModel DeleteByid(string ApiURL, string token, int id)
        {
            SaveZincProxyEmailViewModel ViewModel = new SaveZincProxyEmailViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/Delete/" + id);
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
                ViewModel = JsonConvert.DeserializeObject<SaveZincProxyEmailViewModel>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }

        public SaveZincProxyEmailViewModel ProxyDeleteByid(string ApiURL, string token, int id)
        {
            SaveZincProxyEmailViewModel ViewModel = new SaveZincProxyEmailViewModel();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/ZincProxy/ProxyDelete/" + id);
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
                ViewModel = JsonConvert.DeserializeObject<SaveZincProxyEmailViewModel>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }
    }
}
