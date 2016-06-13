using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Quotes;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminQuoteController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

         #region Constructor
        public AdminQuoteController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion

        //
        //GET: /Admin/Quote/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWQUOTE;

            return View();
        }

        //
        //Post: /Admin/Quote/New
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(QuoteViewModel daily_quote)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWQUOTE;

            if (ModelState.IsValid)
            {


                int new_id = servicesManager.QuoteService.AddNewQuote(daily_quote);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "User Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "User Failed To Add";
                
            }

            return View();
        }

        //
        //GET: /Admin/User/Quote/{quote-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.QUOTES;
            QuoteViewModel user = servicesManager.QuoteService.GetQuoteById(id);

            if (user == null)
                return RedirectToAction("List");

            return View(user);
        }

        //
        //POST: /Admin/Quote/Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(QuoteViewModel  daily_quote)
        {
            ViewBag.SelectedPage = Navigator.Items.QUOTES;
            
            if (ModelState.IsValid)
            {
                int new_id = servicesManager.QuoteService.UpdateQuote(daily_quote);
                if (new_id > 0)
                    TempData["SuccessMessage"] = "Quote Updated Successfully";
                else
                    TempData["ErrorMessage"] = "Quote Failed To Update";
            }
            return View(daily_quote);
        }


        public ActionResult List()
        {
            ViewBag.SelectedPage = Navigator.Items.LISTQUOTES;
            return View();
        }

        public JsonResult _QuotesList(int draw, int start = 0, int length = 2)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.QuoteService.GetQuotes(out total_count, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = result.data.Count();
            return Json(result);
        }

        [HttpPost]
        public JsonResult _Delete(int quote_id)
        {
            if (servicesManager.QuoteService.DeleteQuote(quote_id))
                return Json(new { Success = true, quote_id = quote_id });
            else
                return Json(new { Success = false, quote_id = quote_id });
            
        }
    }
}
