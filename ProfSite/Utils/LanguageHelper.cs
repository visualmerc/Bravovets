using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Routing;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using ProfSite.Models;
using ProfSite.Resources;

namespace ProfSite.Utils
{
    using System.Security.Policy;

    using BravoVets.DomainService.Service;

    public static class LanguageHelper
    {


        public static BravoVetsCountryEnum GetCountryId(string language)
        {
            switch (language)
            {
                case "de":
                    return BravoVetsCountryEnum.DE;
                case "fr":
                    return BravoVetsCountryEnum.FR;
                case "nl":
                    return BravoVetsCountryEnum.NL;
                case "it":
                    return BravoVetsCountryEnum.IT;
                case "es-es":
                    return BravoVetsCountryEnum.ES;
                case "us":
                    return BravoVetsCountryEnum.US;
                case "za":
                    return BravoVetsCountryEnum.ZA;
                case "pt":
                    return BravoVetsCountryEnum.PT;
                case "br":
                    return BravoVetsCountryEnum.BR;
                case "ru":
                    return BravoVetsCountryEnum.RU;
                case "es":
                    return BravoVetsCountryEnum.LAT;
                default:
                    return BravoVetsCountryEnum.GB;

            }

        }


        public static void SetCulture(string language)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
        }

        public static string GetBaseUrl(string code)
        {
            var url = ConfigurationManager.AppSettings[string.Format("{0}.baseurl", code)];
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception(string.Format("BaseUrl not configured for {0}", code));
            }

            return url;
        }

        public static List<SupportedCountry> SupportedCountries()
        {
            var lookupService = new LookupDomainService();

            var countries = lookupService.GetBravoVetsCountries();
            var supportedCountries = new List<SupportedCountry>();

            foreach (var bravoVetsCountry in countries)
            {
                var urlCountryCode = bravoVetsCountry.CountryNameResourceKey.ToLower();

                var supportCountry = new SupportedCountry();
                supportCountry.Code = urlCountryCode;
                supportCountry.DisplayName = GetLocalizedCountryName(bravoVetsCountry.BravoVetsCountryId);
                supportCountry.LocaleCode = bravoVetsCountry.LanguageCode.ToLower();
                supportCountry.SiteUrl = GetBaseUrl(urlCountryCode) + GetLandingRouteName(bravoVetsCountry.BravoVetsCountryId);

                supportedCountries.Add(supportCountry);
            }

            return supportedCountries.OrderBy(c => c.DisplayName).ToList();

        }

        private static string GetLandingRouteName(int bravoVetsCountryId)
        {
            switch (bravoVetsCountryId)
            {
                case (int)BravoVetsCountryEnum.FR:
                case (int)BravoVetsCountryEnum.IT:
                case (int)BravoVetsCountryEnum.DE:
                case (int)BravoVetsCountryEnum.ES:
                case (int)BravoVetsCountryEnum.NL:
                case (int)BravoVetsCountryEnum.ZA:
                case (int)BravoVetsCountryEnum.GB:
                case (int)BravoVetsCountryEnum.PT:
                case (int)BravoVetsCountryEnum.BR:
                case (int)BravoVetsCountryEnum.RU:
                case (int)BravoVetsCountryEnum.LAT:
                    return "/bravecto";
                default:
                    return string.Empty;
            }            
        }

        private static string GetLocalizedCountryName(int bravoVetsCountryId)
        {
            switch (bravoVetsCountryId)
            {
                case (int)BravoVetsCountryEnum.FR:
                    return Resource.France;
                case (int)BravoVetsCountryEnum.IT:
                    return Resource.Italy;
                case (int)BravoVetsCountryEnum.DE:
                    return Resource.Germany;
                case (int)BravoVetsCountryEnum.ES:
                    return Resource.Spain;
                case (int)BravoVetsCountryEnum.NL:
                    return Resource.Netherlands;
                case (int)BravoVetsCountryEnum.US:
                    return Resource.UnitedStates;
                case (int)BravoVetsCountryEnum.ZA:
                    return Resource.SouthAfrica;
                case (int)BravoVetsCountryEnum.PT:
                    return Resource.Portugal;
                case (int)BravoVetsCountryEnum.CA:
                    return Resource.Canada;
                case (int)BravoVetsCountryEnum.BR:
                    return Resource.Brazil;
                case (int)BravoVetsCountryEnum.RU:
                    return Resource.Russia;
                case (int)BravoVetsCountryEnum.LAT:
                    return Resource.Latam;
                default:
                    return Resource.UnitedKingdom;
            }
        }
        
    }
}