using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Sa3adaty.Filters
{
    public class MaintenanceFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/maintenance_mode.html");

            // if we are testing then the file would exist
            if (System.IO.File.Exists(path))
            {
                List<string> validIPAddresses;
                string currentIPAddress;

                // now since it does we need to only allow valid ip addresses through
                validIPAddresses = ConfigurationManager.AppSettings["MaintenanceIps"].Split(',').ToList();
                currentIPAddress = filterContext.RequestContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"];

                if (!validIPAddresses.Contains(currentIPAddress))
                {
                    filterContext.Result = new SiteDownForTestingResult();
                }   
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class SiteDownForTestingResult : ActionResult
    {
        public SiteDownForTestingResult()
            : base()
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/maintenance_mode.html");

            var response = context.HttpContext.Response;

            response.Clear();
            response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            response.StatusDescription = "Service Unavailable.";
            response.WriteFile(path);
            response.End();
        }

    }
}