using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetJobDetailViewModel
    {
        public int Job_Id { get; set; }
        public string Job_Type { get; set; }
        public string File_Name { get; set; }
        public int Running { get; set; }
        public int Status { get; set; }
        public string Job_Start { get; set; }
        public string Job_Completed { get; set; }
        public string File_Bucket { get; set; }
    }
}
