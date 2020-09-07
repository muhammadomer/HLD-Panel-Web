using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UploadFilesToS3ViewModel
    {
        public string JobType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
