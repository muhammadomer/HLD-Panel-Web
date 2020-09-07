using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Models
{
    public class CustomClaimTypes
    {
        public const string Edit_Role = "Edit Role";
        public const string Create_Role = "Create Role";
    }
  
        public static class Users
        {
            public const string Add = "users.add";
            public const string Edit = "users.edit";
            public const string EditRole = "users.edit.role";
        }

        public static class Teams
        {
            public const string AddRemove = "teams.addremove";
            public const string EditManagers = "teams.edit.managers";
            public const string Delete = "teams.delete";
        }

  
  
}
