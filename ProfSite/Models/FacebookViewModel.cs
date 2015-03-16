using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProfSite.Models
{
    public class FacebookViewModel
    {
        public bool IsFacebookLinked { get; set; }
        public List<FacebookTimelinePost> Posts { get; set; }
    }
}