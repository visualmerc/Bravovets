using System.Collections.Generic;

namespace ProfSite.Models
{


    public class TwitterErrorModel
    {
        public string message { get; set; }
        public string code { get; set; }
    }
    public class TwitterErrorsModel
    {
        public List<TwitterErrorModel> errors { get; set; }
    }
    public class TwitterProfile
    {
        public int difference { get; set; }
        public int followers_count { get; set; }
    }
    public class TwitterStatusModel
    {
        public string created_at { get; set; }
        public TweetEntityModel entities { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public TweetUserModel user { get; set; }
        public TwitterStatusModel retweeted_status { get; set; }

        public string retweet_count { get; set; }
        public bool retweeted { get; set; }
    }

    public class TweetUserModel
    {

        public string id { get; set; }
        public string name { get; set; }
        public string profile_image_url { get; set; }
        public string location { get; set; }
        public string profile_link_color { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_text_color { get; set; }
        public double? utc_offset { get; set; }
        public string time_zone { get; set; }
        public string screen_name { get; set; }
        public string description { get; set; }

      
    }

    public class TweetEntityModel
    {
        public List<TweetUrlModel> urls { get; set; }
        public List<TweetHashtagModel> hashtags { get; set; }
        public List<TweetUserMentionModel> user_mentions { get; set; }
    }

    public class TweetUrlModel
    {
        public string expanded_url { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
    }

    public class TweetHashtagModel
    {
        public string text { get; set; }
    }

    public class TweetUserMentionModel
    {
        public string name { get; set; }
        public string id_str { get; set; }
        public float id { get; set; }
        public string screen_name { get; set; }
    }
}