using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Service
{
    using BravoVets.DomainService.Contract;
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;
    using BravoVets.DomainService.Repository;
    using BravoVets.DomainService.RepositoryContract;

    public class SyndicatedContentDomainService : DomainServiceBase, ISyndicatedContentDomainService
    {
        #region private properties

        private ISyndicatedContentRepository _contentRepository;

        private ISyndicatedContentUserRepository _contentUserRepository;

        private const int DefaultRecords = 50;

        private const ContentSortEnum DefaultSort = ContentSortEnum.ContentDate;

        #endregion

        #region ctor

        public SyndicatedContentDomainService() : this(new SyndicatedContentRepository(), new SyndicatedContentUserRepository())
        {
        }

        public SyndicatedContentDomainService(ISyndicatedContentRepository contentRepository) : this(contentRepository, new SyndicatedContentUserRepository())
        {
        }

        public SyndicatedContentDomainService(ISyndicatedContentRepository contentRepository, ISyndicatedContentUserRepository contentUserRepository)
        {
            this._contentRepository = contentRepository;
            this._contentUserRepository = contentUserRepository;
        }

        #endregion

        public List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId)
        {
            return this.GetTrendingTopics(
                bravoVetsUserId,
                bravoVetsCountryId,
                DefaultSort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort)
        {
            return this.GetTrendingTopics(
                bravoVetsUserId,
                bravoVetsCountryId,
                sort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken)
        {
            try
            {
                var trendingTopics = this._contentRepository.GetTrendingTopics(bravoVetsUserId, bravoVetsCountryId, sort, pagingToken);

                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, trendingTopics);

                return trendingTopics;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.GetTrendingTopics");
                throw processedError;
            }
            
        }


        public List<SyndicatedContent> GetFilteredTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentFilterEnum filter)
        {
            return this.GetFilteredTrendingTopics(
                bravoVetsUserId,
                bravoVetsCountryId,
                filter,
                DefaultSort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetFilteredTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentFilterEnum filter, ContentSortEnum sort)
        {
            return this.GetFilteredTrendingTopics(
                bravoVetsUserId,
                bravoVetsCountryId,
                filter,
                sort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetFilteredTrendingTopics(
            int bravoVetsCountryId,
            int bravoVetsUserId,
            ContentFilterEnum filter,
            ContentSortEnum sort,
            PagingToken pagingToken)
        {
            try
            {
                var trendingTopics =
                    this._contentRepository.GetFilteredSyndicatedContents(
                        SyndicatedContentTypeEnum.TrendingTopics,
                        bravoVetsCountryId,
                        bravoVetsUserId,
                        filter,
                        sort,
                        pagingToken);

                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, trendingTopics);

                return trendingTopics;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentDomainService.GetFilteredTrendingTopics");
                throw processedError;
            }
        }

        public bool UserHasHiddenTrendingTopics(int bravoVetsUserId)
        {
            try
            {
                return this._contentUserRepository.UserHasContent(SyndicatedContentTypeEnum.TrendingTopics,
                    ContentFilterEnum.Hidden, bravoVetsUserId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentDomainService.UserHasHiddenTrendingTopics");
                throw processedError;
            }
        }

        public bool UserHasSharedTrendingTopics(int bravoVetsUserId)
        {
            try
            {
                return this._contentUserRepository.UserHasContent(SyndicatedContentTypeEnum.TrendingTopics,
                    ContentFilterEnum.GenericShare, bravoVetsUserId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentDomainService.UserHasSharedTrendingTopics");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetFilteredSocialTips(int bravoVetsCountryId, int bravoVetsUserId, ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken)
        {
            try
            {
                var socialTips =
                    this._contentRepository.GetFilteredSyndicatedContents(
                        SyndicatedContentTypeEnum.SocialTips,
                        bravoVetsCountryId,
                        bravoVetsUserId,
                        filter,
                        sort,
                        pagingToken);

                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, socialTips);

                return socialTips;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentDomainService.GetFilteredSocialTips");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetFilteredBravectoResources(int bravoVetsCountryId, int bravoVetsUserId, ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken)
        {
            try
            {
                var bravectoResources =
                    this._contentRepository.GetFilteredSyndicatedContents(
                        SyndicatedContentTypeEnum.BravectoResources,
                        bravoVetsCountryId,
                        bravoVetsUserId,
                        filter,
                        sort,
                        pagingToken);

                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, bravectoResources);

                return bravectoResources;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "SyndicatedContentDomainService.GetFilteredBravectoResources");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId)
        {
            return this.GetSocialTips(
                bravoVetsUserId,
                bravoVetsCountryId,
                DefaultSort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort)
        {
            return this.GetSocialTips(
                bravoVetsUserId,
                bravoVetsCountryId,
                sort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken)
        {
            try
            {
                var socialTips = this._contentRepository.GetSocialTips(bravoVetsUserId, bravoVetsCountryId, sort, pagingToken);
                
                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, socialTips);

                return socialTips;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.GetFilteredSyndicatedContents");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId)
        {
            return this.GetBravectoResources(
                bravoVetsUserId,
                bravoVetsCountryId,
                DefaultSort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort)
        {
            return this.GetBravectoResources(
                bravoVetsUserId,
                bravoVetsCountryId,
                sort,
                new PagingToken { StartRecord = 0, TotalRecords = DefaultRecords });
        }

        public List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken)
        {
            try
            {
                var bravectoResources = this._contentRepository.GetBravectoResources(bravoVetsUserId, bravoVetsCountryId, sort, pagingToken);

                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, bravectoResources);

                return bravectoResources;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.GetBravectoResources");
                throw processedError;
            }
        }

        public SyndicatedContent ShareContent(int bravoVetsUserId, int syndicatedContentId, SocialPlatformEnum platform)
        {
            try
            {

                var actionEnum = ActivityTypeEnum.TwitterShare;
                int actionId = 0;
                
                var s = BuildSyndicatedContentUser(bravoVetsUserId, syndicatedContentId);
                switch (platform)
                {
                    case SocialPlatformEnum.Facebook:
                        actionId = (int)ActivityTypeEnum.FacebookShare;
                        s.ActivityTypeId = actionId;
                        actionEnum = ActivityTypeEnum.FacebookShare;
                        break;
                    case SocialPlatformEnum.Twitter:
                        actionId = (int)ActivityTypeEnum.TwitterShare;
                        s.ActivityTypeId = actionId;
                        actionEnum = ActivityTypeEnum.TwitterShare;
                        break;
                }

                var shares = this._contentUserRepository.GetByUserActionContent(bravoVetsUserId, actionId, syndicatedContentId);

                if (shares != null)
                {
                    // Don't keep inserting records, if this person has shared this content before
                    return this._contentRepository.Get(syndicatedContentId);
                }

                this._contentUserRepository.Create(s);

                this._contentRepository.IncrementStatusCount(syndicatedContentId, actionEnum);

                return this._contentRepository.Get(syndicatedContentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.ShareContent");
                throw processedError;
            }
        }


        public void FavoriteContent(int bravoVetsUserId, int syndicatedContentId)
        {
            try
            {
                const int favorite = (int) ActivityTypeEnum.Favorite;
                var favorites = this._contentUserRepository.GetByUserActionContent(bravoVetsUserId, favorite, syndicatedContentId);

                if (favorites != null)
                {
                    return;
                }

                var s = BuildSyndicatedContentUser(bravoVetsUserId, syndicatedContentId);
                s.ActivityTypeId = favorite;
                this._contentUserRepository.Create(s);

                this._contentRepository.IncrementStatusCount(syndicatedContentId, ActivityTypeEnum.Favorite);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.FavoriteContent");
                throw processedError;
            }
        }

        public void ViewContent(int bravoVetsUserId, int syndicatedContentId)
        {
            try
            {

                const int expand = (int) ActivityTypeEnum.Expand;

                if (this._contentUserRepository.GetByUserActionContent(bravoVetsUserId, expand, syndicatedContentId) == null)
                {
                    var s = BuildSyndicatedContentUser(bravoVetsUserId, syndicatedContentId);
                    s.ActivityTypeId = expand;
                    this._contentUserRepository.Create(s);

                    this._contentRepository.IncrementStatusCount(syndicatedContentId, ActivityTypeEnum.Expand);
                }
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.ViewContent");
                throw processedError;
            }
        }

        public List<SyndicatedContentAttachment> GetAttachmentsById(int syndicatedContentId)
        {
            try
            {
               return this._contentRepository.GetContentAttachmentsById(syndicatedContentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.GetAttachmentsById");
                throw processedError;
            }
        }

        public void HideContent(int bravoVetsUserId, int syndicatedContentId)
        {
            try
            {
                const int hide = (int) ActivityTypeEnum.Hide;

                if (this._contentUserRepository.GetByUserActionContent(bravoVetsUserId, hide, syndicatedContentId) == null)
                {
                    var s = BuildSyndicatedContentUser(bravoVetsUserId, syndicatedContentId);
                    s.ActivityTypeId = hide;
                    this._contentUserRepository.Create(s);
                }
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.HideContent");
                throw processedError;
            }
        }

        public bool UnHideContent(int bravoVetsUserId, int syndicatedContentId)
        {
            try
            {
                const int Hider = (int)ActivityTypeEnum.Hide;
                var didUnhide = this._contentUserRepository.DeleteByUserActionContent(bravoVetsUserId, Hider, syndicatedContentId);
                return didUnhide;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.UnHideContent");
                throw processedError;
            }
        }

        public SyndicatedContent CreateSyndicatedContent(SyndicatedContent syndicatedContent)
        {
            try
            {
                return this._contentRepository.Create(syndicatedContent);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.CreateSyndicatedContent");
                throw processedError;
            }

        }

        public SyndicatedContent GetSyndicatedContent(int bravoVetsUserId, int syndicatedContentId)
        {
            try
            {
                var content = this._contentRepository.Get(syndicatedContentId);

                this.PopulateSyndicatedContentWithUserInfo(bravoVetsUserId, new List<SyndicatedContent> {content});

                return content;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.GetSyndicatedContent");
                throw processedError;
            }
        }

        public bool UnfavoriteContent(int bravoVetsUserId, int syndicatedContentId)
        {
            try
            {
                const int favoriter = (int)ActivityTypeEnum.Favorite;
                var didDelete = this._contentUserRepository.DeleteByUserActionContent(bravoVetsUserId, favoriter, syndicatedContentId);

                var trueNumber = this._contentUserRepository.GetCountByActionContent(favoriter, syndicatedContentId);

                this._contentRepository.DecrementStatusCount(syndicatedContentId, ActivityTypeEnum.Favorite, trueNumber);

                return didDelete;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.FavoriteContent");
                throw processedError;
            }
        }


        public SyndicatedContentAttachment SaveAttachment(SyndicatedContentAttachment attachment)
        {
            try
            {
                return this._contentRepository.AddAttachment(attachment);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "SyndicatedContentDomainService.AddAttachment");
                throw processedError;
            }
        }

        #region private methods

        private static SyndicatedContentUser BuildSyndicatedContentUser(int bravoVetsUserId, int syndicatedContentId)
        {
            var s = new SyndicatedContentUser();
            s.BravoVetsUserId = bravoVetsUserId;
            s.CreateDateUtc = DateTime.UtcNow;
            s.BravoVetsStatusId = (int)BravoVetsStatusEnum.Active;
            s.Deleted = false;
            s.ModifiedDateUtc = DateTime.UtcNow;
            s.SyndicatedContentId = syndicatedContentId;
            return s;
        }

        private void PopulateSyndicatedContentWithUserInfo(int bravoVetsUserId, List<SyndicatedContent> syndicatedContents)
        {
            // extract the SyndicatedContentId's from the trending topics
            var contentIds = syndicatedContents.Select(scu => scu.SyndicatedContentId).ToList();
            var userContent = this._contentUserRepository.GetByUserAndScope(bravoVetsUserId, contentIds);

            foreach (var scuz in userContent)
            {
                var parentContent = syndicatedContents.FirstOrDefault(s => s.SyndicatedContentId == scuz.SyndicatedContentId);

                if (parentContent == null)
                {
                    continue;
                }

                switch (scuz.ActivityTypeId)
                {
                    case (int)ActivityTypeEnum.Expand:
                        parentContent.IsViewedByMe = true;
                        break;
                    case (int)ActivityTypeEnum.Favorite:
                        parentContent.IsFavoritedByMe = true;
                        break;
                    case (int)ActivityTypeEnum.FacebookShare:
                    case (int)ActivityTypeEnum.TwitterShare:
                        parentContent.IsSharedByMe = true;
                        break;
                    // TODO: factor in the social calendar / queue
                }
            }
        }

        #endregion 

    }
}
