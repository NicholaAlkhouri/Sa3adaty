using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core.ViewModels.Admin.Articles;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.Services;
using Sa3adaty.Common;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminArticleController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
            public AdminArticleController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion


        //
        //GET: /Admin/Article/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWARTICLE;
            FillArticleCategories(null);
            FillArticleAuthor(null);
            FillAvailableTags();
            return View();
        }

        //
        //Post: /Admin/Article/New
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(ArticleViewModel article, IEnumerable<HttpPostedFileBase> Images)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWARTICLE;
            FillArticleCategories(article.CategoryId);
            FillArticleAuthor(article.AuthorId);
            FillAvailableTags();

            if (ModelState.IsValid)
            {
                article.URL = URLValidator.CleanURL(article.URL);
                if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
                {
                    TempData["ErrorMessage"] = "Article Failed To Add, Invalid Image File";
                }
                else if (!URLValidator.IsValidURLPart(article.URL))
                {
                     TempData["ErrorMessage"] = "Article Failed To Add, URL Not Valid";
                }
                else if (servicesManager.ArticleService.IsArticleURLExist(article.URL))
                {
                    TempData["ErrorMessage"] = "Article Failed To Add, URL already exist";
                }
                else
                {
                    int new_id = servicesManager.ArticleService.AddNewArticle(article,Images);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Article Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "Article Failed To Add";
                }
            }

            return View(article);
        }

        //
        //GET: /Admin/Article/Edit/{category-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.ARTICLES;
            ArticleViewModel  art = servicesManager.ArticleService.GetArticleById(id);

            FillArticleCategories(art.CategoryId);
            FillArticleAuthor(art.AuthorId);
            FillAvailableTags();

            if (art == null)
                return RedirectToAction("List");

            return View(art);
        }

        //
        //POST: /Admin/Article/Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ArticleViewModel  article)
        {
            ViewBag.SelectedPage = Navigator.Items.ARTICLES;
            if (ModelState.IsValid)
            {
                article.URL = URLValidator.CleanURL(article.URL);
                if (!URLValidator.IsValidURLPart(article.URL))
                {
                    TempData["ErrorMessage"] = "Article Failed To Add, URL Not Valid";
                }
                else if (servicesManager.ArticleService.IsArticleURLExist(article.URL,article.ArticleId))
                {
                    TempData["ErrorMessage"] = "Article Failed To Add, URL already exist";
                }
                else
                {
                    int new_id = servicesManager.ArticleService.UpdateArticle(article);
                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Article Updated Successfully";
                        return RedirectToAction("Edit", article.ArticleId);
                    }
                    else
                        TempData["ErrorMessage"] = "Article Failed To Update";
                }
            }
            FillArticleCategories(article.CategoryId);
            FillArticleAuthor(article.AuthorId);
            FillAvailableTags();
            return View(article);
        }
        
        [HttpPost]
        public JsonResult _NewImageAjax(int ArticleId, string Caption, string Description, IEnumerable<HttpPostedFileBase> Images)
        {
            if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
            {
                //error
                return Json(new { Success = false, Message = "Invalid image" });
            }
            else
            {
                servicesManager.ArticleService.AddArticleImages(ArticleId, Images, servicesManager.ArticleService.GetArticleURL(ArticleId), Caption, Description);
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

                servicesManager.ArticleService.UpdateArticleImage(ImageId, (Image != null ? Image.FirstOrDefault() : null), servicesManager.ArticleService.GetArticleURLByImage(ImageId), Caption, Description);
            }
            return Json(new { Success = true, Message = "Image Added Successfully" });
        }

        public ActionResult _ArticleImages(int ArticleId)
        {
            var view_model = servicesManager.ArticleService.GetArticleImages(ArticleId);
            return View("_ImagesList", view_model ?? new List<ImageViewModel>());
        }

        [HttpPost]
        public JsonResult _GetImage(int ImageId)
        {
            var view_model = servicesManager.ArticleService.GetArticleImage(ImageId);
            return Json(view_model);
        }

        [HttpPost]
        public JsonResult _DeleteImage(int image_id)
        {
            servicesManager.ArticleService.DeleteArticleimage(image_id);
            return Json(new { Success = true, image_id = image_id });
        }

        //
        //GET: /Admin/Article/List
        public ActionResult List(int CategoryId = 0)
        {
            ViewBag.SelectedPage = Navigator.Items.LISTARTICLES;
            ArticlesListViewModel view_model = new ArticlesListViewModel() { CategoryId = CategoryId };
            FillArticleCategories(CategoryId);
            return View(view_model);
        }

        public JsonResult _ArticlesList(int draw, int start = 0, int length = 2,int CategoryId = 0)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.ArticleService.GetArticles(out total_count,CategoryId, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = total_count;
            return Json(result);
        }

        [HttpPost]
        public JsonResult _Delete(int article_id)
        {
            if (servicesManager.ArticleService.DeleteArticle(article_id))
                return Json(new { Success = true, category_id = article_id });
            else
                return Json(new { Success = false, category_id = article_id });

        }


        public ActionResult CommentsList()
        {
            ViewBag.SelectedPage = Navigator.Items.COMMENTSARTICLE;
            return View();
        }

        public JsonResult _CommentsList(int draw, int start = 0, int length = 2,int article_id = 0)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.ArticleService.GetComments(out total_count, article_id, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = total_count;
            return Json(result);
        }

        [HttpPost]
        public JsonResult _DeleteComment(int comment_id)
        {
            if (servicesManager.ArticleService.DeleteComment(comment_id))
                return Json(new { Success = true, category_id = comment_id });
            else
                return Json(new { Success = false, category_id = comment_id });

        }

        #region Helpers Foreigns
        private void FillArticleCategories(int? selected_value)
        {
           SelectList result = servicesManager.ArticleService.GetSelectListCategories(selected_value);
           ViewData["CategoryId_Data"] = result;
        }

        private void FillArticleAuthor(int? selected_value)
        {
            SelectList result = servicesManager.ArticleService.GetSelectListAuthors(selected_value);
            ViewData["AuthorId_Data"] = result;
        }

        private void FillAvailableTags()
        {
            ViewData["tags_Data"] = servicesManager.TagService.GetAvailableTags();
        }
      
        #endregion
    }
}
