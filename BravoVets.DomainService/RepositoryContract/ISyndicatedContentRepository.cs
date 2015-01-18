using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.RepositoryContract
{
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;

    public interface ISyndicatedContentRepository : IBaseRepository<SyndicatedContent>
    {
        List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken);

        List<SyndicatedContent> GetFilteredSyndicatedContents(SyndicatedContentTypeEnum contentType, int bravoVetsCountryId, int bravoVetsUserId, ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken);

        List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken);

        List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken);

        bool IncrementStatusCount(int syndicatedContentId, ActivityTypeEnum actionType);

        bool DecrementStatusCount(int syndicatedContentId, ActivityTypeEnum actionType, int trueNumber);

        List<SyndicatedContentAttachment> GetContentAttachmentsById(int syndicatedContentId);

        SyndicatedContentAttachment GetAttachment(int syndicatedContentAttachmentId);

        SyndicatedContentAttachment AddAttachment(SyndicatedContentAttachment attachment);

        SyndicatedContentAttachment UpdateAttachment(SyndicatedContentAttachment attachment);

        List<SyndicatedContent> GetSyndicatedContentsAdmin(SyndicatedContentTypeEnum contentType, bool inFuture,
            BravoVetsCountryEnum country,
            ContentSortEnum sort,
            PagingToken pagingToken,
            out int recordCount);

        bool DeleteSyndicatedContentAttachment(int syndicatedContentAttachmentId);

        bool DeleteSyndicatedContentAttachmentByParentId(int syndicatedContentId);

    }
}
