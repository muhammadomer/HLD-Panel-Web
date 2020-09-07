using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class FileStreamModel
    {
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public Byte[] Content { get; set; }
    }
}
