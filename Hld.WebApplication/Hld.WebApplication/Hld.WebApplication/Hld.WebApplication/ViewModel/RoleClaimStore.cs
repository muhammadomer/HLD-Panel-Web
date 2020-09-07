using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class RoleClaimStore
    {
        public static List<Claim> AllRoleClaims = new List<Claim>()
    {
        new Claim("Access to dashboard", "Access to dashboard"),
        new Claim("Access to PO","Access to PO"),
        new Claim("Access to Import PO","Access to Import PO"),
        new Claim("Access to Add or Edit Product","Access to Add or Edit Product"),
        new Claim("Access to Zinc Property Page","Access to Zinc Property Page"),
        new Claim("Access to BB property page","Access to BB property page"),
        new Claim("Access to update DS Qty","Access to update DS Qty"),
        new Claim("Access to Orders","Access to Orders"),
        new Claim("Access to Inventory","Access to Inventory"),
        new Claim("Access to Inventory Tab","Access to Inventory Tab"),
        new Claim("Access to Order Tab","Access to Order Tab"),
        new Claim("Access to BulkUpdate Tab","Access to BulkUpdate Tab"),
        new Claim("Access to Setting Tab","Access to Setting Tab"),
        new Claim("Access to Admin Tab","Access to Admin Tab"),
        new Claim("Access to Product Link in PO","Access to Product Link in PO"),
       
        new Claim("Access to P&L on Order list view page","Access to P&L on Order list view page"),
    };
    }
    public class RoleClaimsViewModel
    {
        public RoleClaimsViewModel()
        {
            Cliams = new List<RoleClaim>();
        }

        public string RoleId { get; set; }
        public List<RoleClaim> Cliams { get; set; }
    }
    public class RoleClaim
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
