using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetChildSkuShadowsViewModel
    {
        public string FileContents { get; set; }
        public int CompanyId { get; set; }
        public int Format { get; set; }
        public Metadata Metadata { get; set; }
    }
    public class FileContents
    {
        public string ParentSKU { get; set; }
        public string ShadowSKU { get; set; }
        public int CompanyID { get; set; }
    }
}
