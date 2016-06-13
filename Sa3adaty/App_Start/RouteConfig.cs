using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sa3adaty
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Static Pages
           
            routes.MapRoute(
                name: "About",
                url: "عن-سعادتي",
                defaults: new { controller = "Home", action = "About" },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

            routes.MapRoute(
               name: "Terms",
               url: "شروط-الاستخدام",
               defaults: new { controller = "Home", action = "Terms" },
               namespaces: new[] { "Sa3adaty.Controllers" }
           );

            routes.MapRoute(
              name: "ContactUs",
              url: "اتصل-بنا",
              defaults: new { controller = "Home", action = "Contact" },
              namespaces: new[] { "Sa3adaty.Controllers" }
          );

            routes.MapRoute(
              name: "Advertise",
              url: "اعلن-معنا",
              defaults: new { controller = "Home", action = "Advertise" },
              namespaces: new[] { "Sa3adaty.Controllers" }
          );
            #endregion


            routes.MapRoute(
                name: "Search",
                url: "بحث/{q}/{page}",
                constraints: new { page = @"\d*" },
                defaults: new { controller = "Article", action = "Search" , page = UrlParameter.Optional},
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

           // routes.MapRoute(
           //    name: "HashTag",
           //    url: "tag/{id}",
           //    defaults: new { controller = "Category", action = "ListByTag" },
           //    namespaces: new[] { "Sa3adaty.Controllers" }
           //);

            routes.MapRoute(
                name: "Article",
                url: "مقالات/{id}",
                defaults: new { controller = "Article", action = "article" },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

            routes.MapRoute(
                name: "Videos_Home",
                url: "فيديو",
                defaults: new { controller = "Video", action = "index" },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

            routes.MapRoute(
                name: "Video",
                url: "فيديو/{id}",
                defaults: new { controller = "Video", action = "video" },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

            routes.MapRoute(
               name: "VideoTag",
                  url: "فيديو/تصنيف/{id}",
               defaults: new { controller = "Category", action = "ListByVideoTag", page = UrlParameter.Optional },
               constraints: new { page = @"\d*" },
               namespaces: new[] { "Sa3adaty.Controllers" }
           );

            routes.MapRoute(
                name: "Author",
                url: "كاتب/{id}",
                defaults: new { controller = "Author", action = "author" },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

            routes.MapRoute(
                 name: "AllAuthors",
                 url: "كل-الكتاب",
                 defaults: new { controller = "Author", action = "index" },
                 namespaces: new[] { "Sa3adaty.Controllers" }
             );

            routes.MapRoute(
                name: "Tag",
                url: "{id}/{page}",
                defaults: new { controller = "Category", action = "ListByTag", page = UrlParameter.Optional },
                constraints: new { page = @"\d*" },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );

            

            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Sa3adaty.Controllers" }
            );
        }
    }
}