using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Services;

namespace ProfSite.Models
{

    public class FacebookActionModel
    {
        public string Status { get; set; }
    }

    public class FacebookTimelineComment
    {

        public FacebookTimelineComment(FacebookComment facebookComment)
        {
            Id = facebookComment.id;
            Message = facebookComment.message;
            LikeCount = facebookComment.like_count;
            UserLikes = facebookComment.user_likes;

            if (facebookComment.from != null)
            {
                From = facebookComment.from.name;
                FromId = facebookComment.from.id;
            }
        }

        public string Id { get; set; }
        public string From { get; set; }

        public string FromId { get; set; }

        public string Message { get; set; }
        public int LikeCount { get; set; }
        public bool UserLikes { get; set; }
    }


    public class FacebookTimelinePost
    {
        public FacebookTimelinePost(FacebookHomeItemModel homeItem)
        {
            Avatar = string.Format("https://graph.facebook.com/{0}/picture", homeItem.from.id);


            if (homeItem.from != null)
            {
                Name = homeItem.from.name;
                FromId = homeItem.from.id;
            }
            else if (!string.IsNullOrEmpty(homeItem.story))
            {
                Name = homeItem.story;
            }
            else if (!string.IsNullOrEmpty(homeItem.name))
            {
                Name = homeItem.name;
            }


            SetPhotoAndCaption(homeItem);
            SetMessage(homeItem);

            Caption = homeItem.caption;

            if (homeItem.type == "video" && !string.IsNullOrEmpty(homeItem.source))
            {
                Link = homeItem.source;
            }
            else
            {
                Link = homeItem.link;
            }

            Type = homeItem.type;

            if (homeItem.status_type == "shared_story")
            {
                Id = homeItem.id;
            }
            else
            {
                Id = string.IsNullOrEmpty(homeItem.object_id) ? homeItem.id : homeItem.object_id;
            }

            Comments = new List<FacebookTimelineComment>();
            if (homeItem.comments != null && homeItem.comments.data != null)
            {
                foreach (var facebookComment in homeItem.comments.data.Take(5))
                {
                    var comment = new FacebookTimelineComment(facebookComment);
                    Comments.Add(comment);
                }

                if (homeItem.comments.data.Count > 5)
                {
                    if (homeItem.from != null)
                    {
                        var id = Id.Replace(FromId + "_", "");

                        FBPostUr = string.Format("https://www.facebook.com/{0}/posts/{1}", FromId, id);
                    }
                    else
                    {
                        FBPostUr = string.Format("https://www.facebook.com/{0}", Id);
                    }
                }
            }
        }

        private void SetMessage(FacebookHomeItemModel homeItem)
        {
            var message = homeItem.message;
            if (string.IsNullOrEmpty(message))
                return;

            Regex regx = new Regex("(?<!(?:href='|<a[^>]*>))(http|https|ftp|ftps)://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

            MatchCollection matches = regx.Matches(message);

            for (int i = matches.Count - 1; i >= 0; i--)
            {
                string newURL = "<a target='_blank' href='" + matches[i].Value + "'>" + matches[i].Value + "</a>";

                message = message.Remove(matches[i].Index, matches[i].Length).Insert(matches[i].Index, newURL);
            }


            Message = message;
        }

        private void SetPhotoAndCaption(FacebookHomeItemModel homeItem)
        {
            Photos = new List<string>();
            if (homeItem.attachments == null || homeItem.attachments.Count.Equals(0))
            {
                if (string.IsNullOrEmpty(homeItem.picture))
                    return;
                
                Photos.Add(FormatPhoto(homeItem.link,homeItem.picture));
                return;
            }

            foreach (var attachment in homeItem.attachments)
            {
                Photos.Add(FormatPhoto(attachment.href,attachment.src));
            }
        }

        private string FormatPhoto(string link, string photoUrl)
        {
            var photo = string.Format("<img class='post-photo' src='{0}' alt='fpPic' />", photoUrl);

            if (string.IsNullOrEmpty(link))
            {
                return photo;
            }
            return string.Format("<a href='{0}' target='_blank'>{1}</a>", link, photo);
        }

        public string Avatar { get; set; }
        public string FromId { get; set; }
        public string Name { get; set; }

        public string Message { get; set; }

        public List<string> Photos { get; set; }

        public string Caption { get; set; }

        public string Link { get; set; }

        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }

        public string Type { get; set; }
        public string Id { get; set; }
        public string FBPostUr { get; set; }
        public List<FacebookTimelineComment> Comments { get; set; }

    }

    public class FacebookTimeline
    {
        public FacebookTimeline()
        {
            Posts = new List<FacebookTimelinePost>();
        }

        public string NextPage { get; set; }
        public List<FacebookTimelinePost> Posts { get; set; }
        public bool AccountLinked { get; set; }
    }
}