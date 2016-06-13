using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Common;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.Articles;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminVideoCategoryController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public AdminVideoCategoryController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion


        //
        //GET: /Admin/Category/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWVIDEOCATEGORY;
            return View();
        }

        //
        //POST: /Admin/Category/New
        [HttpPost]
        public ActionResult New(CategoryViewModel category, HttpPostedFileBase Images)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWVIDEOCATEGORY;

            if (ModelState.IsValid)
            {
                category.URL = category.URL.Trim().Replace(" ", "-");
                if (Images != null  && !ImageService.IsValid(Images))
                {
                    TempData["ErrorMessage"] = "Category Failed To Add, Invalid Image File";
                }
                else if (!URLValidator.IsValidURLPart(category.URL))
                {
                    TempData["ErrorMessage"] = "Category Failed To Add, URL Not Valid";
                }
                else if (servicesManager.VideoService.IsCategoryURLExist(category.URL))
                {
                    TempData["ErrorMessage"] = "Category Failed To Add, URL already exist";
                }
                else
                {
                    int new_id = servicesManager.VideoService.AddNewCategroy(category, Images);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Category Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "Category Failed To Add";
                }
            }

            return View(category);
        }

        [HttpPost]
        public ActionResult NewImage(int CategoryId, string Caption, string Description, HttpPostedFileBase Images)
        {
            if (Images != null && !ImageService.IsValid(Images))
            {
                //error
                return View();
            }
            else
            {
                servicesManager.VideoService.AddCategoryImage(CategoryId, Images, "", Caption, Description);
            }
            return View();
        }

        [HttpPost]
        public JsonResult _NewImageAjax(int CategoryId, string Caption, string Description, HttpPostedFileBase Images)
        {
            if (Images != null  && !ImageService.IsValid(Images))
            {
                //error
                return Json(new { Success = false, Message = "Invalid image" });
            }
            else
            {
                servicesManager.VideoService.AddCategoryImage(CategoryId, Images, "", Caption, Description);
            }
            return Json(new { Success = true, Message = "Image Added Successfully" });
        }

        public JsonResult _UpdateImageAjax(int ImageId, string Caption, string Description, HttpPostedFileBase Image)
        {
            if (Image != null  && !ImageService.IsValid(Image))
            {
                //error
                return Json(new { Success = false, Message = "Invalid image" });
            }
            else
            {
                servicesManager.ArticleService.UpdateCategoryImage(ImageId, (Image != null ? Image : null), "", Caption, Description);
            }
            return Json(new { Success = true, Message = "Image Added Successfully" });
        }

        public ActionResult _CategoryImages(int CategoryId)
        {
            var view_model = servicesManager.VideoService.GetCategoryImages(CategoryId);
            return View("_ImagesList", view_model ?? new List<ImageViewModel>());
        }

        [HttpPost]
        public JsonResult _GetImage(int ImageId)
        {
            var view_model = servicesManager.VideoService.GetCategoryImage(ImageId);
            return Json(view_model);
        }

        [HttpPost]
        public JsonResult _DeleteImage(int image_id)
        {
            servicesManager.ArticleService.DeleteCategoryimage(image_id);
            return Json(new { Success = true, image_id = image_id });
        }
        //
        //GET: /Admin/Category/Edit/{category-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.VIDEOCATEGORIES;
            CategoryViewModel cat = servicesManager.VideoService.GetCategoryById(id);

            if (cat == null)
                return RedirectToAction("List");

            return View(cat);
        }

        //
        //POST: /Admin/Category/Edit
        [HttpPost]
        public ActionResult Edit(CategoryViewModel category)
        {
            ViewBag.SelectedPage = Navigator.Items.VIDEOCATEGORIES;
            if (ModelState.IsValid)
            {
                category.URL = category.URL.Trim().Replace(" ", "-");
                if (!URLValidator.IsValidURLPart(category.URL))
                {
                    TempData["ErrorMessage"] = "Category Failed To Add, URL Not Valid";
                }
                else if (servicesManager.VideoService.IsCategoryURLExist(category.URL, category.CategoryId))
                {
                    TempData["ErrorMessage"] = "Category Failed To Add, URL already exist";
                }
                else
                {
                    int new_id = servicesManager.VideoService.UpdateCategory(category);
                    if (new_id > 0)
                        TempData["SuccessMessage"] = "Category Updated Successfully";
                    else
                        TempData["ErrorMessage"] = "Category Failed To Update";
                }
            }

            return RedirectToAction("Edit", new { id = category.CategoryId });
        }

        [HttpPost]
        public JsonResult _Delete(int category_id)
        {
            if (servicesManager.ArticleService.DeleteCategory(category_id))
                return Json(new { Success = true, category_id = category_id });
            else
                return Json(new { Success = false, category_id = category_id });

        }

        //
        //GET: /Admin/Category/List
        public ActionResult List()
        {
            ViewBag.SelectedPage = Navigator.Items.LISTVIDEOCATEGORIES;

            return View();
        }

        public JsonResult _CategoriesList(int draw, int start = 0, int length = 2)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            result.data = servicesManager.VideoService.GetCategories(start, length, search, order_by, order_dir);

            result.recordsTotal = servicesManager.VideoService.GetOnlineCategoriesCount();
            result.recordsFiltered = result.recordsTotal;
            return Json(result);
        }



       
    }
}
