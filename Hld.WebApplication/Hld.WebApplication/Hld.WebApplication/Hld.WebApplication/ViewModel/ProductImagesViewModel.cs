using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductImagesViewModel
    {
        public int ProductImageId { get; set; }
        public byte[] Image { get; set; }
        public string ImageURL { get; set; }
        public string ProductId { get; set; }
        
    }
}
