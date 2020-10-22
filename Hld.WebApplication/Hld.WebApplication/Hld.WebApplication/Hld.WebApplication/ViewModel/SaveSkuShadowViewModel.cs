using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    //public class SaveSkuShadowViewModel
    //{
    //    public SaveSkuShadowViewModel()
    //    {
    //        list = new List<SaveChildShadowViewModel>();
    //    }
    //    public int ParentId { get; set; }
    //    public List<SaveChildShadowViewModel> list { get; set; }
    //}
    //public class SaveChildShadowViewModel
    //{
    //    public string Sku { get; set; }
    //    public int ChildId { get; set; }
    //}
    public class SaveSkuShadowViewModel
    {
        public int ParentId { get; set; }
        public string Sku { get; set; }
       public int ChildId { get; set; }
    }
}
