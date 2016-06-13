using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core.ViewModels.Account;
using Sa3adaty.Filters;
using WebMatrix.WebData;

namespace Sa3adaty.Areas.Admin.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminHomeController : Controller
    {
        //
        // GET: /Admin/Home/
        public ActionResult Index()
        {
            if (!WebSecurity.IsAuthenticated)
                return RedirectToAction("Login", "AdminAccount", new { returnUrl =Url.Action("Index","AdminHome")});
            return View();
        }

        public ActionResult SendEmailTest()
        {
            return View();
        }
    }
}
