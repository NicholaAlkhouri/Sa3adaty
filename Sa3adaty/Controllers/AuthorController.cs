using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.Core.ViewModels.Author;
using Sa3adaty.DAL.Infrastructure;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
    public class AuthorController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public AuthorController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion


        public ActionResult author(string id)
        {
            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            //get the article info
            AuthorViewModel view_model = servicesManager.AuthorFrontService.GetAuthorByURL(id);

            if (view_model == null || !view_model.IsProfileEnabled )
            {
                throw new HttpException(404, "Page Not Found");
            }

            ////Get latest articles list
            //List<int> except = new List<int>();
            //except.Add(view_model.ArticleId);
            //ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.GetLatestArticles(6, ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3, except);

            //List<int> except_list = ((List<ListArticleViewModel>)ViewBag.LatestArticlesLeft).Select(a => a.ArticleId).ToList();
            //except_list.Add(view_model.ArticleId);
            ////Get related articles list top
            ////  ViewBag.RelatedArticlesTop = servicesManager.ArticleFrontService.GetRelatedArticles(view_model.ArticleId,view_model.Title, 6, ArticleService.ArticleThumbWidth6, ArticleService.ArticleThumbHeight6);

            ////Get related recommended articles bottom
            //ViewBag.RelatedArticlesBottom = servicesManager.ArticleFrontService.GetRelatedArticles(view_model.ArticleId, view_model.Title, 6, ArticleService.ArticleThumbWidth4, ArticleService.ArticleThumbHeight4, except_list);



            ////Fill Bread Crumb
            BreadCrumbViewModel bread_crumb = new BreadCrumbViewModel();
            bread_crumb.Items = new List<BreadCrumbItemViewModel>();
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الرئيسية", URL = "/" });
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = "الكتاب"});
            bread_crumb.Items.Add(new BreadCrumbItemViewModel() { Text = view_model.Title, URL = Url.Action("author", "Author", new { id = view_model.URL }) });
            ViewBag.BreadCrumb = bread_crumb;

            return View(view_model);
        }
        //
        // GET: /Author/

        public ActionResult Index()
        {
            return View();
        }

    }
}
