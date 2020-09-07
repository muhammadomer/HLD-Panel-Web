using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Hld.WebApplication.Controllers
{
    public class HelpController : Controller
    {
        private readonly IConfiguration _configuration;
        PostDataApiAccess _ApiAccess = new PostDataApiAccess();

        string ApiURL = null;

        public HelpController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
        }
        public IActionResult Index()
        {
            string token = Request.Cookies["Token"];
            PostDataViewModel model = new PostDataViewModel();
            List<PostData> listmodel = new List<PostData>();
            listmodel = _ApiAccess.GetCatagory(ApiURL, token);

            //PostData obj = new PostData();
            //obj.catagoryid = 0;
            //obj.catagory = "select catagory";
            //listmodel.Add(obj);
            //listmodel.OrderBy(s => s.catagoryid);
            model.listdatacatagory = listmodel;
            return View(model);
            //string token = Request.Cookies["Token"];
            //PostDataViewModel obj = new PostDataViewModel();
            //List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
            //listmodel = _ApiAccess.GetEditorDataList(ApiURL, token);
            //obj.listdata = listmodel;


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PostDataViewModel model)
        {

            string token = Request.Cookies["Token"];

            try
            {
                if (model.idPostEditor > 0)
                {

                    _ApiAccess.UpdateEditordata(ApiURL, token, model);
                    //List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
                    //listmodel = _ApiAccess.GetEditorDataList(ApiURL, token);
                    //model.listdata = listmodel;
                    //return PartialView("~/Views/Help/PartailListView.cshtml", listmodel);
                    return RedirectToAction("Index", "Help");
                }
                else
                {
                    _ApiAccess.CreatePost(ApiURL, token, model);
                    //List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
                    //listmodel = _ApiAccess.GetEditorDataList(ApiURL, token);
                    //model.listdata = listmodel;
                    //return PartialView("~/Views/Help/PartailListView.cshtml", listmodel);
                    return RedirectToAction("Index", "Help");
                }


            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpGet]
        public ActionResult GetEditorDataList()
        {

            string token = Request.Cookies["Token"];
            List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
            listmodel = _ApiAccess.GetEditorDataList(ApiURL, token);
            return PartialView("~/Views/Help/PartialViewdatalist.cshtml", listmodel);
            //return View(listmodel);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            string token = Request.Cookies["Token"];

            PostDataViewModel model = new PostDataViewModel();
            model = _ApiAccess.DeleteEditordata(ApiURL, token, id);
            return RedirectToAction("Index");

        }

        public ActionResult Edit(int id)
        {

            string token = Request.Cookies["Token"];
            PostDataViewModel ViewModel = new PostDataViewModel();
            ViewModel = _ApiAccess.EditEditorByid(ApiURL, token, id);
            var listmodel = _ApiAccess.GetCatagory(ApiURL, token);
            ViewModel.listdatacatagory = listmodel;
            return View("Index", ViewModel);

        }
        //[HttpPost]
        //public ActionResult Edit(PostDataViewModel model)
        //{

        //    string token = Request.Cookies["Token"];

        //   _ApiAccess.EditEditorByid(ApiURL, token, model);

        //    return View("Index", model);

        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {


                return RedirectToAction(nameof(GetEditorDataList));
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult GetDataByCatagory()
        {
            string token = Request.Cookies["Token"];
            List<PostData> listmodel = new List<PostData>();
            listmodel = _ApiAccess.GetCatagory(ApiURL, token);

            return PartialView("~/Views/Help/GetDataById.cshtml", listmodel);
        }
        [HttpGet]
        public ActionResult GetDataByCatagoryID(int catagoryid)
        {
            string token = Request.Cookies["Token"];
            List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
            listmodel = _ApiAccess.GetDataByCatagoryID(ApiURL, token, catagoryid);
            return PartialView("~/Views/Help/GetDataByCatagoryID.cshtml", listmodel);
        }
        [HttpGet]
        public List<PostDataViewModel> GetDataByCatagoryByTitle(int catagoryidByTitle)
        {
            string token = Request.Cookies["Token"];
            List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
            listmodel = _ApiAccess.GetDataByCatagoryByTitle(ApiURL, token, catagoryidByTitle);
            return listmodel;
        }

        [HttpGet]
        public IActionResult GetDataByCatagoryByTitleSearch(string Title)
        {
            string token = Request.Cookies["Token"];
            List<PostDataViewModel> listmodel = new List<PostDataViewModel>();
            listmodel = _ApiAccess.GetDataByCatagoryByTitleSearch(ApiURL, token, Title);
            return PartialView("~/Views/Help/GetDataByCatagoryID.cshtml", listmodel);

        }
    }
}