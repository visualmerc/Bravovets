using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProfSite.Models
{

    public class TwitterActionModel
    {
        public string Status { get; set; }
    }

    public class TwitterTimeline
    {
        public bool AccountLinked { get; set; }
        public List<TweetModel> Tweets { get; set; }
        public string LastId { get; set; }
    }
    
    public class TweetModel
    {
        public const string TWITTER_DATE_FORMAT = "ddd MMM dd HH:mm:ss +ffff yyyy";
        public const string TWITTER_URL_STRING = "<a href=\"{0}\" target=\"_blank\">{1}</a>";

        public const string TWITTER_HASHTAG_STRING =
            "<a class=\"twitter-hashtag\" href=\"http://www.twitter.com/{0}\" target=\"_blank\">{1}</a>";

        public const string TWITTER_USER_MENTION_STRING =
            "<a class=\"twitter-user-mention\" href=\"http://www.twitter.com/{0}\" target=\"_blank\">{1}</a>";

        public string GetRetweetClass
        {
            get
            {
                if (!retweeted) return string.Empty;

                return "retweeted";
            }

        }

        public TweetModel(TwitterStatusModel twitterStatusModel)
        {
            created_at = twitterStatusModel.created_at;
            entities = twitterStatusModel.entities;
            text = twitterStatusModel.text;
            id = twitterStatusModel.id;
            user = twitterStatusModel.user;
            retweeted_status = twitterStatusModel.retweeted_status;
            retweeted = twitterStatusModel.retweeted;
            retweet_count = twitterStatusModel.retweet_count;
            CanRetweet = true;
        }
        public bool CanRetweet { get; set; }
        public bool retweeted { get; set; }

        public string retweet_count { get; set; }

        public string created_at { get; set; }

        public TweetEntityModel entities { get; set; }

        public string text { get; set; }

        public string id { get; set; }

        public TweetUserModel user { get; set; }

        public TwitterStatusModel retweeted_status { get; set; }

        public string profile_image_url_https
        {
            get
            {
                return retweeted_status != null
                    ? retweeted_status.user.profile_image_url_https
                    : user.profile_image_url_https;
            }
        }

        public string retweeted_by_author
        {
            get { return retweeted_status != null ? user.name : null; }
        }

        public string retweeted_by_screen_name
        {
            get { return retweeted_status != null ? user.screen_name : null; }
        }

        public string author
        {
            get { return retweeted_status != null ? retweeted_status.user.name : user.name; }
        }

        public string author_screen_name
        {
            get { return retweeted_status != null ? retweeted_status.user.screen_name : user.screen_name; }
        }

        public string formatted_text
        {
            get
            {
                string htmlFormattedText = text;

                if (retweeted_status != null)
                {
                    htmlFormattedText = retweeted_status.text;
                }

                //Get links as anchor tags with style tags for link color
                foreach (TweetUrlModel link in entities.urls)
                {
                    htmlFormattedText = htmlFormattedText.Replace(link.url,
                        string.Format(TWITTER_URL_STRING, link.url, link.display_url)
                        );
                }

                //Get hashtags as anchor tags with style for link color
                foreach (TweetHashtagModel hashtag in entities.hashtags)
                {
                    string hashtagString = "#" + hashtag.text;
                    htmlFormattedText = htmlFormattedText.Replace(hashtagString,
                        string.Format(TWITTER_HASHTAG_STRING, hashtagString, hashtagString)
                        );
                }

                //Get user mentions as anchor tages with style for link color
                foreach (TweetUserMentionModel userMention in entities.user_mentions)
                {
                    string userMentionString = "@" + userMention.screen_name;
                    if (htmlFormattedText.Contains(userMentionString))
                    {
                        htmlFormattedText = htmlFormattedText.Replace(userMentionString,
                            string.Format(TWITTER_USER_MENTION_STRING, userMentionString, userMentionString)
                            );
                    }
                }

                //Add style for text color
                htmlFormattedText = string.Format("<span style=\"color:{0};\">{1}</span>", user.profile_text_color,
                    htmlFormattedText);
                return htmlFormattedText;
            }
        }

        public string elapsed_time
        {
            get
            {
                TimeSpan elapsed = DateTime.UtcNow -
                                   DateTime.ParseExact(created_at, TWITTER_DATE_FORMAT, new CultureInfo("en-us"));
                if (elapsed.TotalSeconds < 60)
                {
                    //If this results in a negative number, just show that it must have been VERY recently.
                    if (elapsed.TotalSeconds < 0)
                    {
                        return "2s";
                    }
                    return Convert.ToInt32(elapsed.TotalSeconds).ToString() + "s";
                }
                else
                {
                    if (elapsed.TotalMinutes < 60)
                    {
                        return Convert.ToInt32(elapsed.TotalMinutes).ToString() + "m";
                    }
                    else
                    {
                        if (elapsed.TotalHours < 24)
                        {
                            return Convert.ToInt32(elapsed.TotalHours).ToString() + "h";
                        }
                        else
                        {
                            return created_date.ToString("d MMM");
                        }
                    }
                }
            }
        }

        public DateTime created_date
        {
            get
            {
                if (user.utc_offset.HasValue)
                {
                    return
                        DateTime.ParseExact(created_at, TWITTER_DATE_FORMAT, new CultureInfo("en-us"))
                            .AddSeconds(user.utc_offset.Value);
                }

                return DateTime.ParseExact(created_at, TWITTER_DATE_FORMAT, new CultureInfo("en-us"));
            }
        }

        public string formatted_created_date
        {
            get { return created_date.ToString("h:mm tt - d MMM"); }
        }
    }
}