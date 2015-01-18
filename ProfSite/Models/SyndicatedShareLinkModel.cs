using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProfSite.Models
{
    public class SyndicatedShareLinkModel
    {
        public bool ShowShare { get; set; }

        public bool EnableShare { get; set; }
        public int SyndicatedContentId { get; set; }
    }
}