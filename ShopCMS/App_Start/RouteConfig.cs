using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ahmadi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {


            routes.IgnoreRoute("elmah");
            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
            new { botdetect = @"(.*) BotDetectCaptcha\.ashx" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(name: "file_details", url: "Tf-products/{f}/{w}/{h}/{q}", defaults: new { controller = "Thumbnail", action = "Index",f=UrlParameter.Optional, w = UrlParameter.Optional, h = UrlParameter.Optional, q = UrlParameter.Optional }, namespaces: new string[] { "ahmadi.Controllers" });

            routes.MapRoute(name: "Blog_details", url: "blog", defaults: new { controller = "contentType", action = "Index", id = 1 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Pages_details", url: "pages", defaults: new { controller = "contentType", action = "Index", id = 0 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "news_details", url: "Projects", defaults: new { controller = "contentType", action = "Index", id = 3 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "services_details", url: "services", defaults: new { controller = "contentType", action = "Index", id = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Mservices_details", url: "mservices", defaults: new { controller = "contentType", action = "Index", id = 11 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Videos_details", url: "videos", defaults: new { controller = "AllVideo", action = "Index", id = 5 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Team_details", url: "team", defaults: new { controller = "contentType", action = "Index", id = 13 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "contentType_details", url: "contentType/{id}/{title}", defaults: new { controller = "contentType", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });

            routes.MapRoute(name: "content_details", url: "content/{id}/{title}", defaults: new { controller = "content", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "video_details", url: "Video/{id}/{title}", defaults: new { controller = "video", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Page_details", url: "Page/{id}/{title}", defaults: new { controller = "Page", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
          
            routes.MapRoute(name: "category_details", url: "Category/{id}/{title}", defaults: new { controller = "category", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Videocategory_details", url: "VideoCategory/{id}/{title}", defaults: new { controller = "VideoCategory", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            
            routes.MapRoute(name: "tag_details", url: "Tag/{id}/{title}", defaults: new { controller = "tag", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "contentTag_details", url: "contentTag/{contentTypeId}/{id}/{title}", defaults: new { controller = "contentTag", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });

            routes.MapRoute(name: "SearchResult_ProductCategory_details", url: "SearchResult/{id}/{title}/{page}", defaults: new { controller = "SearchResult", action = "Index", id = UrlParameter.Optional, title = UrlParameter.Optional, page = UrlParameter.Optional }, namespaces: new string[] { "ahmadi.Controllers" });
            
            routes.MapRoute(name: "Ads_details", url: "Ads/{id}", defaults: new { controller = "Ads", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            
            routes.MapRoute(name: "404Page", url: "NotFound-404", defaults: new { controller = "Error", action = "Index" }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "500Page", url: "Error-500", defaults: new { controller = "Error", action = "Index2" }, namespaces: new string[] { "ahmadi.Controllers" });

            routes.MapRoute(name: "En-Home", url: "En", defaults: new { controller = "Home", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "En-Content", url: "En/content/{id}/{title}", defaults: new { controller = "content", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "video_details_en", url: "En/Video/{id}/{title}", defaults: new { controller = "video", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Page_details_en", url: "En/Page/{id}/{title}", defaults: new { controller = "Page", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });

            routes.MapRoute(name: "Blog_detailsEn", url: "En/blog", defaults: new { controller = "contentType", action = "Index", id = 8, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Pages_detailsEn", url: "En/pages", defaults: new { controller = "contentType", action = "Index", id = 9, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "news_detailsEn", url: "En/Projects", defaults: new { controller = "contentType", action = "Index", id = 7, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "services_detailsEn", url: "En/services", defaults: new { controller = "contentType", action = "Index", id = 6, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Mservices_detailsEn", url: "En/mservices", defaults: new { controller = "contentType", action = "Index", id = 12, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Videos_detailsEn", url: "En/videos", defaults: new { controller = "AllVideo", action = "Index", id = 10, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Team_details_En", url: "En/team", defaults: new { controller = "contentType", action = "Index", id = 14, langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "contentType_detailsEn", url: "contentType/{id}/{title}", defaults: new { controller = "contentType", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });

            routes.MapRoute(name: "category_details_en", url: "En/Category/{id}/{title}", defaults: new { controller = "category", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "Videocategory_details_en", url: "En/VideoCategory/{id}/{title}", defaults: new { controller = "VideoCategory", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });


            routes.MapRoute(name: "tag_details_en", url: "En/Tag/{id}/{title}", defaults: new { controller = "tag", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });
            routes.MapRoute(name: "contentTag_details_en", url: "En/contentTag/{contentTypeId}/{id}/{title}", defaults: new { controller = "contentTag", action = "Index", langid = 2 }, namespaces: new string[] { "ahmadi.Controllers" });


            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}/{title}/{page}",
              defaults: new { controller = "home", action = "Index", id = UrlParameter.Optional, title = UrlParameter.Optional, page = UrlParameter.Optional },
              namespaces: new string[] { "ahmadi.Controllers" }
          );
        }
    }
}
