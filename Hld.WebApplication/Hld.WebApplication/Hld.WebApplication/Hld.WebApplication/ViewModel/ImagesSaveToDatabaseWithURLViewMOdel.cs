using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ImagesSaveToDatabaseWithURLViewMOdel
    {
        public string product_Sku { get; set; }
        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public bool isImageExistInSC { get; set; }
    }
}
