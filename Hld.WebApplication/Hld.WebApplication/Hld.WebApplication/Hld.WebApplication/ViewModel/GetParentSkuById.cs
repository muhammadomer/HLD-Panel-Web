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
        public string ManufactureName { get; set; }
        public string ManufactureModel { get; set; }
        public string DeviceModel { get; set; }
        public string Style { get; set; }
        public string Feature { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public List<GetChildSkuVM> list { get; set; }

    }
    
}
