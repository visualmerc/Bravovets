using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using BravoVets.DomainObject;
using BravoVets.DomainService.Contract;
using ProfSite.Controllers;

namespace ProfSite.Auth
{
    using System.Reflection;

    using BravoVets.DomainService.Repository;
    using BravoVets.DomainService.Service;

    using log4net;

    public class BravoVetsTestableAuthManager : IBravoVetsAuthManager
    {
        IBravoVetsUserDomainService _domainService;

        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BravoVetsTestableAuthManager()
        {
            this._domainService = new BravoVetsUserDomainService(new BravoVetsUserRepository());
        }

        public void SyncUserWithLfw()
        {

            var bravoVetsLfwIdStr = NullSessionCheck("BravoVetsLfwId");
            // Override the LFW_user id with this value
            IdParamOverride("testUserId", "TestUserId");

            var testUserIdStr = NullSessionCheck("TestUserId");
            var merckLfwIdStr = NullSessionCheck("LFW_user");

            int testUserId;
            int merckLfwId;
            int bravoVetsLfwId;
            int bravoVetsUserId;

            int.TryParse(testUserIdStr, out testUserId);
            int.TryParse(merckLfwIdStr, out merckLfwId);
            int.TryParse(bravoVetsLfwIdStr, out bravoVetsLfwId);

            if (merckLfwId < 1 && testUserId < 1)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
            }
            else if (testUserId > 0 && merckLfwId != testUserId)
            {
                var didAuth = this.HydrateBravoVetsUser(testUserId, out bravoVetsUserId);
                if (!didAuth)
                {
                    this.CreateBravoVetsUser(this.GetMerckUser(testUserId), out bravoVetsUserId);
                }
                HttpContext.Current.Session["BravoVetsLfwId"] = testUserId;
                HttpContext.Current.Session["LFW_user"] = testUserId;

                var languageController = new AbstractBaseController();
                var lang = languageController.GetUserLanguage();
                AbstractBaseSocialController.UpdateSocialIntegrationInfo(bravoVetsUserId, lang);

            }
            else if (merckLfwId > 0 && merckLfwId != bravoVetsLfwId)
            {
                var didAuth = this.HydrateBravoVetsUser(merckLfwId, out bravoVetsUserId);
                if (!didAuth)
                {
                    this.CreateBravoVetsUser(this.GetMerckUser(merckLfwId), out bravoVetsUserId);
                }
                HttpContext.Current.Session["BravoVetsLfwId"] = merckLfwIdStr;

                var languageController = new AbstractBaseController();
                var lang = languageController.GetUserLanguage();
                AbstractBaseSocialController.UpdateSocialIntegrationInfo(bravoVetsUserId, lang);
            }
        }

        public bool InvalidateCurrentIdentityInfo()
        {
            try
            {
                HttpContext.Current.Session["BravoVetsLfwId"] = "-1";
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
                this.SyncUserWithLfw();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("BravoVetsAuthManager.InvalidateCurrentIdentityInfo", ex);
                return false;
            }
        }

        public bool HydrateBravoVetsUser(int merckId, out int bravoVetsUserId)
        {
            bravoVetsUserId = 0;
            var existingUser = this._domainService.GetBravoVetsUserFromLfwId(merckId);
            if (existingUser == null)
            {
                return false;
            }
            bravoVetsUserId = existingUser.BravoVetsUserId;

            AddClaimsToUser(existingUser);

            return true;
        }

        public bool CreateBravoVetsUser(MerckUser user, out int bravoVetsUserId)
        {
            var newUser = this._domainService.CreateBravoVetsUserFromLfw(user);
            bravoVetsUserId = newUser.BravoVetsUserId;
            AddClaimsToUser(newUser);
            return true;
        }

        public bool SignOutOfLfw()
        {
            throw new NotImplementedException();
        }

        public static void AddClaimsToUser(BravoVetsUser existingUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.FirstName + " " + existingUser.Lastname),
                new Claim("MerckLfwId", existingUser.MerckId.ToString(CultureInfo.InvariantCulture)),
                new Claim("BravoVetsUserId", existingUser.BravoVetsUserId.ToString(CultureInfo.InvariantCulture))
            };

            if (existingUser.AcceptedTandC)
            {
                var acceptRole = new Claim(ClaimTypes.Role, "AcceptedToc");
                claims.Add(acceptRole);
            }

            var identity = new ClaimsIdentity(claims, "Custom");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            var sessionToken = new SessionSecurityToken(claimsPrincipal, TimeSpan.FromHours(8));

            Thread.CurrentPrincipal = claimsPrincipal;

            if (FederatedAuthentication.SessionAuthenticationModule != null)
                FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionToken);

        }

        #region private methods

        private static string NullSessionCheck(string key)
        {
            if (HttpContext.Current.Session[key] == null)
            {
                return string.Empty;
            }
            else if (string.IsNullOrEmpty(HttpContext.Current.Session[key].ToString()))
            {
                return string.Empty;
            }
            else
            {
                return HttpContext.Current.Session[key].ToString();
            }
        }

        private static void IdParamOverride(string key, string sessionKey)
        {
            if (HttpContext.Current.Request[key] == null || string.IsNullOrEmpty(HttpContext.Current.Request[key]))
            {
                return;
            }
            else
            {
                int idKey;
                if (Int32.TryParse(HttpContext.Current.Request[key], out idKey))
                {
                    HttpContext.Current.Session[sessionKey] = idKey;
                }
            }
        }

        private MerckUser GetMerckUser(int merckUserId)
        {
            var merckUser = new MerckUser
            {
                CountryOrigin = NullSessionCheck("LFW_CountryOrigin"),
                EmailAddress = NullSessionCheck("LFW_EmailAddress"),
                FirstName = NullSessionCheck("LFW_FirstName"),
                LastName = NullSessionCheck("LFW_LastName"),
                MerckUserId = merckUserId,
                Occupation = NullSessionCheck("LFW_Occupation")
            };

            return merckUser;
        }

        #endregion

    }
}