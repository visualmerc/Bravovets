using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ProfSite.Models;
using ProfSite.Resources;

namespace ProfSite.Controllers
{
    public class BravectoController : AbstractBaseController
    {
       

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var menu = BravectoMenu.CreateBravectoMenu(filterContext.ActionDescriptor.ActionName);

            var userName = GetCurrentUserName();

            if (string.IsNullOrEmpty(userName))
            {
                menu.Layout = "_Layout_Bravecto_Public";
            }

            ViewBag.Menu = menu;
            ViewBag.Language = this.GetSiteFullLanguage();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View();
        }

        [HttpGet]
        [ActionName("about-us")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Facebook()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Twitter()
        {
            return View();
        }

        [HttpGet]
        [ActionName("social-content")]
        public ActionResult SocialContent()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Products()
        {
            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Title = Resource.Bravecto_Homepage_Title;

            // Method for setting test user values
            int testUserId;
            string testuseridFromRoute = NullRouteInfoCheck("testuserid");
            if (Int32.TryParse(testuseridFromRoute, out testUserId))
            {
                if (testUserId > 0)
                {
                    Session["TestUserId"] = testUserId;
                }
            }


            return View("Home");
        }

        public ActionResult Innovation()
        {
            ViewBag.Title = Resource.Bravecto_Innovation_Title;
            return View("Innovation");
        }

        public ActionResult Compliance()
        {
            ViewBag.Title = Resource.Bravecto_Compliance_Title;
            return View("Compliance");
        }

        public ActionResult NewBusiness()
        {

            ViewBag.Title = Resource.Bravecto_NewBusiness_Title;
            return View("NewBusiness");
        }


        private string NullRouteInfoCheck(string key)
        {
            //RouteData is null only during unit tests
            if (RouteData == null || RouteData.Values[key] == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(RouteData.Values[key].ToString()))
            {
                return string.Empty;
            }
            return RouteData.Values[key].ToString();
        }

       
    }
}