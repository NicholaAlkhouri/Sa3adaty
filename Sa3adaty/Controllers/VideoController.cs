using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.Core.ViewModels.Videos;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Filters;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
    public class VideoController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public VideoController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion

        public ActionResult index()
        {
            List<int> except = new List<int>();
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(3, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3, except);

            List<ListVideoViewModel> view_model  = servicesManager.VideoFrontService.GetLatestVideos(15, VideoService.VideoThumbWidth7, VideoService.VideoThumbHeight7,null);

            ////Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "فيديو", URL = Url.Action("index", "Video") });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }

        public ActionResult video(string id)
        {
            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            //get the video info
            VideoViewModel view_model = servicesManager.VideoFrontService.GetVideoByURL(id, WebSecurity.CurrentUserId);

            if (view_model == null)
            {
                throw new HttpException(404, "Page Not Found");
            }

            //Get latest articles list
            List<int> except = new List<int>();
          
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(4, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3, except);

            //List<int> except_list = ((List<ListArticleViewModel>)ViewBag.LatestArticlesLeft).Select(a => a.ArticleId).ToList();
            //except_list.Add(view_model.ArticleId);
            ////Get related articles list top
            ////  ViewBag.RelatedArticlesTop = servicesManager.ArticleFrontService.GetRelatedArticles(view_model.ArticleId,view_model.Title, 6, ArticleService.ArticleThumbWidth6, ArticleService.ArticleThumbHeight6);

            //Get related recommended articles bottom
            ViewBag.RelatedArticlesBottom = servicesManager.ArticleFrontService.GetRelatedArticles( view_model.Title, 3, ArticleService.ArticleThumbWidth4, ArticleService.ArticleThumbHeight4, null);

            except.Add(view_model.VideoId);
            ViewBag.RelatedVideos = servicesManager.VideoFrontService.GetAuthorVideos(3, view_model.Author.AuthorId, VideoService.VideoThumbWidth7, VideoService.VideoThumbHeight7, except);

            ////Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "فيديو", URL = Url.Action("index", "Video") });
           // bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.CategoryName, URL = Url.Action("ListByTag", "Category", new { id = view_model.CategoryURL.Replace("_", "-") }) });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.Title, URL = Url.Action("video", "Video", new { id = view_model.URL }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }

        [Authorize]
        public JsonResult _commentVideo(int video_id, string text, int? parent_id = null)
        {
            int comment_id = servicesManager.VideoFrontService.AddComment(video_id, WebSecurity.CurrentUserId, text, parent_id);

            if (comment_id > 0)
                return Json(new { success = true, comment_id = comment_id });
            else
                return Json(new { success = false });
        }

        [Authorize]
        [HttpPost]
        public ActionResult _getComment(int comment_id)
        {

            return PartialView("~/Views/Article/_Comment.cshtml", servicesManager.VideoFrontService.GetCommentById(comment_id));
        }


    }
}
