using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using DataAnnotationsExtensions.ClientValidation;
using MVCForum.Domain.Events;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.IOC;
using MVCForum.Website.ScheduledJobs;
using Sa3adaty.Controllers;
using Sa3adaty.Core.Services;
using WebMatrix.WebData;

namespace Sa3adaty
{
    public class Global : System.Web.HttpApplication
    {
        public ISettingsService SettingsService
        {
            get { return DependencyResolver.Current.GetService<ISettingsService>(); }
        }

        public IUnitOfWorkManager UnitOfWorkManager
        {
            get { return DependencyResolver.Current.GetService<IUnitOfWorkManager>(); }
        }

        public IBadgeService BadgeService
        {
            get { return DependencyResolver.Current.GetService<IBadgeService>(); }
        }

        public ILoggingService LoggingService
        {
            get { return DependencyResolver.Current.GetService<ILoggingService>(); }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

           // WebApiConfig.Register(GlobalConfiguration.Configuration);
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //This should be called once in the application live
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "Email", autoCreateTables: true);

            // Start unity
            var unityContainer = UnityHelper.Start();

            // Run scheduled tasks
            ScheduledRunner.Run(unityContainer);

            // Register Data annotations
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();

            // Set default theme
            var defaultTheme = "Metro";

            // Get the theme from the database.
            defaultTheme = SettingsService.GetSettings().Theme;

            // Do the badge processing
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    BadgeService.SyncBadges();
                    unitOfWork.Commit();
                }
                catch (Exception exception)
                {
                    LoggingService.Error(string.Format("Error processing badge classes: {0}", exception.Message));

                    LogService logservice = new LogService();
                    logservice.WriteError(exception.Message, exception.Message, exception.StackTrace, exception.Source);
                }
            }

            // Initialise the events
            EventManager.Instance.Initialize(LoggingService);
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            
           
            if (exception is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError(); //make sure you log the exception first
                Response.Redirect("/error/antiforgery", true);
            }
            else if (exception is HttpException)
            {
                if (((HttpException)exception).GetHttpCode() == 404)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Error");
                    routeData.Values.Add("action", "NotFound");

                    // clear the error, otherwise, we will always get the default error page.
                    Server.ClearError();

                    // call the controller with the route
                    IController errorController = new ErrorController();
                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }

            }
            else
            {
                LogService logservice = new LogService();
                logservice.WriteError(exception.Message, exception.Message, exception.StackTrace, exception.Source);

            }
        }
    }
}