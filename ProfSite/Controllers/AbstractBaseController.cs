using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using ProfSite.Utils;

namespace ProfSite.Controllers
{
    using ProfSite.Auth;

    public class AbstractBaseController : Controller
    {

        public readonly List<object[]> MessagesToSend = new List<object[]>();

        internal IBravoVetsAuthManager AuthManager = new BravoVetsAuthManager();        

        protected string GetSubDomain()
        {
            if (!RouteData.Values.ContainsKey("subdomain"))
                return string.Empty;

            return (string) RouteData.Values["subdomain"];
        }

        protected int GetCountryId()
        {
            var language = GetSiteLanguage();
            return (int)LanguageHelper.GetCountryId(language);
        }

        protected int GetCurrentUserId()
        {
            var firstOrDefault = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "BravoVetsUserId");
            if (firstOrDefault == null) return 0;
            var bvuserId = firstOrDefault.Value;

            int userId;
            int.TryParse(bvuserId, out userId);

            return userId;
        }

        public string GetCurrentUserName()
        {
            var userName = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userName == null)
            {
                return "";
            }

            return userName.Value;
        }

        protected void SetUserLanguage()
        {
            string userLanguage = GetUserLanguage();
//            string userLanguage = "it";

            if (RouteData != null)
                RouteData.Values["language"] = userLanguage;

            LanguageHelper.SetCulture(userLanguage);
        }

        public string GetUserLanguage()
        {
            //if (Request == null || Request.UserLanguages == null)
                return Thread.CurrentThread.CurrentCulture.Name;
           

            //return Request.UserLanguages[0];
        }


        protected string GetSiteLanguage()
        {
            if (RouteData != null && RouteData.Values.ContainsKey("language"))
                return (string) RouteData.Values["language"];

            return "en";

        }

        protected string GetSiteFullLanguage() {
            var language = this.GetSiteLanguage();
            var fullLanguage = string.Empty;

            switch (language)
            {
                case "de":
                    fullLanguage = "de-DE";
                    break;
                case "fr":
                    fullLanguage = "fr-FR";
                    break;
                case "nl":
                    fullLanguage = "nl-NL";
                    break;
                case "it":
                    fullLanguage = "it-IT";
                    break;
                case "es":
                    fullLanguage = "es-ES";
                    break;
                case "pt":
                    fullLanguage = "pt-PT";
                    break;
                case "ru":
                    fullLanguage = "ru-RU";

                    break;
                case "es-ES":
                    fullLanguage = "es-ES";
                    break;
                default:
                    fullLanguage = "en-US";
                    break;
            }

            return fullLanguage;
        }
        protected void SendLater(params object[] msgs)
        {
            MessagesToSend.Add(msgs);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {


        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        #region private methods

        #endregion
    }
}