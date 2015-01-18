using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProfSite.Models
{
    public class SyndicatedContentWidgetModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string ViewAllUrl { get; set; }

        public string ImageUrl { get; set; }

        public bool ShowShare { get; set; }

        public List<SyndicatedContentModel> Contents { get; set; } 

    }
}