using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.Core.ViewModels.Articles;
using Sa3adaty.Core.ViewModels.Polls;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Filters;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
   
    [MaintenanceFilter]
    public class HomeController : Controller
    {
        private int dailyCampaign = 2;

        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

        #region Constructor
        public HomeController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
        }
        #endregion


        public ActionResult Index()
        {
            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            //Get All latest articles list
            List<ListArticleViewModel> all_latest = servicesManager.ArticleFrontService.GetHomeAllLatest(9, new List<ListArticleViewModel>());

            ViewBag.SliderArticles = servicesManager.ArticleFrontService.FormatImageURL(all_latest.Skip(0).Take(4).ToList(), ArticleService.ArticleThumbWidth, ArticleService.ArticleThumbHeight);

            ViewBag.LatestArticlesLeft = servicesManager.ArticleFrontService.FormatImageURL(all_latest.Skip(4).Take(5).ToList(), ArticleService.ArticleThumbWidth3, ArticleService.ArticleThumbHeight3);

            ViewBag.ArticlesPerCategory = servicesManager.ArticleFrontService.GetArticlesPerFeatureCategories(3, all_latest, ArticleService.ArticleThumbWidth8, ArticleService.ArticleThumbHeight8);

            ViewBag.FeaturedVideos = servicesManager.VideoFrontService.GetRandomVideos(3, VideoService.VideoThumbWidth, VideoService.VideoThumbHeight, null);

            return View();
        }

        public ActionResult _Header()
        {
            //Get Current User Info
            if (WebSecurity.IsAuthenticated)
                ViewBag.CurrentUser = servicesManager.AccountFrontService.GetSimpleUserById(WebSecurity.CurrentUserId);

            //ViewBag.FeaturedCategories = servicesManager.ArticleFrontService.GetFeaturedCategories();

            return PartialView();
        }
        public ActionResult _Footer()
        {
            //ViewBag.FeaturedCategories = servicesManager.ArticleFrontService.GetFeaturedCategories();

            return PartialView();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
           return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactInfoViewModel contact_info)
        {
            if(ModelState.IsValid)
            {
                if (servicesManager.ContactInfoFrontService.AddContactInfo(contact_info) > 0)
                {
                    TempData["Success"] = true;
                    return RedirectToAction("Contact");
                }
                else
                {
                    ViewBag.Success = false;
                }
            }
            return View(contact_info);
        }

        public ActionResult Advertise()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Advertise(AdvertiseInfoViewModel advertise_info)
        {
            if (ModelState.IsValid)
            {
                if (servicesManager.AdvertiseInfoFrontService.AddAdvertiseInfo(advertise_info) > 0)
                {
                    TempData["Success"] = true;
                    return RedirectToAction("Advertise");
                }
                else
                {
                    ViewBag.Success = false;
                }
            }
            return View(advertise_info);
        }

        public ActionResult Terms()
        {
            return View(servicesManager.StaticPagesFrontService.GetStaticPage("Terms"));
        }

        public ActionResult _DailyPoll()
        {
            PollViewModel tip = servicesManager.PollFrontService.GetTodayPoll(dailyCampaign);

            return PartialView(tip);
        }
        public JsonResult _SubmitPollAnswer(int poll_id, int answer_id)
        {
            if (servicesManager.PollFrontService.AddPollAnswer(answer_id))
            {
                return Json(new { success = true, data = servicesManager.PollFrontService.GetPollResults(poll_id),poll_id = poll_id  });
            }
            else
            {
                return Json(new { success = false, poll_id = poll_id });
            }
        }
    }
}
