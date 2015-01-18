using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Service
{
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;
    using BravoVets.DomainService.Contract;
    using BravoVets.DomainService.Repository;
    using BravoVets.DomainService.RepositoryContract;

    using DomainObject;

    public class SyndicatedContentAdminService : DomainServiceBase, ISyndicatedContentAdminService
    {
       #region private properties

        private ISyndicatedContentRepository _contentRepository;

        private IFeaturedContentRepository _featuredContentRepository;

        private ISyndicatedContentLinkRepository _linkRepository;

        private const int DefaultRecords = 50;

        private const ContentSortEnum DefaultSort = ContentSortEnum.ContentDate;

        #endregion

        #region ctor

        public SyndicatedContentAdminService() : this(new SyndicatedContentRepository(), new FeaturedContentRepository(), new SyndicatedContentLinkRepository())
        {
        }

        public SyndicatedContentAdminService(ISyndicatedContentRepository syndicatedContentRepository) 
            : this(syndicatedContentRepository, new FeaturedContentRepository(), new SyndicatedContentLinkRepository())
        {
        }

        public SyndicatedContentAdminService(ISyndicatedContentRepository syndicatedContentRepository, IFeaturedContentRepository featuredContentRepository)
            : this(syndicatedContentRepository, featuredContentRepository, new SyndicatedContentLinkRepository())
        {
        }

        public SyndicatedContentAdminService(
            ISyndicatedContentRepository syndicatedContentRepository,
            IFeaturedContentRepository featuredContentRepository,
            ISyndicatedContentLinkRepository syndicatedContentLinkRepository)
        {
            this._contentRepository = syndicatedContentRepository;
            this._featuredContentRepository = featuredContentRepository;
            this._linkRepository = syndicatedContentLinkRepository;
        }

        #endregion

        public SyndicatedContent QueueNewTrendingTopic(SyndicatedContent trendingTopic)
        {
            try
            {
                trendingTopic.SyndicatedContentTypeId = (int)SyndicatedContentTypeEnum.TrendingTopics;
                return this.CreateSyndicatedContent(trendingTopic);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.QueueNewTrendingTopic");
                throw processedError;
            }
        }

        public SyndicatedContent QueueNewBravectoResource(SyndicatedContent bravectoResource)
        {
            try
            {
                bravectoResource.SyndicatedContentTypeId = (int)SyndicatedContentTypeEnum.BravectoResources;
                return this.CreateSyndicatedContent(bravectoResource);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.QueueNewBravectoResource");
                throw processedError;
            }
        }

        public SyndicatedContent QueueNewSocialTip(SyndicatedContent socialTip)
        {
            try
            {
                socialTip.SyndicatedContentTypeId = (int)SyndicatedContentTypeEnum.SocialTips;
                return this.CreateSyndicatedContent(socialTip);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.QueueNewSocialTip");
                throw processedError;
            }
        }

        public List<SyndicatedContentLink> SaveContentLinks(List<SyndicatedContentLink> links)
        {
            try
            {
                return links.Select(syndicatedContentLink => 
                    syndicatedContentLink.SyndicatedContentLinkId > 0 
                    ? this._linkRepository.Update(syndicatedContentLink) 
                    : this._linkRepository.Create(syndicatedContentLink)).ToList();
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.SaveContentLinks");
                throw processedError;
            }
        }

        public SyndicatedContentLink SaveContentLink(SyndicatedContentLink link)
        {
            try
            {
                return link.SyndicatedContentLinkId > 0
                           ? this._linkRepository.Update(link)
                           : this._linkRepository.Create(link);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentAdminService.SaveContentLink");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetTrendingTopicsAdminList(
            bool inFuture,
            BravoVetsCountryEnum country,
            ContentSortEnum sort,
            PagingToken pagingToken,
            out int recordCount)
        {
            try
            {
                const SyndicatedContentTypeEnum SyndicatedType = SyndicatedContentTypeEnum.TrendingTopics;

                var syndicatedContents = this.ProcessSyndicatedContents(inFuture, country, sort, pagingToken, out recordCount, SyndicatedType);
                return syndicatedContents;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.GetTrendingTopicsAdminList");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetBravectoResourcesAdminList(bool inFuture, BravoVetsCountryEnum country, ContentSortEnum sort, PagingToken pagingToken, out int recordCount)
        {
            try
            {
                const SyndicatedContentTypeEnum SyndicatedType = SyndicatedContentTypeEnum.BravectoResources;

                var syndicatedContents = this.ProcessSyndicatedContents(inFuture, country, sort, pagingToken, out recordCount, SyndicatedType);
                return syndicatedContents;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.GetBravectoResourcesAdminList");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetSocialTipsAdminList(bool inFuture, BravoVetsCountryEnum country, ContentSortEnum sort, PagingToken pagingToken, out int recordCount)
        {
            try
            {
                const SyndicatedContentTypeEnum SyndicatedType = SyndicatedContentTypeEnum.SocialTips;

                var syndicatedContents = this.ProcessSyndicatedContents(inFuture, country, sort, pagingToken, out recordCount, SyndicatedType);
                return syndicatedContents;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.GetSocialTipsAdminList");
                throw processedError;
            }
        }


        public SyndicatedContent GetSyndicatedContent(int syndicatedContentId)
        {
            try
            {
                var content = this._contentRepository.Get(syndicatedContentId);
                var attachments = this._contentRepository.GetContentAttachmentsById(content.SyndicatedContentId);

                foreach (var syndicatedContentAttachment in attachments)
                {
                    content.SyndicatedContentAttachments.Add(syndicatedContentAttachment);
                }

                return content;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.GetSyndicatedContent");
                throw processedError;
            }
        }

        public SyndicatedContentAttachment GetSyndicatedContentAttachment(int syndicatedContentAttachmentId)
        {
            return this._contentRepository.GetAttachment(syndicatedContentAttachmentId);
        }

        public SyndicatedContentLink GetSyndicatedContentLink(int syndicatedContentLinkId)
        {
            return this._linkRepository.Get(syndicatedContentLinkId);
        }

        public bool DeleteSyndicatedContent(int syndicatedContentId)
        {
            try
            {
                return this._contentRepository.Delete(syndicatedContentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.DeleteSyndicatedContent");
                throw processedError;
            }
        }

        public bool DeleteSyndicatedContentAttachment(int syndicatedContentAttachmentId)
        {
            try
            {
                return this._contentRepository.DeleteSyndicatedContentAttachment(syndicatedContentAttachmentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.DeleteSyndicatedContentAttachment");
                throw processedError;
            }
        }

        public bool DeleteSyndicatedContentLink(int syndicatedContentLinkId)
        {
            try
            {
                return this._linkRepository.Delete(syndicatedContentLinkId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.DeleteSyndicatedContentAttachment");
                throw processedError;
            }
        }

        public SyndicatedContent UpdateSyndicatedContent(SyndicatedContent content)
        {
            try
            {
                return this._contentRepository.Update(content);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.UpdateSyndicatedContent");
                throw processedError;
            }
        }

        public List<FeaturedContentSlim> GetFeaturedContent(SyndicatedContentTypeEnum contentType, BravoVetsCountryEnum country)
        {
            try
            {
                return this._featuredContentRepository.GetFeaturedContent(contentType, country);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.GetFeaturedContent (List)");
                throw processedError;
            }
        }

        public FeaturedContent GetFeaturedContent(int featuredContentId)
        {
            try
            {
                return this._featuredContentRepository.Get(featuredContentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.GetFeaturedContent");
                throw processedError;
            }
        }

        public SyndicatedContent GetMinimalEmptySyndicatedContent(SyndicatedContentTypeEnum contentType, BravoVetsCountryEnum country, string author)
        {
            var nc = new SyndicatedContent
                         {
                             Author = author,
                             BravoVetsCountryId = (int)country,
                             BravoVetsStatusId = (int)BravoVetsStatusEnum.InProcess,
                             ContentText = string.Empty,
                             CreateDateUtc = DateTime.UtcNow,
                             Deleted = false,
                             LinkUrl = string.Empty,
                             ModifiedDateUtc = DateTime.UtcNow,
                             PublishDateUtc = DateTime.UtcNow.AddDays(10),
                             Subject = string.Empty,
                             Summary = string.Empty,
                             SyndicatedContentPostTypeId = (int)SyndicatedContentPostTypeEnum.TextOnly,
                             SyndicatedContentTypeId = (int)contentType,
                             Title = string.Empty
                         };

            return nc;

        }

        public List<SyndicatedContentAttachment> SaveSyndicatedContentAttachments(List<SyndicatedContentAttachment> attachments)
        {
            try
            {
                return attachments.Select(
                    syndicatedContentAttachment => syndicatedContentAttachment.SyndicatedContentAttachmentId > 0 ?
                        this._contentRepository.UpdateAttachment(syndicatedContentAttachment) :
                        this._contentRepository.AddAttachment(syndicatedContentAttachment)).ToList();
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.SaveSyndicatedContentAttachments");
                throw processedError;
            }

        }

        public SyndicatedContentAttachment SaveSyndicatedContentAttachment(SyndicatedContentAttachment attachment)
        {
            try
            {
                return attachment.SyndicatedContentAttachmentId > 0
                           ? this._contentRepository.UpdateAttachment(attachment)
                           : this._contentRepository.AddAttachment(attachment);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentAdminService.SaveSyndicatedContentAttachment");
                throw processedError;
            }
        }

        public SyndicatedContentAttachment AssociateFeaturedContent(int featuredContentId, int syndicatedContentId)
        {
            try
            {
                var fc = this._featuredContentRepository.Get(featuredContentId);

                var sca = new SyndicatedContentAttachment();
                sca.AttachmentExtension = fc.ContentExtension;
                sca.AttachmentFile = fc.ContentFile;
                sca.AttachmentFileName = fc.ContentFileName;
                sca.CreateDateUtc = DateTime.UtcNow;
                sca.DisplayInUi = true;
                sca.ModifiedDateUtc = DateTime.UtcNow;
                sca.SyndicatedContentId = syndicatedContentId;

                return this._contentRepository.AddAttachment(sca);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.AssociateFeaturedContent");
                throw processedError;
            }
        }

        public SyndicatedContentAttachment CreateMinimalEmptyAttachment(int syndicatedContentId, string attachmentExtension)
        {
            var sca = new SyndicatedContentAttachment();
            sca.AttachmentExtension = attachmentExtension;
            sca.AttachmentFileName = string.Format("Test.{0}", attachmentExtension);
            sca.CreateDateUtc = DateTime.UtcNow;
            sca.DisplayInUi = true;
            sca.ModifiedDateUtc = DateTime.UtcNow;
            sca.SyndicatedContentId = syndicatedContentId;

            return sca;
        }

        public FeaturedContent AddUpdateFeaturedContent(FeaturedContent originalContent)
        {
            try
            {
                return originalContent.FeaturedContentId > 0 
                    ? this._featuredContentRepository.Update(originalContent) 
                    : this._featuredContentRepository.Create(originalContent);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentAdminService.AddUpdateFeaturedContent");
                throw processedError;
            }
        }

        #region Private Methods

        private SyndicatedContent CreateSyndicatedContent(SyndicatedContent syndicatedContent)
        {
            var newContent = this._contentRepository.Create(syndicatedContent);
            return newContent;
        }

        private List<SyndicatedContent> ProcessSyndicatedContents(
            bool inFuture,
            BravoVetsCountryEnum country,
            ContentSortEnum sort,
            PagingToken pagingToken,
            out int recordCount,
            SyndicatedContentTypeEnum syndicatedType)
        {
            // flip the ordering for future posts
            if (inFuture && sort == ContentSortEnum.ContentDateAscending)
            {
                sort = ContentSortEnum.ContentDate;
            }
            else if (inFuture && sort == ContentSortEnum.ContentDate)
            {
                sort = ContentSortEnum.ContentDateAscending;
            }

            var syndicatedContents = this._contentRepository.GetSyndicatedContentsAdmin(
                syndicatedType,
                inFuture,
                country,
                sort,
                pagingToken,
                out recordCount);
            return syndicatedContents;
        }

        #endregion


    }
}
