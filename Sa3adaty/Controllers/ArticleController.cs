using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.Core.ViewModels.Articles;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Filters;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
    [MaintenanceFilter]
    public class ArticleController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public ArticleController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion

        //
        // GET: /Article/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult article(string id)
        {
            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            //get the article info
            ArticleViewModel view_model = servicesManager.ArticleFrontService.GetArticleByURL(id,WebSecurity.CurrentUserId);

            if (view_model == null)
            {
                throw new HttpException(404, "Page Not Found");
            }

            //Get latest articles list
            List<int> except = new List<int>();
            except.Add(view_model.ArticleId);
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(6, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3, except);

            List<int> except_list = ((List<ListArticleViewModel>)ViewBag.LatestArticlesLeft).Select(a=> a.ArticleId).ToList();
            except_list.Add(view_model.ArticleId);
            //Get related articles list top
          //  ViewBag.RelatedArticlesTop = servicesManager.ArticleFrontService.GetRelatedArticles(view_model.ArticleId,view_model.Title, 6, ArticleService.ArticleThumbWidth6, ArticleService.ArticleThumbHeight6);

            //Get related recommended articles bottom
            ViewBag.RelatedArticlesBottom = servicesManager.ArticleFrontService.GetRelatedArticles(view_model.ArticleId, view_model.Title, 6, ArticleService.ArticleThumbWidth4, ArticleService.ArticleThumbHeight4, except_list);

           

            ////Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.CategoryName, URL = Url.Action("ListByTag", "Category", new { id  = view_model.CategoryURL.Replace("_","-") }) });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.Title, URL = Url.Action("article", "Article", new { id=view_model.URL }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }

        [Authorize]
        public JsonResult _likeArticle(int article_id)
        {
            if (servicesManager.ArticleFrontService.LikeArticle(article_id, WebSecurity.CurrentUserId))
            {
                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }

        [Authorize]
        public JsonResult _commentArticle(int article_id, string text,int? parent_id = null)
        {
            int comment_id = servicesManager.ArticleFrontService.AddComment(article_id, WebSecurity.CurrentUserId, text,parent_id);
            
            if(comment_id > 0)
                return Json(new { success = true ,comment_id = comment_id  });
            else
                return Json(new { success = false });
        }

        [Authorize]
        [HttpPost]
        public ActionResult _getComment(int comment_id)
        {
           
            return PartialView("_Comment", servicesManager.ArticleFrontService.GetCommentById(comment_id));
        }


        public ActionResult Search(string q, int page = 1, int page_size = 15)
        {
            if (Request.RawUrl.Contains("Article/Search"))
                return RedirectToActionPermanent("Search", "Article", new { q= q, page=page});
            
            //If first page redirect to correct url without page parameter
            if (page <= 1 && Request.RawUrl.Contains(q + "/" + page.ToString()))
            {
                return RedirectToActionPermanent("Search", new { q = q, page = "" });
            }

            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            //Get latest articles list
            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(5, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3);

            SearchViewModel view_model = new SearchViewModel() { Page = page, SearchKey = q };
            view_model.Articles = servicesManager.ArticleFrontService.GetSearchResult(q, page < 1 ? 1 : page, page_size, ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7, 100);
            view_model.MetaTitle = "نتائج البحث عن: " + q;
            view_model.MetaDescription = "نتائج البحث عن: " + q + " من موقع سعادتي";

            //Set pagination properties
            view_model.PageNumber = page;
            view_model.PageSize = page_size;
            view_model.TotalItems = servicesManager.ArticleFrontService.GetSearchTotalCount(q, page, page_size);
            view_model.LinkTemplate = "/بحث/"+q+"/{page}";

            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "نتائج البحث عن: " + q, URL = Url.Action("Search", "Article", new { q= q }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }

        [HttpPost]
        public ActionResult _SearchMore(string search_key, int page = 0, int page_size = 15)
        {
            List<ListArticleViewModel> view_model = servicesManager.ArticleFrontService.GetSearchResult(search_key, page, page_size, ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7, 100);

            return PartialView("_HorizontalArticlesList", view_model);
        }
    }
}
