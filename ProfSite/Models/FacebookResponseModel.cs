using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace ProfSite.Models
{
    public class FacebookPostResponse
    {
        public string id { get; set; }
    }

    public class FacebookProfile
    {
        public string id { get; set; }

        public int difference { get; set; }
        public int likes { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class FacebookPagingModel
    {
        public string previous { get; set; }
        public string next { get; set; }

        public string getuntil()
        {
            NameValueCollection parsedParams = HttpUtility.ParseQueryString(next);
            string until = parsedParams.Get("until");
            return until;
        }

    }

    public class FacebookPersonModel
    {
        public string name { get; set; }
        public string id { get; set; }
    }


    public class FacebookAvailableActions
    {
        public string name { get; set; }
        public string link { get; set; }
    }

    public class FacebookShares
    {
        public int count { get; set; }
    }

    public class FacebookLikes
    {
        public List<FacebookPersonModel> data { get; set; }
        public FacebookPagingModel paging { get; set; }
    }

    public class FacebookHomeItemModel
    {
        public string id { get; set; }
        public FacebookPersonModel from { get; set; }

        public string story { get; set; }
        public string caption { get; set; }
        public string message { get; set; }
        public string picture { get; set; }
        public string link { get; set; }
        public string source { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string type { get; set; }

        public string status_type { get; set; }
        public string object_id { get; set; }
        public DateTime created_time { get; set; }
        public List<FacebookAvailableActions> actions { get; set; }
        public FacebookShares shares { get; set; }
        public FacebookLikes likes { get; set; }
        public FacebookComments comments { get; set; }

        public List<FacebookMediaAttachment> attachments { get; set; }

    }

    public class FacebookComment
    {
        public string id { get; set; }
        public FacebookPersonModel from { get; set; }
        public string message { get; set; }
        public bool can_remove { get; set; }
        public DateTime created_time { get; set; }
        public int like_count { get; set; }
        public bool user_likes { get; set; }
    }

    public class FacebookPages
    {
        public List<FacebookPage> data { get; set; }
    }

    public class FacebookPage
    {

        public string id { get; set; }
        public string name { get; set; }
        public string access_token { get; set; }

        public string[] perms { get; set; }
    }

    public class FacebookComments
    {
        public List<FacebookComment> data { get; set; }
        public FacebookPagingModel paging { get; set; }
    }

    public class FacebookHomeModel
    {
        public List<FacebookHomeItemModel> data { get; set; }
        public FacebookPagingModel paging { get; set; }
    }

    public class FacebookPostAttachments
    {
        public List<FacebookPostAttachment> data { get; set; }
     
    }
    public class FacebookPostAttachment
    {
        public string post_id { get; set; }
        public FacebookAlbumAttachment attachment { get; set; } 
    }

    public class FacebookAlbumAttachment
    {
        public string name { get; set; }
        public string caption { get; set; }
        public string description { get; set; }
        public string fb_object_id { get; set; }
        public List<FacebookMediaAttachment> media { get; set; }
    }

    public class FacebookMediaAttachment
    {
        public string href { get; set; }
        public string type { get; set; }
        public string src { get; set; }

    }
}