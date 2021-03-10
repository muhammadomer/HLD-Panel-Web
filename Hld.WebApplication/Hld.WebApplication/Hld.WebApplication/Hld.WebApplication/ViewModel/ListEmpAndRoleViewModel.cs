using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ListEmpAndRoleViewModel
    {
        public ListEmpAndRoleViewModel()
        {
            Employees = new List<EmployeeViewModel>();
            Roles = new List<EmployeeRoleViewModel>();
        }
        public List<EmployeeViewModel> Employees { get; set; }
        public List<EmployeeRoleViewModel> Roles { get; set; }
    }
}
