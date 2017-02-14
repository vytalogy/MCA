using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Specialized;
using mca.web.Helpers;

namespace mca.web
{
    public class NotRestricted : FilterAttribute
    {
        // Does nothing, just used for decoration
    }

    public class Restricted : ActionFilterAttribute, IActionFilter
    {
        [ValidateInput(false)]
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            object[] attributes = filterContext.ActionDescriptor.GetCustomAttributes(true);
            if (attributes.Any(a => a is NotRestricted)) return;

            if (!Auth.isLogin)
            {
                var values = new RouteValueDictionary(new { action = "Login", controller = "Account" });
                //values.Add("returnUrl", values);
                filterContext.Result = new RedirectToRouteResult(values);
                base.OnActionExecuting(filterContext);
            }
            else
            {
                bool isSkipAuth = false;
                bool isAllow = false;
                string param = string.Empty;

                var req = filterContext.HttpContext.Request;

                var descriptor = filterContext.ActionDescriptor;
                var ActionParameter = filterContext.ActionParameters;
                var actionName = descriptor.ActionName;
                var controllerName = descriptor.ControllerDescriptor.ControllerName;
                var type = filterContext.Controller.GetType();

                if (req.AcceptTypes.Contains("text/html"))
                {

                }
                else if (req.AcceptTypes.Contains("application/json"))
                {
                    if (!actionName.ToLower().Contains("delete"))
                        isSkipAuth = true;
                }
                else if (req.AcceptTypes.Contains("application/xml") || req.AcceptTypes.Contains("text/xml"))
                {
                    if (!actionName.ToLower().Contains("delete"))
                        isSkipAuth = true;
                }

                if (req.IsAjaxRequest() && !actionName.ToLower().Contains("delete"))
                    isSkipAuth = true;               

                //Allow all Partial Request to pass through Authentication
                if (actionName.StartsWith("_"))
                    isSkipAuth = true;            

                if (isSkipAuth)
                    return;
                else
                {
                    //var queryStringParams = new NameValueCollection(req.QueryString);
                    //SecurityService _serv = new SecurityService();
                    ////var pgIndex = (actionName.ToLower() == "index" ? actionName : "");
                    ////var pgParameter = !string.IsNullOrEmpty(pgIndex) ? param : "";

                    //isAllow = _serv.VerifyUserAccess(controllerName, actionName);

                    //if (!isAllow)
                    //{
                    //    var values = new RouteValueDictionary(new { action = "Restricted", controller = "Dashboard" });
                    //    filterContext.Result = new RedirectToRouteResult(values);
                    //    base.OnActionExecuting(filterContext);
                    //}
                }
            }
        }
    }
}