using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;

namespace BravoVets.DomainService.Contract
{
    public interface IBravoVetsUserDomainService
    {
        /// <summary>
        /// The initial information on a user will come from the Merck login framework. Use the information to 
        /// create a vet and Bravovets user
        /// </summary>
        /// <param name="merckUser">The minimal information from the Lfw</param>
        /// <returns>A newly minted BravoVets user</returns>
        BravoVetsUser CreateBravoVetsUserFromLfw(MerckUser merckUser);

        BravoVetsUser UpdateBravoVetsUser(BravoVetsUser bravoVetsUser);

        /// <summary>
        /// Passes a subset of changed values from the UI to the service
        /// </summary>
        /// <param name="bravoVetsUserDto">The minimal set user information</param>
        /// <returns>An updated, full bravovets user</returns>
        BravoVetsUser UpdateUserFromProfile(BravoVetsUser bravoVetsUserDto);

        BravoVetsUser GetBravoVetsUser(int bravoVetsUserId);

        BravoVetsUser GetBravoVetsUserFromLfwId(int loginFrameworkId);

        BravoVetsUser GetBravoVetsUserFullGraph(int bravoVetsUserId);

        BravoVetsUser GetBravoVetsUserForProfileEdit(int bravoVetsUserId);

        BravoVetsUser GetBravoVetsUserForProfileEdit(int bravoVetsUserId, int veterinarianFacilityId);

        bool DeleteBravoVetsUser(int bravoVetsUserId);

        bool AcceptTermsAndConditions(int bravoVetsUserId);

        bool RejectTermsAndConditions(int bravoVetsUserId);

        VeterinarianSocialIntegration GetTwitterSocialIntegration(BravoVetsUser bravoVetsUser);

        VeterinarianSocialIntegration GetFacebookSocialIntegration(BravoVetsUser bravoVetsUser);

        VeterinarianSocialIntegration SaveFacebookTokens(BravoVetsUser bravoVetsUser, string code, string token,
            string userId, string pageId, string pageToken,
            int numberOfFollowers);

        VeterinarianSocialIntegration SaveTwitterTokens(BravoVetsUser bravoVetsUser, string accessToken,
            string tokenSecret, string userId, int numberOfFollowers);

        bool DeleteFacebookIntegration(int bravoVetsUserId);

        bool DeleteTwitterIntegration(int bravoVetsUserId);

        VeterinarianSocialIntegration UpdateVeterinarianSocialIntegration(
            VeterinarianSocialIntegration veterinarianSocial);
    }
}
