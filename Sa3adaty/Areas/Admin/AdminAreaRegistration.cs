using System.Web.Mvc;

namespace Sa3adaty.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                 "Admin_Login",
                 "Admin",
                 new { controller = "AdminAccount", action = "Login", id = UrlParameter.Optional }
             );

            context.MapRoute(
                "Admin_Home",
                "Admin/Home",
                new { controller = "AdminHome", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
