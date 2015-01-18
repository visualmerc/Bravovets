using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;

namespace BravoVets.DomainService.RepositoryContract
{
    public interface IVeterinarianRepository : IBaseRepository<Veterinarian>
    {
        VeterinarianSocialIntegration SaveFacebookTokens(int veterinarianId, string code, string token, string userId,
            string pageId, string pageToken, int numberOfFollowers);

        VeterinarianSocialIntegration RetrieveFacebookInfo(int veterinarianId);

        VeterinarianSocialIntegration RetrieveTwitterInfo(int veterinarianId);

        VeterinarianSocialIntegration SaveTwitterTokens(int veterinarianId, string accessToken, string tokenSecret,
            string userId, int numberOfFollowers);

        bool DeleteFacebookIntegration(int veterinarianId);

        bool DeleteTwitterIntegration(int veterinarianId);

        VeterinarianSocialIntegration UpdateVeterinarianSocialIntegration(VeterinarianSocialIntegration veterinarianSocialIntegration);

    }
}
