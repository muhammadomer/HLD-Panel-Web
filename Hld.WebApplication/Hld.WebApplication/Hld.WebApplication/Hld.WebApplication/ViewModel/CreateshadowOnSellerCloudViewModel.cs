using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class Metadata
    {
        public string ScheduleDate { get; set; }
        public int CompanyId { get; set; }
    }
  public class CreateshadowOnSellerCloudViewModel
    {
        public Metadata Metadata { get; set; }
        public string FileContents { get; set; }
        public int Format { get; set; }
    }
}
