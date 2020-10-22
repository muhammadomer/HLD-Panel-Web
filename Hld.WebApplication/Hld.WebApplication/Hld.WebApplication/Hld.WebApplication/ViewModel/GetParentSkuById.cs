using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetParentSkuById
    {
        public GetParentSkuById()
        {
            list = new List<GetChildSkuVM>();
        }
        public int Parentproduct_id { get; set; }
        public int productstatus { get; set; }
        public int ColorIds { get; set; }
        public int ID { get; set; }
        public string Sku { get; set; }
        public string ProductTitle { get; set; }
        public List<GetChildSkuVM> list { get; set; }
    }
    
}
