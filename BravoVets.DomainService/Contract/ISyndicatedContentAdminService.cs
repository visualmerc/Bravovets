
namespace BravoVets.DomainService.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;

    public interface ISyndicatedContentAdminService
    {
        /// <summary>
        /// Create a new trending topic, use the PublishDateUtc property to schedule in the future
        /// </summary>
        /// <param name="trendingTopic"></param>
        /// <returns>The syndicatedcontent object, inserted into the db</returns>
        SyndicatedContent QueueNewTrendingTopic(SyndicatedContent trendingTopic);

        SyndicatedContent QueueNewBravectoResource(SyndicatedContent bravectoResource);

        SyndicatedContent QueueNewSocialTip(SyndicatedContent socialTip);

        SyndicatedContent UpdateSyndicatedContent(SyndicatedContent content);

        /// <summary>
        /// Will update or insert a list of attachments.  Will not delete -- do that manually
        /// </summary>
        /// <param name="attachments">raw attachment list</param>
        /// <returns>The processed list of attachments</returns>
        List<SyndicatedContentAttachment> SaveSyndicatedContentAttachments(List<SyndicatedContentAttachment> attachments);

        SyndicatedContentAttachment SaveSyndicatedContentAttachment(SyndicatedContentAttachment attachment);

        SyndicatedContentAttachment AssociateFeaturedContent(int featuredContentId, int syndicatedContentId);

        /// <summary>
        /// Will update or insert syndicated content links
        /// </summary>
        /// <param name="links">raw links</param>
        /// <returns>The processed list of links</returns>
        List<SyndicatedContentLink> SaveContentLinks(List<SyndicatedContentLink> links);

        SyndicatedContentLink SaveContentLink(SyndicatedContentLink link);
        
        /// <summary>
        /// List of trending topics for the admin pages
        /// </summary>
        /// <param name="inFuture">Indicates whether you want records in the future or past</param>
        /// <param name="country">Enum to indicate which country's content you'd like</param>
        /// <param name="sort">How the items should be sorted</param>
        /// <param name="pagingToken"></param>
        /// <param name="recordCount">Total number of records that fit the criteria</param>
        /// <returns>list of trending topics</returns>
        List<SyndicatedContent> GetTrendingTopicsAdminList(bool inFuture, BravoVetsCountryEnum country, ContentSortEnum sort, PagingToken pagingToken, out int recordCount);

        List<SyndicatedContent> GetBravectoResourcesAdminList(bool inFuture, BravoVetsCountryEnum country, ContentSortEnum sort, PagingToken pagingToken, out int recordCount);

        List<SyndicatedContent> GetSocialTipsAdminList(bool inFuture, BravoVetsCountryEnum country, ContentSortEnum sort, PagingToken pagingToken, out int recordCount);

        SyndicatedContent GetSyndicatedContent(int syndicatedContentId);

        SyndicatedContentAttachment GetSyndicatedContentAttachment(int syndicatedContentAttachmentId);

        SyndicatedContentLink GetSyndicatedContentLink(int syndicatedContentLinkId);

        bool DeleteSyndicatedContent(int syndicatedContentId);

        bool DeleteSyndicatedContentAttachment(int syndicatedContentAttachmentId);

        bool DeleteSyndicatedContentLink(int syndicatedContentLinkId);

        /// <summary>
        /// Get a list of featured content, for dropdowns
        /// </summary>
        /// <param name="contentType">SocialTips, TrendingTopics, or BravectoResources</param>
        /// <param name="country">Country that the featured content is targeted towards</param>
        /// <returns>Ids, names, thumbnail images</returns>
        List<FeaturedContentSlim> GetFeaturedContent(SyndicatedContentTypeEnum contentType, BravoVetsCountryEnum country);

        FeaturedContent GetFeaturedContent(int featuredContentId);

        FeaturedContent AddUpdateFeaturedContent(FeaturedContent originalContent);
       
        /// <summary>
        /// Starter method, to get a new syndicated content object
        /// </summary>
        /// <param name="contentType">Trending Topic, Bravecto Resource, and Social Tip</param>
        /// <param name="country">Targeted country</param>
        /// <param name="author"></param>
        /// <returns></returns>
        SyndicatedContent GetMinimalEmptySyndicatedContent(SyndicatedContentTypeEnum contentType, BravoVetsCountryEnum country, string author);

        SyndicatedContentAttachment CreateMinimalEmptyAttachment(int syndicatedContentId, string attachmentExtension);

    }
}
