using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject.Enum;

namespace BravoVets.DomainService.RepositoryContract
{
    using System.Security.Cryptography.X509Certificates;

    using BravoVets.DomainObject;

    public interface ISyndicatedContentUserRepository : IBaseRepository<SyndicatedContentUser>
    {
        List<SyndicatedContentUser> GetByUser(int bravoVetsUserId);

        /// <summary>
        /// Get all the SyndicatedContentUsers, given a list of syndicatedcontentIds
        /// </summary>
        /// <param name="bravoVetsUserId">PK for the user</param>
        /// <param name="eligibleList">The list of SyndicatedConentIds</param>
        /// <returns></returns>
        List<SyndicatedContentUser> GetByUserAndScope(int bravoVetsUserId, List<int> eligibleList);

        /// <summary>
        /// Gets all the SyndicatedContentUsers, given a userid and activity type
        /// </summary>
        /// <param name="bravoVetsUserId"></param>
        /// <param name="activityTypeId"></param>
        /// <returns></returns>
        List<SyndicatedContentUser> GetByUserAndAction(int bravoVetsUserId, int activityTypeId);

        /// <summary>
        /// Gets a specific SyndicatedContentUser, based on the action type, user and parent content
        /// </summary>
        /// <param name="bravoVetsUserId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="syndicatedContentId"></param>
        /// <returns></returns>
        SyndicatedContentUser GetByUserActionContent(int bravoVetsUserId, int activityTypeId, int syndicatedContentId);

        bool DeleteByUserActionContent(int bravoVetsUserId, int activityTypeId, int syndicatedContentId);

        /// <summary>
        /// Used to updated the count of a certian type of activity for content (sharing, favoriting, etc.)
        /// </summary>
        /// <param name="activityTypeId">PK for the activity type</param>
        /// <param name="syndicatedContentId">PK for the activity</param>
        /// <returns>number of total favorites, shares, views</returns>
        int GetCountByActionContent(int activityTypeId, int syndicatedContentId);

        bool UserHasContent(SyndicatedContentTypeEnum contentType, ContentFilterEnum filter, int bravoVetsUserId);

    }
}
