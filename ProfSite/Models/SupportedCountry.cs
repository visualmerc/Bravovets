using System.Collections.Generic;
using System.Web.Mvc;

namespace ProfSite.Models
{
    public class SupportedCountry
    {

        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string LocaleCode { get; set; }
        public string SiteUrl { get; set; }
    }

    public class SelectCountry
    {
        public SelectList SupportedCountries { get; set; }
        public string BaseUrl { get; set; }
        public string Info { get; set; }
        public string ButtonText { get; set; }
    }
}