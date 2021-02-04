using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class TokenExpiresAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.HttpContext.Request.Cookies["Token"] == null)
            {
                context.Result =
                               new RedirectToRouteResult(
                                   new RouteValueDictionary{{ "controller", "Authentication" },
                                          { "action", "Authenticate" }});

            }
            base.OnActionExecuting(context);
        }

    }
}
