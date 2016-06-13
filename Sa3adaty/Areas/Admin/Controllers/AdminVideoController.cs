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
using Sa3adaty.Core.ViewModels.Admin.Videos;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminVideoController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;
        
        #region Constructor
        public AdminVideoController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion

        //
        //GET: /Admin/AdminVideo/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWVIDEO;
            FillVideoCategories(null);
            FillVideoAuthor(null);
            FillAvailableTags();
            return View();
        }

        //
        //Post: /Admin/AdminVideo/New
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(VideoViewModel video, IEnumerable<HttpPostedFileBase> Images)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWVIDEO;
            FillVideoCategories(video.CategoryId);
            FillVideoAuthor(video.AuthorId);
            FillAvailableTags();

            if (ModelState.IsValid)
            {
                video.URL = URLValidator.CleanURL(video.URL);
                if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
                {
                    TempData["ErrorMessage"] = "Video Failed To Add, Invalid Image File";
                }
                else if (!URLValidator.IsValidURLPart(video.URL))
                {
                    TempData["ErrorMessage"] = "Video Failed To Add, URL Not Valid";
                }
                else if (servicesManager.VideoService.IsVideoURLExist(video.URL))
                {
                    TempData["ErrorMessage"] = "Video Failed To Add, URL already exist";
                }
                else
                {
                    int new_id = servicesManager.VideoService.AddNewVideo(video, Images);

                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Video Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "Video Failed To Add";
                }
            }

            return View(video);
        }

        //
        //GET: /Admin/AdminVideo/{video-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.VIDEOS;
            VideoViewModel vi = servicesManager.VideoService.GetVideoById(id);

            //FillArticleCategories(art.CategoryId);
            FillVideoCategories(vi.CategoryId);
            FillVideoAuthor(vi.AuthorId);
            FillAvailableTags();

            if (vi == null)
                return RedirectToAction("List");

            return View(vi);
        }

        //
        //POST: /Admin/Article/Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(VideoViewModel  video)
        {
            ViewBag.SelectedPage = Navigator.Items.VIDEOS;
            if (ModelState.IsValid)
            {
                video.URL = URLValidator.CleanURL(video.URL);
                if (!URLValidator.IsValidURLPart(video.URL))
                {
                    TempData["ErrorMessage"] = "Video Failed To Add, URL Not Valid";
                }
                else if (servicesManager.VideoService.IsVideoURLExist(video.URL, video.VideoId))
                {
                    TempData["ErrorMessage"] = "Video Failed To Add, URL already exist";
                }
                else
                {
                    int new_id = servicesManager.VideoService.UpdateVideo(video);
                    if (new_id > 0)
                    {
                        TempData["SuccessMessage"] = "Video Updated Successfully";
                        return RedirectToAction("Edit", video.VideoId);
                    }
                    else
                        TempData["ErrorMessage"] = "Video Failed To Update";
                }
            }
            //FillArticleCategories(article.CategoryId);
            FillVideoAuthor(video.AuthorId);
            FillVideoCategories(video.CategoryId);
            FillAvailableTags();
            return View(video);
        }


        [HttpPost]
        public JsonResult _NewImageAjax(int VideoId, string Caption, string Description, IEnumerable<HttpPostedFileBase> Images)
        {
            if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
            {
                //error
                return Json(new { Success = false, Message = "Invalid image" });
            }
            else
            {
                servicesManager.VideoService.AddVideoImages(VideoId, Images, servicesManager.VideoService.GetVideoURL(VideoId), Caption, Description);
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

                servicesManager.VideoService.UpdateVideoImage(ImageId, (Image != null ? Image.FirstOrDefault() : null), servicesManager.VideoService.GetvideoURLByImage(ImageId), Caption, Description);
            }
            return Json(new { Success = true, Message = "Image Added Successfully" });
        }

        public ActionResult _VideoImages(int VideoId)
        {
            var view_model = servicesManager.VideoService.GetVideoImages(VideoId);
            return View("_ImagesList", view_model ?? new List<ImageViewModel>());
        }

        [HttpPost]
        public JsonResult _GetImage(int ImageId)
        {
            var view_model = servicesManager.VideoService.GetVideoImage(ImageId);
            return Json(view_model);
        }

        [HttpPost]
        public JsonResult _DeleteImage(int image_id)
        {
            servicesManager.VideoService.DeleteVideoimage(image_id);
            return Json(new { Success = true, image_id = image_id });
        }

        //
        //GET: /Admin/Video/List
        public ActionResult List(int CategoryId = 0)
        {
            ViewBag.SelectedPage = Navigator.Items.LISTVIDEOS;
            VideosListViewModel view_model = new VideosListViewModel() { CategoryId = CategoryId };
            FillVideoCategories(CategoryId);
            return View();
        }

        public JsonResult _VideosList(int draw, int start = 0, int length = 2, int CategoryId = 0)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.VideoService.GetVideos(out total_count,CategoryId, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = total_count;
            return Json(result);
        }

        [HttpPost]
        public JsonResult _Delete(int video_id)
        {
            if (servicesManager.VideoService.DeleteVideo(video_id))
                return Json(new { Success = true, video_id = video_id });
            else
                return Json(new { Success = false, video_id = video_id });

        }

        public ActionResult CommentsList()
        {
            ViewBag.SelectedPage = Navigator.Items.COMMENTSVIDEOS;
            return View();
        }

        public JsonResult _CommentsList(int draw, int start = 0, int length = 2, int article_id = 0)
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
        //private void FillArticleCategories(int? selected_value)
        //{
        //    SelectList result = servicesManager.ArticleService.GetSelectListCategories(selected_value);
        //    ViewData["CategoryId_Data"] = result;
        //}

        private void FillVideoAuthor(int? selected_value)
        {
            SelectList result = servicesManager.ArticleService.GetSelectListAuthors(selected_value);
            ViewData["AuthorId_Data"] = result;
        }

        private void FillAvailableTags()
        {
            ViewData["tags_Data"] = servicesManager.TagOfVideoService.GetAvailableTags();
        }

        private void FillVideoCategories(int? selected_value)
        {
            SelectList result = servicesManager.VideoService.GetSelectListCategories(selected_value);
            ViewData["CategoryId_Data"] = result;
        }
        #endregion

    }
}
