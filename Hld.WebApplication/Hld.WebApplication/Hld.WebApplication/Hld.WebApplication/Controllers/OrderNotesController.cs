using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class OrderNotesController : Controller
    {
        private readonly IConfiguration _configuration;
        string ApiURL = "";
        string SCRestURL = "";
        string token = "";


        EncDecChannel _encDecChannel = null;
        GetChannelCredViewModel _Selller = null;
        OrderNotesAPiAccess _OrderApiAccess = null;
        public OrderNotesController(IConfiguration configuration)
        {
            _configuration = configuration;

            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            SCRestURL = _configuration.GetValue<string>("WebApiURL:SCRestURL");
           

            _encDecChannel = new EncDecChannel();
            _OrderApiAccess = new OrderNotesAPiAccess();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public List<GetNotesOrderViewModel> GetSCOrderNotes(string orderID)
        {
            token = Request.Cookies["Token"];
            List<GetNotesOrderViewModel> getNotes =  _OrderApiAccess.GetOrderNotes(ApiURL, token, Convert.ToInt32(orderID));
           
            return getNotes;
        }

        [HttpPost]
        public bool CreateSCOrderNotes(string orderID,string Notes)
        {
           bool status = false;
            token = Request.Cookies["Token"];

            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            CreateNotesViewModel createNotesView = new CreateNotesViewModel();
            createNotesView.Message = Notes;
            createNotesView.Category = "0";
            _OrderApiAccess.CreateOrderNotesFormSC(SCRestURL, authenticate.access_token, Convert.ToInt32(orderID), createNotesView);
            List<CreateOrderNotesViewModel> getNotes = _OrderApiAccess.GetOrderNotesFormSC(SCRestURL, authenticate.access_token, Convert.ToInt32(orderID));
            if (getNotes.Count > 0)
            {
                List<CreateOrderNotesViewModel> data = (List<CreateOrderNotesViewModel>)getNotes.Select(p => p).Where(p => p.NoteID != 0).ToList();
                if (data.Count > 0)
                {
                    status = _OrderApiAccess.saveOrderNotes(ApiURL, token, data);
                    if (status == true)
                    {
                        _OrderApiAccess.SetOrderHavingNotes(ApiURL, token, Convert.ToInt32(orderID));
                    }
                }


            }

            return status;
        }


        public List<CreateOrderNotesViewModel> GetOrderNotesFromSC(string orderID)
        {
            bool status = false;
            token = Request.Cookies["Token"];
            List<CreateOrderNotesViewModel> data = new List<CreateOrderNotesViewModel>();
            _Selller = new GetChannelCredViewModel();
            _Selller = _encDecChannel.DecryptedData(ApiURL, token, "sellercloud");
            AuthenticateSCRestViewModel authenticate = _OrderApiAccess.AuthenticateSCForIMportOrder(_Selller, SCRestURL);
            
            List<CreateOrderNotesViewModel> getNotes = _OrderApiAccess.GetOrderNotesFormSC(SCRestURL, authenticate.access_token, Convert.ToInt32(orderID));
            if (getNotes.Count > 0)
            {
                data = (List<CreateOrderNotesViewModel>)getNotes.Select(p => p).Where(p => p.NoteID != 0).ToList();
                if (data.Count > 0)
                {
                    status = _OrderApiAccess.saveOrderNotes(ApiURL, token, data);
                    if (status == true)
                    {
                        _OrderApiAccess.SetOrderHavingNotes(ApiURL, token, Convert.ToInt32(orderID));
                    }
                }
              

            }

            return data;
        }



    }
}