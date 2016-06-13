using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Authors;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
    public class AdminAuthorController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
            public AdminAuthorController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion

        #region Actions
            //
            //GET: /Admin/Quote/New
            public ActionResult New()
            {
                ViewBag.SelectedPage = Navigator.Items.NEWAUTHOR;

                return View();
            }

            //
            //Post: /Admin/Quote/New
            [HttpPost]
            [ValidateInput(false)]
            public ActionResult New(AuthorViewModel autor,IEnumerable<HttpPostedFileBase> Images)
            {
                ViewBag.SelectedPage = Navigator.Items.NEWAUTHOR;

                if (ModelState.IsValid)
                {

                    if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
                    {
                        TempData["ErrorMessage"] = "Author Failed To Add, Invalid Image File";
                    }
                    else
                    {
                        int new_id = servicesManager.AuthorService.AddNewAuthor(autor, Images);

                        if (new_id > 0)
                        {
                            TempData["SuccessMessage"] = "Author Added Successfully";
                            return RedirectToAction("Edit", new { id = new_id });
                        }
                        else
                            TempData["ErrorMessage"] = "Author Failed To Add";
                    }
                }

                return View();
            }

            //
            //GET: /Admin/User/Quote/{quote-id}
            public ActionResult Edit(int id)
            {
                ViewBag.SelectedPage = Navigator.Items.AUTHORS;
                AuthorViewModel user = servicesManager.AuthorService.GetAuthorById(id);

                if (user == null)
                    return RedirectToAction("List");

                return View(user);
            }

            //
            //POST: /Admin/Quote/Edit
            [HttpPost]
            [ValidateInput(false)]
            public ActionResult Edit(AuthorViewModel author)
            {
                ViewBag.SelectedPage = Navigator.Items.AUTHORS;

                if (ModelState.IsValid)
                {
                    int new_id = servicesManager.AuthorService.UpdateAuthor(author);
                    if (new_id > 0)
                        TempData["SuccessMessage"] = "Author Updated Successfully";
                    else
                        TempData["ErrorMessage"] = "Author Failed To Update";
                }
                return View(author);
            }

            [HttpPost]
            public JsonResult _NewImageAjax(int AuthorId, string Caption, string Description, IEnumerable<HttpPostedFileBase> Images)
            {
                if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
                {
                    //error
                    return Json(new { Success = false, Message = "Invalid image" });
                }
                else
                {
                    servicesManager.AuthorService.AddAuthorImages(AuthorId, Images, "", Caption, Description);
                }
                return Json(new { Success = true, Message = "Image Added Successfully" });
            }

            public JsonResult _UpdateImageAjax(int ImageId, string Caption, string Description, IEnumerable<HttpPostedFileBase> Image)
            {
                if (Image != null && Image.Count() > 0 && !ImageService.IsValid(Image))
                {
                    //error
                    return Json(new { Success = false, Message = "Invalid image" });
                }
                else
                {
                    servicesManager.AuthorService.UpdateAuthorImage(ImageId, (Image != null ? Image.FirstOrDefault() : null), "", Caption, Description);
                }
                return Json(new { Success = true, Message = "Image Added Successfully" });
            }

            public ActionResult _AuthorImages(int AuthorId)
            {
                var view_model = servicesManager.AuthorService.GetAuthorImages(AuthorId);
                return View("_ImagesList", view_model ?? new List<ImageViewModel>());
            }

            [HttpPost]
            public JsonResult _GetImage(int ImageId)
            {
                var view_model = servicesManager.AuthorService.GetAuthorImage(ImageId);
                return Json(view_model);
            }

            [HttpPost]
            public JsonResult _DeleteImage(int image_id)
            {
                servicesManager.AuthorService.DeleteAuthorimage(image_id);
                return Json(new { Success = true, image_id = image_id });
            }


            public ActionResult List()
            {
                ViewBag.SelectedPage = Navigator.Items.LISTAUTHOR;
                return View();
            }

            public JsonResult _AuthorsList(int draw, int start = 0, int length = 2)
            {
                string search = Request.Form["search[value]"];
                string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                string order_dir = Request.Form["order[0][dir]"];

                DataTableViewModel result = new DataTableViewModel();
                result.draw = draw;
                int total_count;
                result.data = servicesManager.AuthorService.GetAuthors(out total_count, start, length, search, order_by, order_dir);

                result.recordsTotal = total_count;
                result.recordsFiltered = result.data.Count();
                return Json(result);
            }

            [HttpPost]
            public JsonResult _Delete(int author_id)
            {
                if (servicesManager.AuthorService.DeleteAuthor(author_id))
                    return Json(new { Success = true, quote_id = author_id });
                else
                    return Json(new { Success = false, quote_id = author_id });

            }
        #endregion
    }
}
