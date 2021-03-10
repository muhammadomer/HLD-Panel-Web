using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hld.WebApplication.Helper;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Hld.WebApplication.Controllers
{

    [TokenExpires]
    public class ManageEmployee : Controller
    {
        private readonly IConfiguration _configuration = null;
        EmployeeApiAccess _employeeApiAccess = new EmployeeApiAccess();
        EmployeeRoleApiAccess _employeeRoleApiAccess = new EmployeeRoleApiAccess();
        string ApiURL = "";
        string token = "";
        string connectionString = "";
        HLDSalesApiAccess _hldApiAccess = null;
        ProductApiAccess _productApiAccess = null;
        BBProductApiAccess _bBProductApiAccess = null;
        ListEmpAndRoleViewModel listEmpAndRoleViewModel = null;
        public ManageEmployee(IConfiguration configuration)
        {
            
            this._configuration = configuration;
            connectionString = _configuration.GetValue<string>("ConnectionString:bb2HldNet");
            ApiURL = _configuration.GetValue<string>("WebApiURL:URL");
            _hldApiAccess = new HLDSalesApiAccess();
            _productApiAccess = new ProductApiAccess();
            _bBProductApiAccess = new BBProductApiAccess();
            listEmpAndRoleViewModel = new ListEmpAndRoleViewModel();
        }
        public IActionResult Index()
        {
            string token = Request.Cookies["Token"];
            List<EmployeeViewModel> listmodel = new List<EmployeeViewModel>();
            var Employees = _employeeApiAccess.EmployeeList(ApiURL, token);
            listEmpAndRoleViewModel.Roles = _employeeRoleApiAccess.EmployeeRolesList(ApiURL, token);
            foreach (var item in Employees)
            {
                item.EmployeeRoleName = listEmpAndRoleViewModel.Roles.Where(a => a.RollId == item.EmployeeRole).Select(a=>a.EmployeeRole).FirstOrDefault();
                listEmpAndRoleViewModel.Employees.Add(item);
            }
            return View(listEmpAndRoleViewModel);
        }

        [HttpPost]
        public ActionResult AddEmployeeRecord(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string token = Request.Cookies["Token"];
                    if (model.Id > 0)
                    {
                        //_employeeApiAccess.UpdateWarehouseAddress(ApiURL, token, model);
                    }
                    else
                    {
                        model.CreatedOn = DateTime.UtcNow;
                        model.EmployeeId = 123456;
                        _employeeApiAccess.SaveEmployee(ApiURL, token, model);
                    }
                    return RedirectToAction("Index", "ManageEmployee");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public EmployeeViewModel GetEmployeeById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int empId = Convert.ToInt32(id);
                string token = Request.Cookies["Token"];
                EmployeeViewModel ViewModel = new EmployeeViewModel();
                ViewModel = _employeeApiAccess.EidtEmployeeByid(ApiURL, token, empId);
                return ViewModel;
            }
            else
            {
                return null;
            }
        }

        public IActionResult EmployeeRoleIndex()
        {
            return View();
        }

        public ActionResult AddEmployeeRole(EmployeeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string token = Request.Cookies["Token"];
                    if (model.RollId > 0)
                    {
                        //_employeeApiAccess.UpdateWarehouseAddress(ApiURL, token, model);
                    }
                    else
                    {
                        _employeeRoleApiAccess.SaveEmployeeRole(ApiURL, token, model);
                    }
                    return RedirectToAction("ManageEmployeeIndex", "ManageEmployee");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }
    }
}
