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
using BravoVets.DomainService.Repository;
using BravoVets.DomainService.Service;

namespace ProfSite.Auth
{
    using System.Reflection;

    using log4net;

    using ProfSite.Controllers;

    public class BravoVetsAuthManager : IBravoVetsAuthManager
    {
        IBravoVetsUserDomainService _domainService;

        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BravoVetsAuthManager()
        {
            this._domainService = new BravoVetsUserDomainService(new BravoVetsUserRepository());
        }

        public void SyncUserWithLfw()
        {
            var bravoVetsLfwIdStr = NullSessionCheck("BravoVetsLfwId");
            var merckLfwIdStr = NullSessionCheck("LFW_user");
            int merckLfwId;
            int bravoVetsLfwId;
            int.TryParse(merckLfwIdStr, out merckLfwId);
            int.TryParse(bravoVetsLfwIdStr, out bravoVetsLfwId);

            if (merckLfwId < 1)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
            }
            else if (merckLfwId > 0 && merckLfwId != bravoVetsLfwId)
            {
                int bravoVetsUserId;
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


        private static void AddClaimsToUser(BravoVetsUser existingUser)
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

            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionToken);

        }

        #endregion


        public bool SignOutOfLfw()
        {
            throw new NotImplementedException();
        }
    }
}