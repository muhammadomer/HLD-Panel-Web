using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Views.Product
{
    public class ProductManufactureListViewModel
    {
        public int ManufactureId { get; set; }
        public int ParentID { get; set; }

        public string ManufactureName { get; set; }
        public string ManufactureModel { get; set; }

        public string DeviceModel { get; set; }
    }
}
