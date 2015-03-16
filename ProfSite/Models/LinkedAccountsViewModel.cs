using System.Collections.Generic;
using System.Linq;

namespace ProfSite.Models
{
    public class LinkedAccountsViewModel
    {
        public LinkedAccountsViewModel()
        {
            Tweets = Enumerable.Empty<TweetModel>();
            Posts = Enumerable.Empty<FacebookTimelinePost>();
        }

        public bool IsFacebookLinked { get; set; }
        public bool IsTwitterLinked { get; set; }
        public string TwitterUserName { get; set; }
        public IEnumerable<TweetModel> Tweets { get; set; }
        public IEnumerable<FacebookTimelinePost> Posts { get; set; }
    }
}