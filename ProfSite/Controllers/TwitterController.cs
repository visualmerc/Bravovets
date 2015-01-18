using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using BravoVets.DomainObject;
using BravoVets.DomainService.Service;
using log4net;
using log4net.Util;
using Newtonsoft.Json;
using ProfSite.Models;
using ProfSite.Utils;
using WebGrease;

namespace ProfSite.Controllers
{
    public class TwitterController : AbstractBaseSocialController
    {


        public RedirectResult OAuthRedirect()
        {
            var requestToken = TwitterHelper.GetUserRequestToken(GetSiteLanguage());

            var url = TwitterHelper.CreateoAuthRedirectUrl(requestToken);

            return Redirect(url);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
            ViewBag.Language = this.GetSiteFullLanguage();
        }

        public ActionResult oauth_callback(string oauth_token, string oauth_verifier)
        {
            if (string.IsNullOrEmpty(oauth_verifier))
                return Redirect("/EditProfile");

            var results = TwitterHelper.GetUserAccessToken(oauth_token, oauth_verifier, GetSiteLanguage());

            if (results == null || string.IsNullOrEmpty(results.token_secret))
                return Redirect("/EditProfile");

            var userId = GetCurrentUserId();

            BravoVetsUser currentUser = vetUserDomainService.GetBravoVetsUser(userId);

            var profile = TwitterHelper.GetProfile(results.access_token, results.token_secret, results.user_id, GetSiteLanguage());


            vetUserDomainService.SaveTwitterTokens(currentUser, results.access_token, results.token_secret, results.user_id, profile.followers_count);

            return Redirect("/EditProfile");
        }

        public ActionResult Timeline(string maxId)
        {
            try
            {
                var twitterInfo = GetTwitterVetSocialIntegration();

                if (twitterInfo == null || string.IsNullOrEmpty(twitterInfo.AccessToken))
                {
                    return PartialView("NoLinkedAccount", new TwitterTimeline { AccountLinked = false });
                }

                var accessCode = twitterInfo.AccessCode;
                var accessToken = twitterInfo.AccessToken;

                var success = false;

                var response = TwitterHelper.GetHomeTimeline(accessCode, accessToken, maxId, GetSiteLanguage(), out success);

                if (!success && !string.IsNullOrEmpty(twitterInfo.LastFeed))
                {
                    response = twitterInfo.LastFeed;
                }
                else if (success)
                {
                    twitterInfo.LastFeed = response;
                    vetUserDomainService.UpdateVeterinarianSocialIntegration(twitterInfo);
                }

                var timelineResponse = JsonConvert.DeserializeObject<List<TwitterStatusModel>>(response);

                var tweets = new List<TweetModel>();

                var lastId = string.Empty;

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

                return PartialView("Timeline", new TwitterTimeline { AccountLinked = true, Tweets = tweets, LastId = lastId });
            }
            catch (Exception ex)
            {
                Logger.Error("Twitter Timeline", ex);
                return PartialView("TimelineError");
            }
        }

        public ActionResult Reply(string postId, string replyText)
        {
            var twitterInfo = GetTwitterVetSocialIntegration();

            if (twitterInfo == null || string.IsNullOrEmpty(twitterInfo.AccessToken))
            {
                return PartialView("Timeline", new TwitterTimeline { AccountLinked = false });
            }

            var accessCode = twitterInfo.AccessCode;
            var accessToken = twitterInfo.AccessToken;

            var response = TwitterHelper.Reply(accessCode, accessToken, postId, replyText, GetSiteLanguage());

            return PartialView("Tweet", new TweetModel(response));

        }

        public ActionResult Retweet(string postId)
        {
            var twitterInfo = GetTwitterVetSocialIntegration();

            if (twitterInfo == null || string.IsNullOrEmpty(twitterInfo.AccessToken))
            {
                return PartialView("Timeline", new TwitterTimeline { AccountLinked = false });
            }

            var accessCode = twitterInfo.AccessCode;
            var accessToken = twitterInfo.AccessToken;

            TwitterHelper.Retweet(accessCode, accessToken, postId, GetSiteLanguage());

            return Json(new TwitterActionModel { Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete()
        {

            var userId = GetCurrentUserId();
            vetUserDomainService.DeleteTwitterIntegration(userId);

            return Redirect("/EditProfile");
        }


    }
}