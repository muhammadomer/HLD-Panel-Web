using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetDataForBulkUpdateViewModel
    {
        public int JobId { get; set; }
        public string JobType { get; set; }
        public string File { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompletionTime { get; set; }
    }
}
