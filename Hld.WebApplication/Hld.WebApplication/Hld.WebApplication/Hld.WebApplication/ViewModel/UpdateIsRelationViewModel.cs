﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UpdateIsRelationViewModel
    {
        public string ParentSKU { get; set; }
        public string QueuedJobLink { get; set; }
        public string Status { get; set; }
        public string JobType { get; set; }
        public string FileDirectory { get; set; }
        public string FileName { get; set; }
        public DateTime JobCreationTime { get; set; }
    }
}
