using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Tags;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagOfVideoController : Controller
    {

        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public AdminTagOfVideoController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion

        //
        //GET: /Admin/AdminTagOfVideo/List
        public ActionResult List()
        {
            ViewBag.SelectedPage = Navigator.Items.LISTVIDEOTAG;
            return View();
        }


        public JsonResult _TagsList(int draw, int start = 0, int length = 2)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.TagOfVideoService.GetTags(out total_count, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = total_count;
            return Json(result);
        }

        //
        //GET: /Admin/AdminTagOfVideo/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWVIDEOTAG;

            return View();
        }

        //
        //Post: /Admin/AdminTagOfVideo/New
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(TagViewModel tag)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWVIDEOTAG;

            if (ModelState.IsValid)
            {
                if (servicesManager.TagOfVideoService.IsTagExist(tag.TagName))
                {
                    TempData["ErrorMessage"] = "Tag Already Exist";
                }
                else
                {
                    int new_id = servicesManager.TagOfVideoService.AddNewTag(tag);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Tag Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "Tag Failed To Add";
                }
            }

            return View(tag);
        }

        //
        //GET: /Admin/AdminTagOfVideo/Edit/{tag-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.TAGS;
            TagViewModel tag = servicesManager.TagOfVideoService.GetTagById(id);

            if (tag == null)
                return RedirectToAction("List");

            return View(tag);
        }

        //
        //POST: /Admin/AdminTagOfVideo/Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TagViewModel tag)
        {
            ViewBag.SelectedPage = Navigator.Items.TAGS;
            if (ModelState.IsValid)
            {
                if (servicesManager.TagOfVideoService.IsTagExist(tag.TagName, tag.TagId))
                {
                    TempData["ErrorMessage"] = "Tag Name Already Exist";
                }
                else
                {
                    int new_id = servicesManager.TagOfVideoService.UpdateTag(tag);
                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Tag Updated Successfully";
                        return RedirectToAction("Edit", tag.TagId);
                    }
                    else
                        TempData["ErrorMessage"] = "Tag Failed To Update";
                }
            }
            return View(tag);
        }


    }
}
