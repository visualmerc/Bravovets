using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ProfSite.Utils;

namespace ProfSite.Infrastructure
{
  
    public class GlobalizationViewEngine : RazorViewEngine
    {
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            partialPath = GlobalizeViewPath(controllerContext, partialPath);
         //   return new WebFormView(controllerContext, partialPath, null, ViewPageActivator);
            return base.CreatePartialView(controllerContext, partialPath);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            viewPath = GlobalizeViewPath(controllerContext, viewPath);
            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        private static string GetSiteLanguage(ControllerContext controllerContext)
        {
            if (!controllerContext.RouteData.Values.ContainsKey("language"))
                return null;

            return (string)controllerContext.RouteData.Values["language"];
        }

        private static string GlobalizeViewPath(ControllerContext controllerContext, string viewPath)
        {
            HttpRequestBase request = controllerContext.HttpContext.Request;

            string language = GetSiteLanguage(controllerContext);

            if (!string.IsNullOrEmpty(language))
            {
                LanguageHelper.SetCulture(language);
            }
            else if (request.UserLanguages != null)
            {
                language = request.UserLanguages[0];
            }

            if (language == null || string.IsNullOrEmpty(language) ||
                string.Equals(language, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                return viewPath;
            }

            var localizedViewPath = Regex.Replace(
                viewPath, "^~/Views/",
                string.Format("~/Views/Globalization/{0}/", language));

            if (File.Exists(request.MapPath(localizedViewPath)))
            {
                viewPath = localizedViewPath;
            }
            return viewPath;
        }
    }
}