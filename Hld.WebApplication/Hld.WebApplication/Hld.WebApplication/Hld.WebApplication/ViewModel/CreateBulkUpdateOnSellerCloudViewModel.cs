using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CreateBulkUpdateOnSellerCloudViewModel
    {
        public MetadataForBulkUpdate metadataForBulkUpdate { get; set; }
        public string FileContents { get; set; }
        public int Format { get; set; }
    }
    public class MetadataForBulkUpdate
    {
        public string ScheduleDate { get; set; }
        public bool CreateProductIfDoesntExist { get; set; }
        public int CompanyIdForNewProduct { get; set; }
        public bool DoNotUpdateProducts { get; set; }
    }
}
