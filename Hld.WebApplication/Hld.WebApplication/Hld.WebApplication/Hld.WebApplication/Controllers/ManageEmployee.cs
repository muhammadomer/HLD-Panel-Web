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
        public JsonResult AddEmployeeRecord(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool status = false;
                try
                {
                    string token = Request.Cookies["Token"];
                    if (model.Id > 0)
                    {
                        status= _employeeApiAccess.UpdateEmployeeById(ApiURL, token, model);
                    }
                    else
                    {
                        model.CreatedOn = DateTime.UtcNow;
                        status = _employeeApiAccess.SaveEmployee(ApiURL, token, model);
                    }
                    return Json(status);
                }
                catch
                {
                    return Json(status);
                }
            }
            else
            {
                return Json(false);
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
                ViewModel = _employeeApiAccess.EditEmployeeByid(ApiURL, token, empId);
                return ViewModel;
            }
            else
            {
                return null;
            }
        }

        public IActionResult EmployeeRoleIndex()
        {
            ListEmpAndRoleViewModel model = new ListEmpAndRoleViewModel();
            var rolesList = _employeeRoleApiAccess.EmployeeRolesList(ApiURL, token);
            if (rolesList != null)
            {
                model.Roles = rolesList;
                return View(model);
            }
            return View();
        }

        public JsonResult AddEmployeeRole(EmployeeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool status = false;
                try
                {
                    string token = Request.Cookies["Token"];
                    if (model.RollId > 0)
                    {
                        status=_employeeRoleApiAccess.UpdateEmployeeRoleByRoleId(ApiURL, token, model);
                    }
                    else
                    {
                        model.CreatedOn = DateTime.UtcNow;
                        status=_employeeRoleApiAccess.SaveEmployeeRole(ApiURL, token, model);
                    }
                    return Json(status);
                }
                catch
                {
                    return Json(status);
                }
            }
            else
            {
                return Json(false);
            }
        }

        [HttpGet]
        public EmployeeRoleViewModel GetEmployeeRoleByRollId(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int empId = Convert.ToInt32(id);
                string token = Request.Cookies["Token"];
                EmployeeRoleViewModel ViewModel = new EmployeeRoleViewModel();
                ViewModel = _employeeRoleApiAccess.GettEmployeeRollByRollId(ApiURL, token, empId);
                return ViewModel;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult UpdateEmpActiveStatusById(EmployeeViewModel model)
        {
            bool status = false;
            try
            {
                string token = Request.Cookies["Token"];
                if (model.Id > 0)
                {
                    status = _employeeApiAccess.UpdateEmpActiveStatusById(ApiURL, token, model);
                }
                return Json(status);
            }
            catch
            {
                return Json(status);
            }
        }

        public JsonResult GetRandom6DigitsEmployeeId()
        {
            string randomNumbers = string.Empty;
            Random generator = new Random();
            randomNumbers = generator.Next(0, 1000000).ToString("D6");
            return Json(randomNumbers);
        }
    }
}
