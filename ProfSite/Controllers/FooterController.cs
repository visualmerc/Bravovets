using System.Web.Mvc;
using ProfSite.Models;
using ProfSite.Resources;

namespace ProfSite.Controllers
{
    public class FooterController : AbstractBaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            ViewBag.Language = this.GetSiteFullLanguage();
        }

        public ActionResult Disclaimer(string menu)
        {
            ViewBag.Title = Resource.Disclaimer_Title;
            if (string.IsNullOrEmpty(menu))
                return View();

            CreateMenu(menu);
            return View();
        }

        public ActionResult PrivacyPolicy(string menu)
        {
            ViewBag.Title = Resource.Privacy_Title;
            if (string.IsNullOrEmpty(menu))
                return View();

            CreateMenu(menu);

            return View();
        }

        public ActionResult SiteMap(string menu)
        {
            ViewBag.Title = Resource.Footer_SiteMap_Title;
            if (string.IsNullOrEmpty(menu))
                return View();

            CreateMenu(menu);

            return View();
        }

        private void CreateMenu(string targetMenu)
        {
            BravectoMenu menu = null;

            var userName = GetCurrentUserName();
            if (string.IsNullOrEmpty(userName))
            {
                targetMenu = "bravecto";
            }

            menu = targetMenu == "bravecto" ? CreateBravectoMenu() : CreateBravoVetsMenu();

            ViewBag.Menu = menu;
        }

        private BravectoMenu CreateBravectoMenu()
        {
            var menu = BravectoMenu.CreateBravectoMenu(string.Empty);

            var userName = GetCurrentUserName();

            if (string.IsNullOrEmpty(userName))
            {
                menu.Layout = "_Layout_Bravecto_Public";
            }

            ViewBag.Menu = menu;
            return menu;
        }

        private BravectoMenu CreateBravoVetsMenu()
        {
            var menu = BravectoMenu.CreateBravovetsMenu(string.Empty);

            menu.Layout = "_Layout";

            return menu;
        }
    }
}