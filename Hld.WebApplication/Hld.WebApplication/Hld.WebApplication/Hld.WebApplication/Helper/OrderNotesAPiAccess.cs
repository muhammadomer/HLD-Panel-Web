using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class OrderNotesAPiAccess
    {
        public List<GetNotesOrderViewModel> GetOrderNotes(string ApiURL, string token, int id)
        {
            List<GetNotesOrderViewModel> listmodel = new List<GetNotesOrderViewModel>();
            try
            {



                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/OrderNotes/"+id);
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
                listmodel = JsonConvert.DeserializeObject<List<GetNotesOrderViewModel>>(strResponse);

            }
            catch (Exception)
            {

                throw;
            }
            return listmodel;
        }


        public bool SetOrderHavingNotes(string ApiURL, string token, int id)
        {
            try
            {
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/OrderNotes/having/" + id);
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
                return Convert.ToBoolean(strResponse);

            }
            catch (Exception)
            {

                return false;
            }
           
        }

        public AuthenticateSCRestViewModel AuthenticateSCForIMportOrder(GetChannelCredViewModel _getChannelCredViewModel, string ApiURL)
        {
            AuthenticateSCRestViewModel responses = new AuthenticateSCRestViewModel();
            try
            {


                RestSCCredViewModel Data = new RestSCCredViewModel();
                Data.Username = _getChannelCredViewModel.UserName;
                Data.Password = _getChannelCredViewModel.Key;

                var data = JsonConvert.SerializeObject(Data);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/token");
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

                responses = JsonConvert.DeserializeObject<AuthenticateSCRestViewModel>(strResponse);

            }
            catch (WebException ex)
            {
                return responses;
            }
            return responses;
        }

        public List<CreateOrderNotesViewModel> GetOrderNotesFormSC(string ApiURL,string token ,int id)
        {
            List<CreateOrderNotesViewModel> listmodel = new List<CreateOrderNotesViewModel>();
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/Orders/Notes?id=" + id);
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
                listmodel = JsonConvert.DeserializeObject<List<CreateOrderNotesViewModel>>(strResponse);
               

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return listmodel;
        }

        public bool saveOrderNotes(string ApiURL, string token, List<CreateOrderNotesViewModel> createonlocal)
        {
            List<GetNotesOrderViewModel> listmodel = new List<GetNotesOrderViewModel>();
            try
            {

                var data = JsonConvert.SerializeObject(createonlocal);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/api/OrderNotes");
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
                return Convert.ToBoolean(strResponse);

            }
            catch (WebException ex)
            {
                return false;
            }
           
        }

        public List<GetNotesOrderViewModel> CreateOrderNotesFormSC(string ApiURL, string token, int id, CreateNotesViewModel createNotesView)
        {
            List<GetNotesOrderViewModel> listmodel = new List<GetNotesOrderViewModel>();
            try
            {
                

                var data = JsonConvert.SerializeObject(createNotesView);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/Orders/"+id+"/Notes");
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

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return listmodel;
        }




    }
}
