using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UpdateJobIdForBulkUpdateViewModel
    {
        public string QueuedJobLink { get; set; }
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Sku { get; set; }
        public string S3FilePath { get; set; }
    }
}
