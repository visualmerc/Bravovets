using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.WebPages;
using BravoVets.DomainObject;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Newtonsoft.Json;
using ProfSite.Models;

namespace ProfSite.Utils
{
    public static class FacebookHelper
    {
        public const string facebook_feed_post_status = "{0}/{1}/feed?access_token={2}&message={3}";
        public const string facebook_feed_post_link_status = "{0}/{1}/feed?access_token={2}&message={3}&link={4}";
        public const string facebook_feed_url = "{0}/{1}/feed?limit={2}&access_token={3}";
        public const string facebook_home_url = "{0}/{1}/home?limit={2}&access_token={3}";
        public const string facebook_like_post_url = "{0}/{1}/likes?access_token={2}";
        public const string facebook_comment_post_url = "{0}/{1}/comments?message={2}&access_token={3}";
        public const string facebook_comment_get_url = "{0}/{1}?access_token={2}";
        public const string facebook_profile_url = "{0}/{1}?access_token={2}";
        public const string facebook_me_url = "{0}/me?access_token={1}";

        public const string facebook_post_photo_url = "{0}/{1}/photos?access_token={2}";

        public const string facebook_oauth_url =
            "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}";

        public const string facebook_scope =
            "email,read_stream,user_likes,user_videos,user_status,user_friends,status_update,share_item,friends_status,manage_notifications,publish_actions,manage_pages";

        public static string facebook_graph_api = "https://graph.facebook.com";
        public static string facebook_pages_url = "{0}/{1}/accounts?access_token={2}";

        public static string facebook_exchangetoken =
            "{0}/oauth/access_token?client_id={1}&redirect_uri={2}&client_secret={3}&code={4}";

        private static string appid(string lang)
        {
            string key = string.Format("facebook.{0}.appid", lang);
            return ConfigurationManager.AppSettings[key];
        }

        private static string appSecret(string lang)
        {
            string key = string.Format("facebook.{0}.appSecret", lang);
            return ConfigurationManager.AppSettings[key];
        }

        private static string redirectUrl(string lang)
        {
            string key = string.Format("facebook.{0}.redirectUrl", lang);
            return ConfigurationManager.AppSettings[key];
        }


        public static string CreateAuthUrl(string lang)
        {
            return string.Format(facebook_oauth_url, appid(lang), redirectUrl(lang), facebook_scope);
        }


        public static FacebookCodeExchange ExchangeCode(string code, string lang)
        {
            string url = string.Format(facebook_exchangetoken, facebook_graph_api, appid(lang), redirectUrl(lang),
                appSecret(lang), code);

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            NameValueCollection parsedParams = HttpUtility.ParseQueryString(response);

            string accessToken = parsedParams.Get("access_token");

            FacebookProfile profile = GetId(accessToken);
            string userid = profile.id;

            long expires = 0;
            long.TryParse(parsedParams.Get("expires"), out expires);

            return new FacebookCodeExchange { access_token = accessToken, expires = expires, userid = userid };
        }


        public static List<FacebookPage> GetFacebookPages(string accessToken, string userId, string lang)
        {
            string url = string.Format(facebook_pages_url, facebook_graph_api, userId, accessToken);

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var facebookPageResult = JsonConvert.DeserializeObject<FacebookPages>(response);

            var pages = new List<FacebookPage>();

            if (facebookPageResult == null || facebookPageResult.data == null)
                return pages;


            foreach (FacebookPage page in facebookPageResult.data)
            {
                pages.Add(page);
            }

            return pages;
        }

        public static HttpWebRequest CreateWebRequest(string requestUrl, string requestMethod, dynamic parameters)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);

