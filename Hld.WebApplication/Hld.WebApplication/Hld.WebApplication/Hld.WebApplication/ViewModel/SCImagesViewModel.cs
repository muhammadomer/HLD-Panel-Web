using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SCImage
    {
        public int ImageID { get; set; }
        public string Url { get; set; }
        public bool IsDefault { get; set; }
        public bool IsMainDescriptionImage { get; set; }
        public string OriginalImageName { get; set; }
    }

    public class SCImagesViewModel
    {
        public List<SCImage> MyArray { get; set; }
    }


}
