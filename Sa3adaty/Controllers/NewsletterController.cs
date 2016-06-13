using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels.Account;
using Sa3adaty.Core.ViewModels.Admin.User;
using Sa3adaty.Core.ViewModels.Articles;
using Sa3adaty.DAL.Infrastructure;
using Sa3adaty.Filters;
using Sa3adaty.Helpers;
using Sa3adaty.Mailers;
using WebMatrix.WebData;

namespace Sa3adaty.Controllers
{
    [MaintenanceFilter]
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class NewsletterController : AsyncController
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;
        public LogService logService;
        private int DailyEmailCampagin = 1;
        private int BatchSize = Convert.ToInt32(ConfigurationManager.AppSettings["EmailBatchSize"]);

        #region Constructor
        public NewsletterController()
        {
            dataManager = new DataAccessManager();
            servicesManager = new ServicesManager(dataManager);
            logService = new LogService(dataManager);
        }
        #endregion

        //
        // GET: /Admin/Newsletter/Daily/password



        public ActionResult Daily(string id)
        {
           
            if (id == "abcd1234")
            {
                // Send Email
                UserMailer mailer = new UserMailer();

                List<SubscriptionViewModel> subs = servicesManager.AccountService.GetSubscriptionEmails(DailyEmailCampagin, BatchSize);
                List<ListArticleViewModel> articles = servicesManager.ArticleFrontService.FormatImageURL(servicesManager.ArticleFrontService.GetHomeAllLatest(6,null, 200), ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7);

                //Prepare Email Service
                EmailService ES = new EmailService();

                string Layout= "";
                 string DailyEmail= "";
                     string DailyEmailArticle= "";
                         string articles_text = "";
                try
                {
                    Layout = ES.LoadTemplate("Layout.html");
                   DailyEmail = ES.LoadTemplate("DailyEmail.html");
                   DailyEmailArticle = ES.LoadTemplate("DailyEmailArticle.html");
                    
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                }
               
                
                foreach (ListArticleViewModel article in articles)
                {
                    articles_text += DailyEmailArticle.Replace("{Description}", article.Description)
                        .Replace("{ArticleTitle}", article.Title)
                        .Replace("{PublishDate}", FrontHelpers.FormatDate(null, article.PublishDate))
                        .Replace("{ArticleURL}", FrontHelpers.AbsoluteArabicURL(null, Url.Action("Article", "Article", new { id = article.URL })))
                        .Replace("{ImageURL}", FrontHelpers.AbsoluteImagePath(null,Url.Content("~/" + article.ImageURL)));
                }

                string final_html_message = Layout.Replace("{Body}", DailyEmail.Replace("{ArticlesList}", articles_text));
                foreach (SubscriptionViewModel sub in subs)
                {
                    try{
                        ES.SendHtmlEmail(sub.Email,articles.FirstOrDefault().Title ,final_html_message);
                                               
                        servicesManager.AccountService.EmailSent(sub.Email, DailyEmailCampagin);
                        //log this sent
                        servicesManager.AccountService.LogSubscription(sub.UserName, sub.UserId.ToString(), sub.Email,Server.MapPath("~/newsletter-log/"));
                      
                    }
                    catch(System.Net.Mail.SmtpFailedRecipientsException ex)
                    {
                        logService.WriteError(ex.Message +"::" + sub.Email + "::" +sub.UserId, ex.Message, ex.StackTrace, ex.Source);
                        servicesManager.AccountService.LogSubscription(ex.Message, ex.StackTrace.ToString(), sub.Email, Server.MapPath("~/newsletter-log/"));
                        servicesManager.AccountService.Unsubscribe(sub.UnsubscriptionToken, DailyEmailCampagin);
                    }
                    catch(Exception ex)
                    {
                        if (ex.GetType().Name == "SmtpFailedRecipientException")
                             servicesManager.AccountService.Unsubscribe(sub.UnsubscriptionToken, DailyEmailCampagin);
                        
                        logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                        servicesManager.AccountService.LogSubscription(ex.Message, ex.StackTrace.ToString(), sub.Email, Server.MapPath("~/newsletter-log/"));

                    }
                }
            }

            return View();
        }



        //public ActionResult DailyCompleted()
        //{
        //    return View();
        //}

        public JsonResult Subscribe(SubscribeNewsletter subscribe)
        {
            if (ModelState.IsValid)
            {
                if (servicesManager.AccountService.Subscribe(null, subscribe.email, DailyEmailCampagin))
                    return Json(new { success = true });
                else
                    return Json(new { success = false , message="حدث خطأ يرجى المحاولة لاحقا"});
            }
            return Json(new { success = false,message="البريد الإلكتروني المدخل غير صالح" });
        }

        protected async Task SendDailyAsync(UserMailer user_mailer,SubscriptionViewModel subscription,List<ListArticleViewModel> articles)
        {
            try
            {
                
                await user_mailer.SendEmail(subscription.Email, "DailyEmail", articles, subscription.UnsubscriptionToken).SendAsync();
                servicesManager.AccountService.EmailSent(subscription.Email, DailyEmailCampagin);

                //log this sent
                servicesManager.AccountService.LogSubscription(subscription.UserName, subscription.UserId.ToString(), subscription.Email,Server.MapPath("~/newsletter-log/"));

            }catch (Exception e)
            {
                logService.WriteError( e.Message, e.Message, e.StackTrace, e.Source);
                servicesManager.AccountService.LogSubscription(e.Message, e.StackTrace.ToString(), subscription.Email, Server.MapPath("~/newsletter-log/"));
                ModelState.AddModelError("", "Issue sending email: " + e.Message);
                servicesManager.AccountService.Unsubscribe(subscription.Email , DailyEmailCampagin);
            }
        }


        public ActionResult DailyNewsletter()
        {

            List<ListArticleViewModel> articles = servicesManager.ArticleFrontService.FormatImageURL(servicesManager.ArticleFrontService.GetHomeAllLatest(6, null, 200), ArticleService.ArticleThumbWidth7, ArticleService.ArticleThumbHeight7);
            ViewBag.Articles = articles;

            return View("~/Views/UserMailer/DailyEmail.cshtml", articles);
        }

       
        public ActionResult Unsubscribe(string token)
        {
            ViewBag.Unsubscribe = true;
            servicesManager.AccountService.Unsubscribe(token, DailyEmailCampagin);
            return View();
            if (WebSecurity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Login", "Account",new{ returnUrl = Url.Action("Unsubscribe", "Newsletter")});
        }

    }
}
