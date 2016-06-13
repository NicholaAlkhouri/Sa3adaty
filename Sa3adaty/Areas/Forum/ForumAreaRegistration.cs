using System.Web.Mvc;
using LowercaseRoutesMVC;
using MVCForum.Domain.Constants;

namespace Sa3adaty.Areas.Forum
{
    public class ForumAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Forum";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {


            context.MapRoute(
               name: "ForumHome",
               url: "المنتدى",
               defaults: new { controller = "ForumHome", action = "Index", Area = "Forum" },
               namespaces: new[] { "MVCForum.Website.Controllers" }
           );

            context.MapRoute(
               "Forum_Favorite",
               "المنتدى/المفضلة/{action}/{id}",
               new { action = "Index", controller = "Favourite", id = UrlParameter.Optional, }, null, namespaces: new[] { "MVCForum.Website.Controllers" }
           );


            context.MapRoute(
            "Forum_AllCategory",
            "المنتدى/تصنيفات",
            new { action = "Index", controller = "Category" }, null, namespaces: new[] { "MVCForum.Website.Controllers" }
        );
            context.MapRouteLowercase(
                "categoryUrls", // Route name
                string.Concat("المنتدى/", AppConstants.CategoryUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Category", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
                , namespaces: new[] { "MVCForum.Website.Controllers" }
            );

            context.MapRouteLowercase(
                "categoryRssUrls", // Route name
                string.Concat("المنتدى/", AppConstants.CategoryUrlIdentifier, "/rss/{slug}"), // URL with parameters
                new { controller = "Category", action = "CategoryRss", slug = UrlParameter.Optional } // Parameter defaults
                , namespaces: new[] { "MVCForum.Website.Controllers" }
            );
            context.MapRoute(
              "Forum_Category",
              "المنتدى/تصنيفات/{action}/{id}",
              new { action = "Index", controller = "Category", id = UrlParameter.Optional, }, null, namespaces: new[] { "MVCForum.Website.Controllers" }
          );
            context.MapRouteLowercase(
                "topicUrls", // Route name
                string.Concat("المنتدى/", AppConstants.TopicUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Topic", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
                , namespaces: new[] { "MVCForum.Website.Controllers" }
            );

            context.MapRouteLowercase(
                "memberUrls", // Route name
                string.Concat("المنتدى/", AppConstants.MemberUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Members", action = "GetByName", slug = UrlParameter.Optional } // Parameter defaults
                , namespaces: new[] { "MVCForum.Website.Controllers" }
            );

            context.MapRouteLowercase(
                "tagUrls", // Route name
                string.Concat("المنتدى/",AppConstants.TagsUrlIdentifier, "/{tag}"), // URL with parameters
                new { controller = "Topic", action = "TopicsByTag", tag = UrlParameter.Optional } // Parameter defaults
                , namespaces: new[] { "MVCForum.Website.Controllers" }
            );

            context.MapRoute(
                "Forum_default",
                "Forum/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }, null,namespaces: new[] { "MVCForum.Website.Controllers" }
            );
        }
    }
}
