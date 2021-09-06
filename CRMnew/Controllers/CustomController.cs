using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMnew.Controllers
{
    public class CustomController : Controller
    {       

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext.User.Identity.Name != "")
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                else
                {
                    List<string> controllerRoles = new List<string>();
                    foreach (var authAttribute in filterContext.ActionDescriptor
                        .GetFilterAttributes(true)
                        .Where(a => a is AuthorizeAttribute).Select(a => a as AuthorizeAttribute))
                    {
                        controllerRoles.AddRange(authAttribute.Roles.Split(','));
                    }

                    foreach (var authAttribute in filterContext.ActionDescriptor.ControllerDescriptor
                        .GetFilterAttributes(true)
                        .Where(a => a is AuthorizeAttribute).Select(a => a as AuthorizeAttribute))
                    {
                        controllerRoles.AddRange(authAttribute.Roles.Split(','));
                    }

                    bool isInRole = controllerRoles.Count == 0;
                    foreach (string controllerRole in controllerRoles)
                    {
                        isInRole = filterContext.HttpContext.User.IsInRole(controllerRole);
                        if (isInRole) break;
                    }

                    if (!isInRole)
                    {
                        filterContext.Result = View("~/Views/Shared/BadRole.cshtml");
                    }
                }
            }
        }
    }
}