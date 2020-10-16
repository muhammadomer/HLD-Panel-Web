using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetParentSkuById
    {
        public int product_id { get; set; }
        public int productstatus { get; set; }
        public int ID { get; set; }
        public string Sku { get; set; }
        public string ProductTitle { get; set; }
        public List<GetParentSkuBy> list { get; set; }
    }
    public class GetParentSkuBy
    {
        public int product_id { get; set; }
        public int productstatus { get; set; }
        public int ID { get; set; }
        public string Sku { get; set; }
        public string ProductTitle { get; set; }
    }
}
