using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BravoVets.DomainObject;
using Microsoft.Ajax.Utilities;
using ProfSite.Models;
using ProfSite.Utils;

namespace ProfSite.Controllers
{
    public class FacebookController : AbstractBaseSocialController
    {
        public RedirectResult OAuthRedirect()
        {
            string url = FacebookHelper.CreateAuthUrl(GetSiteLanguage());

            return new RedirectResult(url);
        }

        public BravectoMenu CreateMenu(string activeItem)
        {
            var menu = new BravectoMenu { MenuItems = new List<BravectoMenuItem>(), ActiveItem = activeItem };
            menu.MenuItems.Add(new BravectoMenuItem { Code = "resourcetopics", Display = "Bravecto Resources", Url = "/resourcetopics" });
            menu.MenuItems.Add(new BravectoMenuItem { Code = "trendingtopics", Display = "Trending Topics", Url = "/trendingtopics" });
            menu.MenuItems.Add(new BravectoMenuItem { Code = "socialtips", Display = "Social Tips", Url = "/socialtips" });
            return menu;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var menu = CreateMenu(filterContext.ActionDescriptor.ActionName);
            ViewBag.Menu = menu;
            ViewBag.Language = this.GetSiteFullLanguage();
        }

        public ActionResult oauth_callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return new RedirectResult("/EditProfile");

            FacebookCodeExchange response = FacebookHelper.ExchangeCode(code, GetSiteLanguage());

            string accessToken = response.access_token;
            string fbUserId = response.userid;

            int userId = GetCurrentUserId();
            BravoVetsUser currentUser = vetUserDomainService.GetBravoVetsUser(userId);

            List<FacebookPage> pages = FacebookHelper.GetFacebookPages(accessToken, fbUserId, GetSiteLanguage());

            vetUserDomainService.SaveFacebookTokens(currentUser, code, accessToken, fbUserId, string.Empty, string.Empty, 0);

            if (pages.Count.Equals(0))
            {
                return new RedirectResult("/EditProfile");
            }

            pages.Insert(0, new FacebookPage { name = "Primary account" });

            return View("SelectManagedPage", pages);
        }

        public ActionResult SaveManagedAccount(string pageId, string pageToken)
        {
            if (string.IsNullOrEmpty(pageId) || string.IsNullOrEmpty(pageToken))
                return new RedirectResult("/EditProfile");

            VeterinarianSocialIntegration socialInfo = GetFacebookVetSocialIntegration();

            int userId = GetCurrentUserId();
            BravoVetsUser currentUser = vetUserDomainService.GetBravoVetsUser(userId);
            socialInfo.PageId = pageId;
            socialInfo.PageAccessToken = pageToken;
            var profile = FacebookHelper.GetProfile(socialInfo);
          
            vetUserDomainService.SaveFacebookTokens(currentUser, socialInfo.AccessCode, socialInfo.AccessToken, socialInfo.AccountName, pageId, pageToken, profile.likes);

            return new RedirectResult("/EditProfile");
        }


        public ActionResult Timeline(string until)
        {
            VeterinarianSocialIntegration socialInfo = GetFacebookVetSocialIntegration();

            if (socialInfo == null || string.IsNullOrEmpty(socialInfo.AccessToken))
            {
                return PartialView("NoLinkedAccount", new FacebookTimeline { AccountLinked = false });
            }

            FacebookHomeModel model = FacebookHelper.GetTimeline(socialInfo, until);

            var posts = new List<FacebookTimelinePost>();
            if (model.data != null)
            {
                foreach (FacebookHomeItemModel item in model.data)
                {
                    if (item.type == "status" && string.IsNullOrEmpty(item.message))
                        continue;

                    posts.Add(new FacebookTimelinePost(item));
                }
            }

            return PartialView("Timeline", new FacebookTimeline { AccountLinked = true, Posts = posts, NextPage = model.paging == null ? string.Empty: model.paging.getuntil()});
        }


        public ActionResult LikePost(string postId)
        {
            VeterinarianSocialIntegration socialInfo = GetFacebookVetSocialIntegration();

            if (socialInfo == null || string.IsNullOrEmpty(socialInfo.AccessToken))
            {
                return PartialView("Timeline", new FacebookTimeline { AccountLinked = false });
            }

            FacebookHelper.LikePost(postId, socialInfo);

            return Json(new FacebookActionModel { Status = "Success" });
        }

        public ActionResult CommentPost(string postId, string message)
        {
            VeterinarianSocialIntegration socialInfo = GetFacebookVetSocialIntegration();

            if (socialInfo == null || string.IsNullOrEmpty(socialInfo.AccessToken))
            {
                return PartialView("Timeline", new FacebookTimeline { AccountLinked = false });
            }

            FacebookComment response = FacebookHelper.CommentPost(postId, message, socialInfo);

            var comment = new FacebookTimelineComment(response);
            return PartialView("Comment", comment);
        }

        public ActionResult Delete()
        {

            var userId = GetCurrentUserId();
            vetUserDomainService.DeleteFacebookIntegration(userId);

            return Redirect("/EditProfile");
        }

    }
}