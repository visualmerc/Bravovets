using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BravoVets.DomainObject;
using log4net;
using Newtonsoft.Json;
using ProfSite.Models;

namespace ProfSite.Utils
{
    public class TwitterHelper
    {
        private const string oauthSignatureMethod = "HMAC-SHA1";
        private const string oauthVersion = "1.0";

        private const string api_twitter_request_token = "https://api.twitter.com/oauth/request_token";
        private const string api_twitter_authorize = "https://api.twitter.com/oauth/authorize";
        private const string api_twitter_access_token = "https://api.twitter.com/oauth/access_token";
        private const string api_twitter_post = "https://api.twitter.com/1.1/statuses/update.json";
        private const string api_twitter_post_with_media = "https://api.twitter.com/1.1/statuses/update_with_media.json";
        private const string api_twitter_verify_creds = "https://api.twitter.com/1.1/account/verify_credentials.json";

        private const string api_twitter_retweet = "https://api.twitter.com/1.1/statuses/retweet/{0}.json";
        private const string api_twitter_profile = "https://api.twitter.com/1.1/users/show.json";

        private const string api_twitter_home_timeline = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TwitterHelper));
        protected static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        private static string oauth_callbackUrl(string lang)
        {
            string key = string.Format("twitter.{0}.oauth_callbackUrl", lang);

            return ConfigurationManager.AppSettings[key];
        }

        private static string consumer_key(string lang)
        {
            string key = string.Format("twitter.{0}.consumer_key", lang);

            return ConfigurationManager.AppSettings[key];
        }

        private static string consumer_secret(string lang)
        {
            string key = string.Format("twitter.{0}.consumer_secret", lang);

            return ConfigurationManager.AppSettings[key];
        }


        public static string GetUserRequestToken(string lang)
        {
            HttpWebRequest webRequest = GetOAuthRequest(api_twitter_request_token, string.Empty, "POST",
                consumer_key(lang), consumer_secret(lang),
                string.Empty,
                string.Empty,
                string.Empty, oauth_callbackUrl(lang));

            bool success = false;
            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            //TODO:handle errors

            NameValueCollection parsedParams = HttpUtility.ParseQueryString(response);

            string oauth_request_token = parsedParams["oauth_token"];

            return oauth_request_token;
        }

        public static string CreateoAuthRedirectUrl(string requestToken)
        {
            return string.Format("{0}?oauth_token={1}", api_twitter_authorize, requestToken);
        }

        public static TwitterAccess GetUserAccessToken(string oauth_token, string oauth_verifier, string lang)
        {
            string oauthParams = "oauth_verifier=" + oauth_verifier;

            HttpWebRequest webRequest = GetOAuthRequest(api_twitter_access_token, oauthParams, "POST",
                consumer_key(lang), consumer_secret(lang),
                oauth_token,
                string.Empty,
                string.Empty, string.Empty);

            bool success = false;
            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            //TODO:handle errors
            NameValueCollection parsedParams = HttpUtility.ParseQueryString(response);

            var results = new TwitterAccess
                          {
                              user_id = parsedParams["user_id"],
                              access_token = parsedParams["oauth_token"],
                              token_secret = parsedParams["oauth_token_secret"]
                          };

            return results;
        }

        public static string GetHomeTimeline(string accessToken, string accessTokenSecret,
            string maxId, string lang, out bool success)
        {

            success = false;
            int retry = 0;
            string response = "";

            while (!success && retry <= 1)
            {
                string param = string.IsNullOrEmpty(maxId) ? "count=10" : string.Format("count=10&max_id={0}", maxId);

                HttpWebRequest webRequest = GetOAuthRequest(api_twitter_home_timeline, param, "GET",
                    consumer_key(lang), consumer_secret(lang),
                    accessToken,
                    accessTokenSecret,
                    string.Empty, string.Empty);

                response = WebRequestHelper.GetWebResponse(webRequest, out success);

                retry++;
            }
            if (success)
            {
                return response;
            }
            Logger.Error(response);
            return string.Empty;
        }

        public static TwitterStatusModel Reply(string accessToken, string accessTokenSecret, string postId,
            string replyText, string lang)
        {
            HttpWebRequest webRequest = GetOAuthRequest(api_twitter_post, "in_reply_to_status_id=" + postId, "POST",
                consumer_key(lang), consumer_secret(lang),
                accessToken,
                accessTokenSecret,
                replyText,
                string.Empty);

            bool success = false;
            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var model = JsonConvert.DeserializeObject<TwitterStatusModel>(response);
            return model;
        }

        public static TwitterStatusModel Retweet(string accessToken, string accessTokenSecret, string postId,
            string lang)
        {
            //POST statuses/retweet/:id
            string url = string.Format(api_twitter_retweet, postId);

            HttpWebRequest webRequest = GetOAuthRequest(url, string.Empty, "POST",
                consumer_key(lang), consumer_secret(lang),
                accessToken,
                accessTokenSecret,
                string.Empty,
                string.Empty);

            bool success = false;
            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var model = JsonConvert.DeserializeObject<TwitterStatusModel>(response);
            return model;
        }

        public static void PostTweet(string accessToken, string accessTokenSecret, string status, string lang)
        {
            HttpWebRequest webRequest = GetOAuthRequest(api_twitter_post, string.Empty, "POST",
                consumer_key(lang), consumer_secret(lang),
                accessToken,
                accessTokenSecret,
                status,
                string.Empty);

            bool success = false;
            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            //TODO:Handle errors
        }

        public static void PostImage(string accessToken, string accessTokenSecret, string status,
            List<QueueContentAttachment> attachments, string lang)
        {
            var attachment = attachments[0];

            HttpWebRequest request = GetOAuthRequest(api_twitter_post_with_media, string.Empty, "POST",
                consumer_key(lang), consumer_secret(lang),
                accessToken,
                accessTokenSecret,
                string.Empty,
                string.Empty);

            string boundary = DateTime.UtcNow.Ticks.ToString("x");

            request.ContentType = "multipart/form-data; boundary=" + boundary;

            IAsyncResult streamAsyncResult = request.BeginGetRequestStream(null, null);
            using (Stream s = request.EndGetRequestStream(streamAsyncResult))
            {
                var writer = new MultipartWriter(s, boundary);
                writer.WriteSeperator().WriteBoundary().WriteNewline();

                writer.Write("Content-Disposition: form-data; name=\"" + "status" + "\"").WriteNewline();
                writer.WriteNewline();
                writer.Write(status);
                writer.WriteNewline();
                writer.WriteSeperator().WriteBoundary().WriteNewline();
                writer.Write("Content-Type: application/octet-stream").WriteNewline();
                writer.Write("Content-Disposition: form-data; name=\"" + "media[]" + "\"" + "; filename=\"" + attachment.AttachmentFileName +
                             "\"").WriteNewline();
                writer.WriteNewline();
                writer.Write(attachment.AttachmentFile);
                writer.WriteNewline();
                writer.WriteNewline().WriteSeperator().WriteBoundary().WriteSeperator().WriteNewline();
            }

            bool success = false;
            WebRequestHelper.GetWebResponse(request, out success);
        }


        public static TwitterProfile GetProfile(string accessToken, string accessTokenSecret, string userId, string lang)
        {
            string url = api_twitter_profile;

            HttpWebRequest webRequest = GetOAuthRequest(url, string.Format("user_id={0}", userId), "GET",
                consumer_key(lang), consumer_secret(lang),
                accessToken,
                accessTokenSecret,
                string.Empty, string.Empty);

            bool success = false;
            string response = WebRequestHelper.GetWebResponse(webRequest, out success);

            var model = JsonConvert.DeserializeObject<TwitterProfile>(response);
            return model;
        }

        public static HttpWebRequest GetOAuthRequest(string url, string urlParams, string httpMethod, string consumerKey,
            string consumerSecret, string accessToken, string accessTokenSecret, string body, string callbackUrl)
        {
            //Set up the one time use values for the twitter signature process
            string oauthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string oauthTimestamp = buildOauthTimestamp();

            //Gather all the pieces together (body, header, request)
            string postBody = string.Empty;

            if (httpMethod == "POST")
            {
                postBody = "status=" + percentEncode(body);
            }

            //build the signature string of the request header
            string baseString = buildBaseString(url, urlParams, httpMethod, body, consumerKey, accessToken, callbackUrl,
                oauthNonce, oauthTimestamp);
            string signatureString = buildSignatureString(baseString, consumerSecret, accessTokenSecret);

            var request = (HttpWebRequest)WebRequest.Create(url + "?" + urlParams);

            //oauth_callback

            string authorizationHeaderParams = "";

            if (!string.IsNullOrEmpty(accessToken))
            {
                authorizationHeaderParams = string.Format(
                    "OAuth oauth_consumer_key=\"{0}\",oauth_nonce=\"{1}\",oauth_signature=\"{2}\",oauth_signature_method=\"{3}\",oauth_timestamp=\"{4}\",oauth_token=\"{5}\",oauth_version=\"{6}\"",
                    percentEncode(consumerKey),
                    percentEncode(oauthNonce),
                    percentEncode(signatureString),
                    percentEncode(oauthSignatureMethod),
                    percentEncode(oauthTimestamp),
                    percentEncode(accessToken),
                    percentEncode(oauthVersion)
                    );
            }
            else
            {
                authorizationHeaderParams = string.Format(
                    "OAuth oauth_callback=\"{0}\",oauth_consumer_key=\"{1}\",oauth_nonce=\"{2}\",oauth_signature=\"{3}\",oauth_signature_method=\"{4}\",oauth_timestamp=\"{5}\",oauth_version=\"{6}\"",
                    UrlEncode(callbackUrl),
                    percentEncode(consumerKey),
                    percentEncode(oauthNonce),
                    percentEncode(signatureString),
                    percentEncode(oauthSignatureMethod),
                    percentEncode(oauthTimestamp),
                    percentEncode(oauthVersion)
                    );
            }

            request.Headers.Add("Authorization", authorizationHeaderParams);
            request.Method = httpMethod;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 3 * 60 * 1000;
            request.KeepAlive = false;

            //Add the message to the request body
            if (!string.IsNullOrEmpty(body))
            {
                using (Stream stream = request.GetRequestStream())
                {
                    byte[] bodyBytes = new ASCIIEncoding().GetBytes(postBody);
                    stream.Write(bodyBytes, 0, bodyBytes.Length);
                    stream.Flush();
                    stream.Close();
                }
            }

            return request;
        }


        private static string percentEncode(string stringToEncode)
        {
            if (string.IsNullOrEmpty(stringToEncode))
            {
                return string.Empty;
            }

            stringToEncode = HttpUtility.UrlEncode(stringToEncode).Replace("+", "%20");

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            stringToEncode = Regex.Replace(stringToEncode, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            stringToEncode = stringToEncode
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            stringToEncode = stringToEncode.Replace("%7E", "~");

            return stringToEncode;
        }

        public static string UrlEncode(string value)
        {
            var result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        private static string buildBaseString(string apiUrl, string urlParams, string httpMethod, string status,
            string consumerKey, string accessToken, string callbackUrl, string oauthNonce, string oauthTimestamp)
        {
            string baseString = string.Format("{0}&{1}&", httpMethod, percentEncode(apiUrl));

            //Use a sorted dictionary because we need these params in the header in order.  
            var alphabeticalSignatureParams = new SortedDictionary<string, string>();

            //Note that the status is encoded here prior to the encoding (meaning it's double encoded).  This has
            //to be done correctly or it will fail to authorize.
            alphabeticalSignatureParams.Add("oauth_version", oauthVersion);
            alphabeticalSignatureParams.Add("oauth_consumer_key", consumerKey);
            alphabeticalSignatureParams.Add("oauth_nonce", oauthNonce);
            alphabeticalSignatureParams.Add("oauth_signature_method", oauthSignatureMethod);
            alphabeticalSignatureParams.Add("oauth_timestamp", oauthTimestamp);

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                alphabeticalSignatureParams.Add("oauth_callback", UrlEncode(callbackUrl));
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                alphabeticalSignatureParams.Add("oauth_token", accessToken);
            }
            if (!string.IsNullOrEmpty(status))
            {
                alphabeticalSignatureParams.Add("status", percentEncode(status));
            }
            if (!string.IsNullOrEmpty(urlParams))
            {
                string[] urlParamList = urlParams.Split('&');
                foreach (string urlParam in urlParamList)
                {
                    string[] nameValuePair = urlParam.Split('=');
                    if (nameValuePair.Length == 2)
                    {
                        alphabeticalSignatureParams.Add(nameValuePair[0], nameValuePair[1]);
                    }
                }
            }

            int counter = 0;
            foreach (var entry in alphabeticalSignatureParams)
            {
                counter++;
                if (counter == alphabeticalSignatureParams.Count)
                {
                    baseString += percentEncode(string.Format("{0}={1}", entry.Key, entry.Value));
                    break;
                }

                baseString += percentEncode(string.Format("{0}={1}&", entry.Key, entry.Value));
            }

            return baseString;
        }

        private static string buildSignatureString(string stringToSign, string consumerSecret, string accessTokenSecret)
        {
            //Create the signature string using the correct encryption method (per twitter this is HMACSHA1)
            string signingKey = string.Format("{0}&{1}", percentEncode(consumerSecret), percentEncode(accessTokenSecret));
            var hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
            return Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(stringToSign)));
        }

        private static string buildOauthTimestamp()
        {
            //Generate the timestamp value
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}