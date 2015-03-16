using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProfSite.Models
{
    public class TwitterViewModel
    {
        public string UserName { get; set; }
        [StringLength(140)]
        public string Tweet { get; set; }
        public bool IsTwitterLinked { get; set; }
    }
}