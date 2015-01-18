using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ProfSite.Models;
using ProfSite.Utils;

namespace ProfSite.Infrastructure
{
    public class LanguageRoute : Route
    {
        public LanguageRoute(string url)
            : base(url, new MvcRouteHandler())
        {
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData routeData = base.GetRouteData(httpContext);
            if (routeData == null) return null;

            string subdomain = httpContext.Request.Params["subdomain"];
            string host = httpContext.Request.Headers["Host"];

            if (subdomain == null)
            {
                int index = host.IndexOf('.');
                if (index >= 0)
                    subdomain = host.Substring(0, index);
            }

            if (subdomain != null)
                routeData.Values["subdomain"] = subdomain;

            SetLanguage(host, routeData);

            return routeData;
        }


        public static void SetLanguage(string host, RouteData routeData)
        {
            var language = GetLanguage(host);
            routeData.Values["language"] = language;
        }

        public static string GetLanguage(string host)
        {
            var currentLanguage = GetCurrentLanguage(host);
            LanguageHelper.SetCulture(currentLanguage);
            return currentLanguage;
        }

        public static string GetCurrentLanguage(string host)
        {
            
            List<SupportedCountry> supportedCountries = LanguageHelper.SupportedCountries();

            foreach (SupportedCountry supportedCountry in supportedCountries)
            {
//                if (!host.ToLower().Contains(supportedCountry.SiteUrl.ToLower())) continue;
//                if (!supportedCountry.SiteUrl.ToLower().Contains(host.ToLower())) continue;
                string baseSiteUrl = supportedCountry.SiteUrl.ToLower();

                if (baseSiteUrl.EndsWith("/bravecto"))
                {
                    baseSiteUrl = baseSiteUrl.Substring(0, baseSiteUrl.LastIndexOf("/bravecto"));
                }

                if (!host.ToLower().Contains(baseSiteUrl))
                {
                    continue;
                }

                
                return supportedCountry.LocaleCode;
            }
            return "en";
        }

        public static string GetCurrentLanguage()
        {
            return GetCurrentLanguage(HttpContext.Current.Request.Headers["Host"]);
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            object subdomainParam = requestContext.HttpContext.Request.Params["subdomain"];
            if (subdomainParam != null)
                values["subdomain"] = subdomainParam;
            return base.GetVirtualPath(requestContext, values);
        }


        public static void MapSubdomainRoute(RouteCollection routes, string name, string url, object defaults = null,
            object constraints = null)
        {
            routes.Add(name, new LanguageRoute(url)
                             {
                                 Defaults = new RouteValueDictionary(defaults),
                                 Constraints = new RouteValueDictionary(constraints),
                                 DataTokens = new RouteValueDictionary()
                             });
        }
    }
}