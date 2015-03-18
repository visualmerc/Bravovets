using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ProfSite.Models;
using ProfSite.Resources;
using BravoVets.DomainService.Service;
using ProfSite.Utils;
using System.Linq;
using Newtonsoft.Json;

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
            var model = new FacebookViewModel();
            var userDomainService = new BravoVetsUserDomainService();
            var userId = this.GetCurrentUserId();

            var user = userDomainService.GetBravoVetsUserForProfileEdit(userId);

            model.IsFacebookLinked = user.Veterinarian.IsFacebookLinked;

            if (model.IsFacebookLinked)
            {
                var facebookInfo = userDomainService.GetFacebookSocialIntegration(user);
                var timeline = FacebookHelper.GetTimeline(facebookInfo, "");
                model.UserName = facebookInfo.AccountName;

                var posts = new List<FacebookTimelinePost>();
                if (timeline.data != null)
                {
                    foreach (FacebookHomeItemModel item in timeline.data)
                    {
                        if (item.type == "status" && string.IsNullOrEmpty(item.message))
                            continue;

                        posts.Add(new FacebookTimelinePost(item));
                    }
                }
                model.Posts = posts;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Twitter()
        {
            var model = new TwitterViewModel();
            var userId = this.GetCurrentUserId();
            var userDomainService = new BravoVetsUserDomainService();
            var user = userDomainService.GetBravoVetsUserForProfileEdit(userId);

            model.IsTwitterLinked = user.Veterinarian.IsTwitterLinked;

            if (model.IsTwitterLinked)
            {
                var twitterInfo = userDomainService.GetTwitterSocialIntegration(user);

                var twitterAccount = TwitterHelper.GetProfile(twitterInfo.AccessCode,
                    twitterInfo.AccessToken, twitterInfo.AccountName, GetSiteLanguage());

                model.UserName = twitterAccount.screen_name;
            }
            return View(model);
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

        [HttpGet]
        [ActionName("linked-accounts")]
        public ActionResult LinkedAccounts()
        {
            var model = new LinkedAccountsViewModel();

            //TODO: Check for authentication
            var userId = this.GetCurrentUserId();
            var userDomainService = new BravoVetsUserDomainService();
            var user = userDomainService.GetBravoVetsUserForProfileEdit(userId);

            model.IsFacebookLinked = user.Veterinarian.IsFacebookLinked;
            model.IsTwitterLinked = user.Veterinarian.IsTwitterLinked;

            if (model.IsTwitterLinked)
            {
                var twitterInfo = userDomainService.GetTwitterSocialIntegration(user);

                var twitterAccount = TwitterHelper.GetProfile(twitterInfo.AccessCode,
                    twitterInfo.AccessToken, twitterInfo.AccountName, GetSiteLanguage());

                model.TwitterUserName = twitterAccount.screen_name;

                var success = false;
                var response = TwitterHelper.GetHomeTimeline(twitterInfo.AccessCode, twitterInfo.AccessToken, "", GetSiteLanguage(), out success);
                var timelineResponse = JsonConvert.DeserializeObject<List<TwitterStatusModel>>(response);
                var tweets = new List<TweetModel>();
                var lastId = string.Empty;
                if (timelineResponse != null)
                {
                    foreach (var twitterStatusModel in timelineResponse)
                    {
                        var item = new TweetModel(twitterStatusModel);
                        if (twitterInfo.AccountName == item.user.id)
                        {
                            item.CanRetweet = false;
                        }
                        tweets.Add(item);
                        lastId = twitterStatusModel.id;
                    }
                }
                model.Tweets = tweets;
            }

            if (model.IsFacebookLinked)
            {
                var facebookInfo = userDomainService.GetFacebookSocialIntegration(user);
                var timeline = FacebookHelper.GetTimeline(facebookInfo, "");

                var posts = new List<FacebookTimelinePost>();
                if (timeline.data != null)
                {
                    foreach (FacebookHomeItemModel item in timeline.data)
                    {
                        if (item.type == "status" && string.IsNullOrEmpty(item.message))
                            continue;

                        posts.Add(new FacebookTimelinePost(item));
                    }
                }
                model.Posts = posts;
            }
            return View(model);
        }

        [HttpGet]
        [ActionName("unlinked-accounts")]
        public ActionResult UnLinkedAccounts()
        {
            return View();
        }

        [HttpGet]
        [ActionName("contact-us")]
        public ActionResult ContactUs()
        {
            ViewBag.Countries = LanguageHelper.SupportedCountries().Select(c => new SelectListItem
            {
                Value = c.Code,
                Text = c.DisplayName
            });
            return View();
        }

        [HttpGet]
        public ActionResult Support()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Resources()
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