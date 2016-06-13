using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.Core.ViewModels.Articles;
using Sa3adaty.Core.ViewModels.Videos;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Filters;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
    [MaintenanceFilter]
    public class CategoryController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public CategoryController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion


        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult List(string id,int page =1, int page_size = 15)
        {
            //Redirect to tag from now on, no category list will ever exist.
            return RedirectToActionPermanent("ListByTag", new { id = id.Replace("_", "-") });

            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            CategoryViewModel view_model = servicesManager.ArticleFrontService.GetCategoryByURL(id, page, page_size, ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7,100);

            if (view_model == null)
            {
                throw new HttpException(404, "Page Not Found");
            }
            //Get latest articles list
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(5, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3);

            //Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() {Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.Name, URL = Url.Action("List", "Category", new { id = id }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }

        [HttpPost]
        public ActionResult _LoadMore(int category_id, int page, int page_size = 15)
        {
            List<ListArticleViewModel> view_model = servicesManager.ArticleFrontService.GetCategoryArticles(category_id, page, page_size, ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7, 100);

            return PartialView("_HorizontalArticlesList", view_model); 
        }

        public ActionResult ListByTag(string id, int page = 1, int page_size = 15)
        {
           
            if (id.Contains("_"))
            {
                return RedirectToActionPermanent("ListByTag", new { id = id.Replace("_", "-") });
            }

            //If first page redirect to correct url without page parameter
            if (page <= 1 && Request.RawUrl.Contains(id + "/" + page.ToString() ))
            {
                return RedirectToActionPermanent("ListByTag", new { id = id, page=""});
            }


            //Temporary code replaceo old category with new 
            if (id == "الحياة-الاجتماعية")
            {
                return RedirectToActionPermanent("ListByTag", new { id = "حياة-اجتماعية" });
            }
            else if (id == "الصحة-النفسية")
            {
                return RedirectToActionPermanent("ListByTag", new { id = "صحة-نفسية" });
            }
            else if (id == "الصحة-الشخصية")
            {
                return RedirectToActionPermanent("ListByTag", new { id = "صحة-شخصية" });
            }
            else if (id == "السعادة-الزوجية")
            {
                return RedirectToActionPermanent("ListByTag", new { id = "سعادة-زوجية"});
            }
            else if (id == "الحياة-العائلية")
            {
                return RedirectToActionPermanent("ListByTag", new { id = "حياة-عائلية" });
            }


            CategoryViewModel view_model = servicesManager.ArticleFrontService.GetArticlesTag(id, page<1?1:page , page_size, ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7, 100);
            view_model.LinkTemplate = "/"+id + "/{page}";
            if (view_model == null)
            {
                throw new HttpException(404, "Page Not Found");
            }

            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            view_model.URL = id;
           
            //Get latest articles list
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(5, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3);

            //Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.Name, URL = Url.Action("ListByTag", "Category", new { id = id,page ="" }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }

        [HttpPost]
        public ActionResult _LoadMoreByTag(string  tag, int page, int page_size = 15)
        {
            List<ListArticleViewModel> view_model = servicesManager.ArticleFrontService.GetTagArticles(tag, page, page_size, ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7, 100);

            return PartialView("_HorizontalArticlesList", view_model);
        }

        public ActionResult ListByVideoTag(string id, int page = 1, int page_size = 15)
        {
           
            if (id.Contains("_"))
            {
                return RedirectToActionPermanent("ListByVideoTag", new { id = id.Replace("_", "-") });
            }

            //If first page redirect to correct url without page parameter
            if (page <= 1 && Request.RawUrl.Contains(id + "/" + page.ToString() ))
            {
                return RedirectToActionPermanent("ListByVideoTag", new { id = id, page = "" });
            }


            VideoCategoryViewModel view_model = servicesManager.VideoFrontService.GetVideosTag(id, page < 1 ? 1 : page, page_size, VideoService.VideoThumbWidth7, VideoService.VideoThumbHeight7, 100);
            view_model.LinkTemplate = "/تصنيف-فيديو/"+id + "/{page}";
            if (view_model == null)
            {
                throw new HttpException(404, "Page Not Found");
            }

            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            view_model.URL = id;
           
            //Get latest articles list
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(5, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3);

            //Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "فيديو", URL = "/فيديو" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.Name, URL = Url.Action("ListByVideoTag", "Category", new { id = id, page = "" }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }
        
    }
}
