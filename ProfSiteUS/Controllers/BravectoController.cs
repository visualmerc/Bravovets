using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfSite.Models;

namespace ProfSiteUS.Controllers
{
    public class BravectoController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var menu = BravectoMenu.CreateBravectoMenu(filterContext.ActionDescriptor.ActionName);
            ViewBag.Menu = menu;
        }


        public ActionResult Home()
        {
            ViewBag.Title = "BRAVECTO: FDA-approved tablet for dogs against fleas and ticks";
            return View();
        }

        public ActionResult Compliance()
        {
            ViewBag.Title = "BRAVECTO: Easier dosing with a palatable flavored chew.";
            return View();
        }

        public ActionResult Innovation()
        {
            ViewBag.Title = "BRAVECTO: Innovative protection from fleas and ticks;";
            return View();
        }
        public ActionResult SocialTips()
        {
            ViewBag.Title = "BRAVECTO: Using social media to grow your practice.";
            return View();
        }


        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult SiteMap()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }  

	}
}