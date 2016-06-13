using System.Web.Mvc;
using LowercaseRoutesMVC;

namespace Sa3adaty.Areas.ForumAdmin
{
    public class ForumAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ForumAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ForumAdmin_editcategoryroute",
                "ForumAdmin/{controller}/{action}/{id}",
                new { controller = "AdminCategory", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "MVCForum.Website.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "ForumAdmin_edituserroute",
                "ForumAdmin/{controller}/{action}/{userId}",
                new { controller = "Admin", action = "Index", userId = UrlParameter.Optional },
                  namespaces: new[] { "MVCForum.Website.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "ForumAdmin_pagingroute",
                "ForumAdmin/{controller}/{action}/{page}",
                new { controller = "Account", action = "Index", page = UrlParameter.Optional },
                  namespaces: new[] { "MVCForum.Website.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "ForumAdmin_defaultroute",
                "ForumAdmin/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "MVCForum.Website.Areas.Admin.Controllers" }
            );

        }
    }
}
