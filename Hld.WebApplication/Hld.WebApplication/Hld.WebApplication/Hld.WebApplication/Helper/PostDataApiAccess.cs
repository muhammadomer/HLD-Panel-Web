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
    public class PostDataApiAccess
    {
 
    
        public bool CreatePost(string ApiURL, string token, PostDataViewModel ViewModel)
        {
            bool status = false;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/Create/");
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

                throw;
            }
            return status;
        }
        public List<PostDataViewModel> GetEditorDataList(string ApiURL, string token)
        {
            List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
            try
            {



                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/");
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
                listmodel = JsonConvert.DeserializeObject<List<PostDataViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }

        public PostDataViewModel DeleteEditordata(string ApiURL, string token, int id)
        {
            PostDataViewModel ViewModel = new PostDataViewModel();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/Delete?id=" + id);
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
                ViewModel = JsonConvert.DeserializeObject<PostDataViewModel>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }
        public PostDataViewModel EditEditorByid(string ApiURL, string token, int id)
        {
            PostDataViewModel ViewModel = new PostDataViewModel();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/Edit?id=" + id);
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
                ViewModel = JsonConvert.DeserializeObject<PostDataViewModel>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return ViewModel;
        }
        public bool UpdateEditordata(string ApiURL, string token, PostDataViewModel ViewModel)
        {
            bool status = false;
            try
            {


                var data = JsonConvert.SerializeObject(ViewModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/Update/");
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
        public List<PostData> GetCatagory(string ApiURL, string token)
        {
            List<PostData> listmodel = new List<PostData>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/GetCatagory");
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
                listmodel = JsonConvert.DeserializeObject<List<PostData>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }
        public List<PostDataViewModel> GetDataByCatagoryID(string ApiURL, string token,int catagoryid)
        {
            List<PostDataViewModel> listViewModel = new List<PostDataViewModel>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/GetDataByCatagory?catagoryid=" + catagoryid);
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
                listViewModel = JsonConvert.DeserializeObject<List<PostDataViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listViewModel;
        }

        public List<PostDataViewModel> GetDataByCatagoryByTitle(string ApiURL, string token, int catagoryidByTitle)
        {
            List<PostDataViewModel> listViewModel = new List<PostDataViewModel>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/GetDataByCatagoryByTitle?catagoryidByTitle=" + catagoryidByTitle);
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
                listViewModel = JsonConvert.DeserializeObject<List<PostDataViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listViewModel;
        }
        public List<PostDataViewModel> GetDataByCatagoryByTitleSearch(string ApiURL, string token, string Title)
        {
            List<PostDataViewModel> listViewModel = new List<PostDataViewModel>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/Help/GetDataByCatagoryByTitleSearch?title=" + Title);
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
                listViewModel = JsonConvert.DeserializeObject<List<PostDataViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listViewModel;
        }


    }

}
