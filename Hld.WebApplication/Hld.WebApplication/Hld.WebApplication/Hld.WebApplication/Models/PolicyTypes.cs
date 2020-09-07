using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Models
{
    public static class PolicyTypes
    {
        public static class Users
        {
            public const string Manage = "users.manage.policy";
            public const string EditRole = "users.edit.role.policy";
        }

        public static class Teams
        {
            public const string Manage = "Edit Role";

            public const string AddRemove = "Delete Role";
        }
    }
}
