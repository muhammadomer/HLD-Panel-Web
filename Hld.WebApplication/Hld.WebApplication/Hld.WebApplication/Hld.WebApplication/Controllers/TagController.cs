using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Hld.WebApplication.Controllers
{
    [TokenExpires]
    public class TagController : Controller
    {

        private readonly IConfiguration _configuration;
        TagApiAccess tagApiAccess = new TagApiAccess();

        string ApiURL = null;

        public TagController(IConfiguration configuration)
        {
            this._configuration = configuration;
            ApiURL =  _configuration.GetValue<string>("WebApiURL:URL");
        }
        // GET: Tag
        public ActionResult TagList()
        {
          
            string token = Request.Cookies["Token"];
            List<TagViewModel> listmodel = new List<TagViewModel>();
            listmodel = tagApiAccess.GetTag(ApiURL, token);
            return View(listmodel);
        }


        public List<TagViewModel> TagListForAssign()
        {

            string token = Request.Cookies["Token"];
            List<TagViewModel> listmodel = new List<TagViewModel>();
            listmodel = tagApiAccess.GetTag(ApiURL, token);
            return listmodel;
        }

        // GET: Tag/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tag/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tag/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TagViewModel model)
        {
            if (ModelState.IsValid)
            {
              
                try
                {
                  
                    string token = Request.Cookies["Token"];
                    
                    tagApiAccess.TagCreate(ApiURL, token, model);
                    
                    return RedirectToAction("TagList", "Tag");
                }
                catch
                {
                    return View();
                }
            }
            else
                return View(model);
        }

        // GET: Tag/Edit/5
        public ActionResult Edit(int id)
        {

            string token = Request.Cookies["Token"];
            TagViewModel tagViewModel = new TagViewModel();
            tagViewModel = tagApiAccess.GetTagByid(ApiURL, token, id);

            return View("Create",tagViewModel);

        }

        // POST: Tag/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(TagList));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tag/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        public bool AssignTagsToProduct(List<string> sku, List<int> Tagids)
        {
            bool success = false;
            List<AssignTagViewModel> viewModel = new List<AssignTagViewModel>();
            List<TagsList> list = new List<TagsList>();
            foreach (var item in Tagids)
            {
                list.Add(new TagsList{
                    TagId =item
                });
            }
           
            foreach (var item in sku)
            {
                viewModel.Add(new AssignTagViewModel
                {
                    SKu = item,
                    tags = list
                });
            }
          
            string token = Request.Cookies["Token"];

            success = tagApiAccess.AssignTags(ApiURL, token, viewModel);
           
            return success;
        }

        public List<SkuTagOrderViewModel> GetTagforsku(string sku)
        {
            string token = Request.Cookies["Token"];
            List<SkuTagOrderViewModel> tagViewModel = new List<SkuTagOrderViewModel>();
            tagViewModel = tagApiAccess.GetTagBySku(ApiURL, token, sku);
            return tagViewModel;


        }
        public bool RemoveTagsToProduct(string sku, List<int> Tagids)
        {
            bool success = false;
           AssignTagViewModel viewModel = new AssignTagViewModel();
            List<TagsList> list = new List<TagsList>();
            foreach (var item in Tagids)
            {
                list.Add(new TagsList
                {
                    TagId = item
                });
            }
            viewModel.SKu = sku;
            viewModel.tags = list;
          

            string token = Request.Cookies["Token"];

            success = tagApiAccess.RemoveTag(ApiURL, token, viewModel);

            return success;
        }


    }
}