            httpWebRequest.Method = requestMethod;
            httpWebRequest.UseDefaultCredentials = true;
            if (parameters != null)
            {
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                string data = JsonConvert.SerializeObject(parameters);

                var stream = new StreamWriter(httpWebRequest.GetRequestStream());
                stream.Write(data);
                stream.Close();
            }
            return httpWebRequest;
        }

        public static string GetAccessToken(VeterinarianSocialIntegration socialIntegration)
        {
            if (string.IsNullOrEmpty(socialIntegration.PageAccessToken))
                return socialIntegration.AccessToken;

            return socialIntegration.PageAccessToken;
        }

        public static string GetId(VeterinarianSocialIntegration socialIntegration)
        {
            if (string.IsNullOrEmpty(socialIntegration.PageId))
                return socialIntegration.AccountName;

            return socialIntegration.PageId;
        }

        private static string GetTimelineUrl(VeterinarianSocialIntegration socialIntegration)
        {
            if (string.IsNullOrEmpty(socialIntegration.PageAccessToken))
                return facebook_home_url;

            return facebook_feed_url;
        }

        public static FacebookHomeModel GetTimeline(VeterinarianSocialIntegration socialIntegration, string nextPage)
        {
            string url = nextPage;

            string accessToken = GetAccessToken(socialIntegration);

            string id = GetId(socialIntegration);

            string timeLineUrl = GetTimelineUrl(socialIntegration);

            url = string.Format(timeLineUrl, facebook_graph_api, id, 5, accessToken);

            if (!string.IsNullOrEmpty(nextPage))
            {
                url += string.Format("&until={0}", nextPage);
            }

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var homeModel = JsonConvert.DeserializeObject<FacebookHomeModel>(response);
            var postIdsWithAttachments = new List<string>();

            foreach (var facebookHomeItemModel in homeModel.data)
            {
                if (facebookHomeItemModel.type == "photo")
                {
                    postIdsWithAttachments.Add(string.Format("'{0}'",facebookHomeItemModel.id));
                }
            }

            var attachments = GetPostAttachments(id, accessToken, postIdsWithAttachments);
            if (attachments == null)
            {
                return homeModel;
            }

            foreach (var post in attachments.data)
            {
                var fbPost = homeModel.data.Find(x => x.id == post.post_id);
                if (fbPost == null)
                {
                    continue;
                }

                fbPost.attachments = post.attachment.media;
            }

            return homeModel;
        }

        public static FacebookPostAttachments GetPostAttachments(string accountId, string accessToken, List<string> postIds)
        {
            var url = string.Format("{0}/fql", facebook_graph_api);

            var query = string.Format("q=SELECT post_id,attachment FROM stream WHERE source_id = {0} AND post_id in({1})",
                accountId, string.Join(",", postIds));
            url += "?" +  query + "&access_token=" + accessToken;

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);
            if (!success)
            {
                return null;
            }
            var attachments = JsonConvert.DeserializeObject<FacebookPostAttachments>(response);

            return attachments;
        }


        public static string PostStatus(string accountId, string accessToken, string message)
        {
            string url = string.Format(facebook_feed_post_status, facebook_graph_api, accountId, accessToken, message);
            HttpWebRequest webRequest = CreateWebRequest(url, "POST", null);

            bool success;

            string jsonResponse = WebRequestHelper.GetWebResponse(webRequest, out success);

            var response = JsonConvert.DeserializeObject<FacebookPostResponse>(jsonResponse);

            return response.id;
        }

        public static string PostLink(string accountId, string accessToken, string message, string linkUrl)
        {
            string url = string.Format(facebook_feed_post_link_status, facebook_graph_api, accountId, accessToken, message, linkUrl);

            HttpWebRequest webRequest = CreateWebRequest(url, "POST", null);

            bool success;

            string jsonResponse = WebRequestHelper.GetWebResponse(webRequest, out success);

            var response = JsonConvert.DeserializeObject<FacebookPostResponse>(jsonResponse);

            return response.id;
        }

        public static void PostImages(string accountId, string accessToken, string message, List<QueueContentAttachment> attachments)
        {

            string baseURL = string.Format(facebook_post_photo_url, facebook_graph_api, accountId, accessToken);
            baseURL += "&no_story=true";
            var imageId = string.Empty;
            foreach (var queueContentAttachment in attachments)
            {

                var url = baseURL;

                string boundary = DateTime.UtcNow.Ticks.ToString("x");

                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;

                IAsyncResult streamAsyncResult = request.BeginGetRequestStream(null, null);
                using (Stream s = request.EndGetRequestStream(streamAsyncResult))
                {
                    var writer = new MultipartWriter(s, boundary);

                    writer.WriteSeperator().WriteBoundary().WriteNewline();
                    writer.Write("Content-Disposition: form-data; filename=\"" + "source" + "\"").WriteNewline();
                    writer.Write("Content-Type: image/jpeg").WriteNewline();
                    writer.WriteNewline();
                    writer.Write(queueContentAttachment.AttachmentFile);

                    writer.WriteNewline().WriteSeperator().WriteBoundary().WriteSeperator().WriteNewline();
                }

                bool success;
                var response = WebRequestHelper.GetWebResponse(request, out success);
                imageId = JsonConvert.DeserializeObject<FacebookPostResponse>(response).id;
            }

            var linkUrl =
                string.Format("https://www.facebook.com/{0}/photos/{1}/?type=1&relevant_count=2", accountId, imageId);

            PostLink(accountId, accessToken, message, linkUrl);
        }

        public static void LikePost(string postId, VeterinarianSocialIntegration socialIntegration)
        {
            string accessToken = GetAccessToken(socialIntegration);

            string url = string.Format(facebook_like_post_url, facebook_graph_api, postId, accessToken);

            HttpWebRequest webRequest = CreateWebRequest(url, "POST", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);
        }

        public static FacebookComment CommentPost(string postId, string message,
            VeterinarianSocialIntegration socialIntegration)
        {
            string accessToken = GetAccessToken(socialIntegration);

            string url = string.Format(facebook_comment_post_url, facebook_graph_api, postId, message, accessToken);

            HttpWebRequest webRequest = CreateWebRequest(url, "POST", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var comment = JsonConvert.DeserializeObject<FacebookComment>(response);

            return GetComment(comment.id, socialIntegration);
        }

        public static FacebookComment GetComment(string commentId, VeterinarianSocialIntegration socialIntegration)
        {
            string accessToken = GetAccessToken(socialIntegration);

            string url = string.Format(facebook_comment_get_url, facebook_graph_api, commentId, accessToken);

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var comment = JsonConvert.DeserializeObject<FacebookComment>(response);

            return comment;
        }

        public static FacebookProfile GetProfile(VeterinarianSocialIntegration socialIntegration)
        {
            string accessToken = GetAccessToken(socialIntegration);

            string id = GetId(socialIntegration);

            string url = string.Format(facebook_profile_url, facebook_graph_api, id, accessToken);

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var profile = JsonConvert.DeserializeObject<FacebookProfile>(response);

            return profile;
        }


        public static FacebookProfile GetId(string accessToken)
        {
            string url = string.Format(facebook_me_url, facebook_graph_api, accessToken);

            HttpWebRequest webRequest = CreateWebRequest(url, "GET", null);

            bool success;

            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var profile = JsonConvert.DeserializeObject<FacebookProfile>(response);

            return profile;
        }
    }
}