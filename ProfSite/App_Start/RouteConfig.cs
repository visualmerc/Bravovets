using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ProfSite.Infrastructure;

namespace ProfSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // LFW: Add ignore route for ASPX pages
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");

         

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-disclaimer-default", "Disclaimer", new { controller = "Footer", Action = "Disclaimer", menu = "bravovets" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-privacypolicy-default", "PrivacyPolicy", new { controller = "Footer", Action = "PrivacyPolicy", menu = "bravovets" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-sitemap-default", "SiteMap", new { controller = "Footer", Action = "SiteMap", menu = "bravovets" });

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-disclaimer", "{menu}/Disclaimer", new { controller = "Footer" ,Action="Disclaimer" ,menu="bravecto" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-privacypolicy", "{menu}/PrivacyPolicy", new { controller = "Footer", Action = "PrivacyPolicy",menu="bravecto" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-sitemap", "{menu}/SiteMap", new { controller = "Footer", Action = "SiteMap", menu = "bravecto" });


            LanguageRoute.MapSubdomainRoute(routes, "domain-route-account", "logout", new { controller = "Account" ,action="logout" });

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-calendar", "calendar", new { controller = "ShareSocialContent", Action = "SocialCalendar" });

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-sharesocial", "sharesocial/{action}", new { controller = "ShareSocialContent" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-share", "share/{action}", new { controller = "ShareSocialContent" });

            //FeaturedContent
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-syndicatedcontent-featuredimage", "syndicatedcontent/featuredcontent/{contentId}/{type}", new { controller = "SyndicatedContent",action="FeaturedContent" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-syndicatedcontent", "syndicatedcontent/{action}/{contentId}", new { controller = "SyndicatedContent" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-syndicatedcontent-attachment", "syndicatedcontent/{action}/{contentId}/{attachmentId}", new { controller = "SyndicatedContent" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-bravovetswidgets", "bravovetswidgets/{action}", new { controller = "BravoVetsWidgets" });
  

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-facebook", "facebook/{userid}/{action}", new { controller = "Facebook" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-facebook-default", "facebook/{action}", new { controller = "Facebook", action = "OAuthRedirect" });

//            LanguageRoute.MapSubdomainRoute(routes, "domain-route-twitter", "twitter/{action}", new { controller = "Twitter" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-twitter-default", "twitter/{action}", new { controller = "Twitter", action = "OAuthRedirect" });

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-bravecto-testuser", "bravecto/testuserid/{testuserid}/{action}/{id}", new { testuserid = -1, controller = "Bravecto", action = "Home", id = UrlParameter.Optional });

            routes.MapRoute("domain-route-scheduledjobs", "scheduledjob/{action}", new { controller = "ScheduledJob" });

            // PreviewUrl = "/admin/bravectoresources/preview"

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-br-preview", "adminpost/bravectoresources/preview", new { controller = "Admin", action = "BravectoResourcePreview" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-st-preview", "adminpost/socialtips/preview", new { controller = "Admin", action = "SocialTipPreview" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-tr-preview", "adminpost/trendingtopics/preview", new { controller = "Admin", action = "TrendingTopicsPreview" });


            //only doing this to deal with issue with lfw admin posts
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-br-post", "adminpost/posts/bravectoresources/", new { controller = "Admin", action = "BravectoResourcePosts" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-st-post", "adminpost/posts/socialtips/", new { controller = "Admin", action = "SocialTipPosts" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-tr-post", "adminpost/posts/trendingtopics/", new { controller = "Admin", action = "TrendingTopicsPosts" });


            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-br", "admin/posts/bravectoresources/", new { controller = "Admin", action = "BravectoResourcePosts" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-st", "admin/posts/socialtips/", new { controller = "Admin", action = "SocialTipPosts" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-posts-tr", "admin/posts/trendingtopics/", new { controller = "Admin", action = "TrendingTopicsPosts" });



            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-edit-br", "admin/bravectoresources/{clientOffset}/edit/{id}", new { controller = "Admin", action = "BravectoResourcesEdit" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-edit-st", "admin/socialtips/{clientOffset}/edit/{id}", new { controller = "Admin", action = "SocialTipEdit" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-edit-tr", "admin/trendingtopics/{clientOffset}/edit/{id}", new { controller = "Admin", action = "TrendingTopicsEdit" });

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-new-br", "admin/bravectoresources/{clientOffset}/new", new { controller = "Admin", action = "BravectoResourcesEdit" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-new-st", "admin/socialtips/{clientOffset}/new", new { controller = "Admin", action = "SocialTipEdit" });
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin-new-tr", "admin/trendingtopics/{clientOffset}/new", new { controller = "Admin", action = "TrendingTopicsEdit" });



            LanguageRoute.MapSubdomainRoute(routes, "domain-route-admin", "admin/{action}/{id}", new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional });
 

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-bravecto", "bravecto/{action}/{id}", new { controller = "Bravecto", action = "Home", id = UrlParameter.Optional });

            LanguageRoute.MapSubdomainRoute(routes, "domain-route-bravopets", "bravopets/{action}/{id}", new { controller = "bravopets", action = "index", id = UrlParameter.Optional });
            
            LanguageRoute.MapSubdomainRoute(routes, "domain-route-default", "{action}/{id}", new { controller = "Bravovets", action = "SelectCountry", id = UrlParameter.Optional });

        }
    }
}
