using System.Collections.Generic;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainObject.Infrastructure;

namespace BravoVets.DomainService.Contract
{
    public interface ISyndicatedContentDomainService
    {
        /// <summary>
        ///     Get the trending topics relevant to a particular country, using default sort and paging
        /// </summary>
        /// <param name="bravoVetsUserId">PK of the calling user</param>
        /// <param name="bravoVetsCountryId">PK for the country</param>
        /// <returns>A list of syndicated content items</returns>
        List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId);

        /// <summary>
        ///     Get the trending topics relevant to a particular country, using default paging
        /// </summary>
        /// <param name="bravoVetsUserId">PK of the calling user</param>
        /// <param name="bravoVetsCountryId">PK for the country</param>
        /// <param name="sort">the sort term for trending topics</param>
        /// <returns>A list of syndicated content items</returns>
        List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort);

        List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort,
            PagingToken pagingToken);

        /// <summary>
        ///     Get a filtered list of trending topics, filtered by certain types of content
        /// </summary>
        /// <param name="bravoVetsUserId">PK of the calling user</param>
        /// <param name="bravoVetsCountryId">PK for the country</param>
        /// <param name="filter">An enumeration of filter types</param>
        /// <returns>A list of syndicated content items</returns>
        List<SyndicatedContent> GetFilteredTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId,
            ContentFilterEnum filter);

        List<SyndicatedContent> GetFilteredTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId,
            ContentFilterEnum filter, ContentSortEnum sort);

        List<SyndicatedContent> GetFilteredTrendingTopics(int bravoVetsCountryId, int bravoVetsUserId,
            ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken);

        /// <summary>
        ///     Get the Social Tips relevant to a particular country, with default sort and paging
        /// </summary>
        /// <param name="bravoVetsUserId">PK of the calling user</param>
        /// <param name="bravoVetsCountryId">PK for the country</param>
        /// <returns>A list of syndicated content items</returns>
        List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId);

        /// <summary>
        ///     Get the Social Tips relevant to a particular country, with default paging
        /// </summary>
        /// <param name="bravoVetsUserId">PK of the calling user</param>
        /// <param name="bravoVetsCountryId">PK for the country</param>
        /// <param name="sort">the sort term for trending topics</param>
        /// <returns>A list of syndicated content items</returns>
        List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort);

        /// <summary>
        ///     Get the Social Tips relevant to a particular country.
        /// </summary>
        /// <param name="bravoVetsUserId">PK of the calling user</param>
        /// <param name="bravoVetsCountryId">PK for the country</param>
        /// <param name="sort">the sort term for trending topics</param>
        /// <param name="pagingToken">Paging token, to indicate which records to return</param>
        /// <returns>A list of syndicated content items</returns>
        List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort,
            PagingToken pagingToken);

        List<SyndicatedContent> GetFilteredSocialTips(int bravoVetsCountryId, int bravoVetsUserId,
            ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken);

        List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId);

        List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort);

        /// <summary>
        ///     Get the Bravecto Resources relevant to a particular country
        /// </summary>
        /// <param name="bravoVetsUserId"></param>
        /// <param name="bravoVetsCountryId"></param>
        /// <param name="sort"></param>
        /// <param name="pagingToken"></param>
        /// <returns></returns>
        List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort,
            PagingToken pagingToken);

        List<SyndicatedContent> GetFilteredBravectoResources(int bravoVetsCountryId, int bravoVetsUserId,
            ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken);

        /// <summary>
        ///     A user shares a particular trending topic to a social network
        /// </summary>
        /// <param name="bravoVetsUserId">PK for a Bravovets user</param>
        /// <param name="syndicatedContentId">PK for the syndicated content</param>
        /// <param name="platform">Twitter or Facebook</param>
        /// <returns>The content to share immediately</returns>
        SyndicatedContent ShareContent(int bravoVetsUserId, int syndicatedContentId, SocialPlatformEnum platform);

        /// <summary>
        ///     A user selects a particular trending topic as a favorite item
        /// </summary>
        /// <param name="bravoVetsUserId">PK for a Bravovets user</param>
        /// <param name="syndicatedContentId">PK for the syndicated content</param>
        void FavoriteContent(int bravoVetsUserId, int syndicatedContentId);

        bool UnfavoriteContent(int bravoVetsUserId, int syndicatedContentId);

        /// <summary>
        ///     A user selects a particular trending topic as a favorite item
        /// </summary>
        /// <param name="bravoVetsUserId">PK for a Bravovets user</param>
        /// <param name="syndicatedContentId">PK for the syndicated content</param>
        void HideContent(int bravoVetsUserId, int syndicatedContentId);

        bool UnHideContent(int bravoVetsUserId, int syndicatedContentId);

        /// <summary>
        ///     A user expands the view of a particular piece of syndicated content
        /// </summary>
        /// <param name="bravoVetsUserId">PK for a Bravovets user</param>
        /// <param name="syndicatedContentId">PK for the syndicated content</param>
        void ViewContent(int bravoVetsUserId, int syndicatedContentId);

        List<SyndicatedContentAttachment> GetAttachmentsById(int syndicatedContentId);

        SyndicatedContent CreateSyndicatedContent(SyndicatedContent syndicatedContent);

        SyndicatedContent GetSyndicatedContent(int bravoVetsUserId, int syndicatedContentId);

        SyndicatedContentAttachment SaveAttachment(SyndicatedContentAttachment attachment);

        /// <summary>
        /// Check to see if a user has hidden trending topics
        /// </summary>
        /// <param name="bravoVetsUserId">PK for a Bravovets user</param>
        /// <returns>a boolean indicating whether the user has hidden topics</returns>
        bool UserHasHiddenTrendingTopics(int bravoVetsUserId);

        bool UserHasSharedTrendingTopics(int bravoVetsUserId);
    }
}