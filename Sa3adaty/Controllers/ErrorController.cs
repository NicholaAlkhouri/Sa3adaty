using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sa3adaty.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ViewResult Index()
        {
            return View("Error");
        }

        public ActionResult antiforgery()
        {
            return View();
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
