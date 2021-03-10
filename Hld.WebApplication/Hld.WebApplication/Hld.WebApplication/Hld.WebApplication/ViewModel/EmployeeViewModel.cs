using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeRole { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeRoleName { get; set; }
    }
}
