using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ProfSite.Auth
{
    public class BravoVetsClaimsAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            // Action that we are checking (i.e. Accepted Terms and conditions)
            var claimOperation = context.Action.First().Value;

            // The resources that we are securing (BravoVets)
            var resources = context.Resource.Select(resource => resource.Value).ToList();

            // Check the currently set claims, compare claim values vs. our action
            foreach (var claim in context.Principal.Claims)
            {
                if (claim.Value == claimOperation)
                {
                    return true;
                }
            }

            //TODO TEMP 
            return true;
            //return false;
        }

    }
}