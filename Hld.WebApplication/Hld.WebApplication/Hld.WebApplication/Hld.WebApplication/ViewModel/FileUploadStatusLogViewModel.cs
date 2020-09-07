using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class FileUploadStatusLogViewModel
    {
        public int FileUploadJobDetail_id { get; set; }
        public int FileUploadJob_id { get; set; }
        public string Sku { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
