using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Core;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    using BravoVets.Dal;
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;
    using BravoVets.DomainService.RepositoryContract;

    public class SyndicatedContentRepository : RepositoryBase, ISyndicatedContentRepository
    {

        private BravoVetsDbEntities _db;

        private ISyndicatedContentUserRepository _syndicatedContentRepository;

        public SyndicatedContentRepository() : this(new SyndicatedContentUserRepository())
        {
        }

        public SyndicatedContentRepository(ISyndicatedContentUserRepository syndicatedContentUserRepository)
        {
            this._syndicatedContentRepository = syndicatedContentUserRepository;
            this._db = new BravoVetsDbEntities();
        }

        public SyndicatedContent Get(int id)
        {
            var content = this._db.SyndicatedContents.Find(id);
            if (content != null)
            {
                content.SyndicatedContentAttachments = this.GetContentAttachmentsById(id);
            }

            return content;
        }

        public List<SyndicatedContent> GetTrendingTopics(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken)
        {
            var syndicatedContentTypeId = (int)SyndicatedContentTypeEnum.TrendingTopics;

            var skipper = pagingToken.StartRecord;
            var taker = pagingToken.TotalRecords;

            var contents = this.ExtractSyndicatedContent(bravoVetsUserId, bravoVetsCountryId, sort, syndicatedContentTypeId, true, true);

            var slimContents = contents.Skip(skipper).Take(taker).ToList();

            foreach (var syndicatedContent in slimContents)
            {
                syndicatedContent.SyndicatedContentAttachments =
                    this.GetContentAttachmentsById(syndicatedContent.SyndicatedContentId);
            }

            return slimContents;
        }


        public List<SyndicatedContent> GetFilteredSyndicatedContents(
            SyndicatedContentTypeEnum contentType,
            int bravoVetsCountryId,
            int bravoVetsUserId,
            ContentFilterEnum filter,
            ContentSortEnum sort,
            PagingToken pagingToken)
        {
            var syndicatedContentTypeId = (int)contentType;

            var syndicatedContentUserActions = new List<SyndicatedContentUser>();
            bool shouldExcludeHidden = true;
            bool shouldExcludeShared = true;

            // Get the syndicated content that this user has acted upon with the specified type of action: Favorite, share, hide...
            syndicatedContentUserActions = this.ApplyTheFilter(bravoVetsUserId, filter, syndicatedContentUserActions, ref shouldExcludeHidden, ref shouldExcludeShared);

            // Build a query for the syndicated content
            IQueryable<SyndicatedContent> contents = this.ExtractSyndicatedContent(
                bravoVetsUserId,
                bravoVetsCountryId,
                sort,
                syndicatedContentTypeId,
                shouldExcludeHidden,
                shouldExcludeShared);

            // Get the syndicated content id's from the user actions
            var userActionContentIds =
                syndicatedContentUserActions.Select(syndicatedContentUser => syndicatedContentUser.SyndicatedContentId)
                    .ToList();

            // Get the paged version of contents
            var finalContents =
                contents.Where(s => userActionContentIds.Contains(s.SyndicatedContentId))
                    .Skip(pagingToken.StartRecord)
                    .Take(pagingToken.TotalRecords)
                    .ToList();

            // Now add the attachments
            foreach (var syndicatedContent in finalContents)
            {
                syndicatedContent.SyndicatedContentAttachments =
                    this.GetContentAttachmentsById(syndicatedContent.SyndicatedContentId);
            }

            return finalContents;
        }

        public List<SyndicatedContent> GetSocialTips(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken)
        {
            const int SyndicatedContentTypeId = (int)SyndicatedContentTypeEnum.SocialTips;

            var skipper = pagingToken.StartRecord;
            var taker = pagingToken.TotalRecords;

            var contents = this.ExtractSyndicatedContent(bravoVetsUserId, bravoVetsCountryId, sort, SyndicatedContentTypeId, true, true);

            var slimContents = contents.Skip(skipper).Take(taker).ToList();

            foreach (var syndicatedContent in slimContents)
            {
                syndicatedContent.SyndicatedContentAttachments =
                    this.GetContentAttachmentsById(syndicatedContent.SyndicatedContentId);
            }

            return slimContents;
        }

        public List<SyndicatedContent> GetBravectoResources(int bravoVetsUserId, int bravoVetsCountryId, ContentSortEnum sort, PagingToken pagingToken)
        {
            const int SyndicatedContentTypeId = (int)SyndicatedContentTypeEnum.BravectoResources;

            var skipper = pagingToken.StartRecord;
            var taker = pagingToken.TotalRecords;

            var contents = this.ExtractSyndicatedContent(bravoVetsUserId, bravoVetsCountryId, sort, SyndicatedContentTypeId, true, true);

            var slimContents = contents.Skip(skipper).Take(taker).ToList();

            foreach (var syndicatedContent in slimContents)
            {
                syndicatedContent.SyndicatedContentAttachments =
                    this.GetContentAttachmentsById(syndicatedContent.SyndicatedContentId);
            }

            return slimContents;
        }

        public SyndicatedContent Create(SyndicatedContent syndicatedContent)
        {
            try
            {
                var newSyndicatedContent = this._db.SyndicatedContents.Add(syndicatedContent);
                this._db.SaveChanges();
                return newSyndicatedContent;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.Create");
                throw processedError;
            }
        }

        public SyndicatedContent Update(SyndicatedContent syndicatedContent)
        {
            var oldContent =
                this._db.SyndicatedContents.Find(syndicatedContent.SyndicatedContentId);

            if (oldContent != null)
            {
                try
                {
                    this._db.Entry(oldContent).CurrentValues.SetValues(syndicatedContent);
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.Update");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to update");
            }

            return syndicatedContent;
        }

        public bool Delete(int syndicatedContentId)
        {
            var oldContent =
                this._db.SyndicatedContents.Find(syndicatedContentId);
            bool didDelete = false;

            if (oldContent != null)
            {
                try
                {
                    this.DeleteSyndicatedContentAttachmentByParentId(oldContent.SyndicatedContentId);

                    this._db.SyndicatedContentLinks.RemoveRange(oldContent.SyndicatedContentLinks);

                    this._db.SyndicatedContentUsers.RemoveRange(oldContent.SyndicatedContentUsers);

                    this._db.SyndicatedContentTags.RemoveRange(oldContent.SyndicatedContentTags);
                    
                    this.DeleteUnpublishedRelatedQueueItems(syndicatedContentId);

                    this._db.SyndicatedContents.Remove(oldContent);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.Delete");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to delete");
            }

            return didDelete;
        }

        public bool IncrementStatusCount(int syndicatedContentId, ActivityTypeEnum actionType)
        {
            var content =
                this._db.SyndicatedContents.Find(syndicatedContentId);
            bool incremented = false;

            if (content != null)
            {
                try
                {
                    int incrementor;
                    switch (actionType)
                    {
                        case ActivityTypeEnum.FacebookShare:
                        case ActivityTypeEnum.TwitterShare:
                            incrementor = content.NumberOfShares + 1;
                            content.NumberOfShares = incrementor;
                            incremented = true;
                            break;
                        case ActivityTypeEnum.Favorite:
                            incrementor = content.NumberOfFavorites + 1;
                            content.NumberOfFavorites = incrementor;
                            incremented = true;
                            break;
                        case ActivityTypeEnum.Expand:
                            incrementor = content.NumberOfViews + 1;
                            content.NumberOfViews = incrementor;
                            incremented = true;
                            break;

                        default:
                            incremented = false;
                            break;
                    }
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.IncrementStatusCount");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to update");
            }

            return incremented;
        }

        public bool DecrementStatusCount(int syndicatedContentId, ActivityTypeEnum actionType, int trueNumber)
        {
            var content =
                this._db.SyndicatedContents.Find(syndicatedContentId);

            bool decrementer = false;

            if (content != null)
            {
                try
                {
                    switch (actionType)
                    {
                        case ActivityTypeEnum.Favorite:
                            content.NumberOfFavorites = trueNumber;
                            content.IsFavoritedByMe = false;
                            decrementer = true;
                            break;
                        default:
                            decrementer = false;
                            break;
                    }
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.DecrementStatusCount");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to update");
            }

            return decrementer;
        
        }

        public List<SyndicatedContentAttachment> GetContentAttachmentsById(int syndicatedContentId)
        {
            var returnList = 
                this._db.SyndicatedContentAttachments.Where(s => s.SyndicatedContentId == syndicatedContentId).ToList();
            return returnList;
        }

        public bool DeleteSyndicatedContentAttachmentByParentId(int syndicatedContentId)
        {
            try
            {
                var deleteList =
                    this._db.SyndicatedContentAttachments.Where(s => s.SyndicatedContentId == syndicatedContentId).ToList();

                this._db.SyndicatedContentAttachments.RemoveRange(deleteList);

                this._db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.DeleteSyndicatedContentAttachmentByParentId");
                throw processedError;
            }
            
        }

        public SyndicatedContentAttachment AddAttachment(SyndicatedContentAttachment attachment)
        {
            try
            {
                var newSyndicatedContentAttachment = this._db.SyndicatedContentAttachments.Add(attachment);
                this._db.SaveChanges();
                return newSyndicatedContentAttachment;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.AddAttachment");
                throw processedError;
            }
        }

        public SyndicatedContentAttachment UpdateAttachment(SyndicatedContentAttachment attachment)
        {
            try
            {
                var oldAttachment = this._db.SyndicatedContentAttachments.Find(attachment.SyndicatedContentAttachmentId);

                if (oldAttachment != null)
                {
                    try
                    {
                        this._db.Entry(oldAttachment).CurrentValues.SetValues(attachment);
                        this._db.SaveChanges();
                        return oldAttachment;
                    }
                    catch (Exception ex)
                    {
                        var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.UpdateAttachment");
                        throw processedError;
                    }
                }
                else
                {
                    throw new ObjectNotFoundException("Did not find an attachment to update");
                }
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentRepository.UpdateAttachment");
                throw processedError;
            }
        }

        public List<SyndicatedContent> GetSyndicatedContentsAdmin(
            SyndicatedContentTypeEnum contentType,
            bool inFuture,
            BravoVetsCountryEnum country,
            ContentSortEnum sort,
            PagingToken pagingToken,
            out int recordCount)
        {
            IQueryable<SyndicatedContent> contents;

            var skipper = pagingToken.StartRecord;
            var taker = pagingToken.TotalRecords;
            var currentUtcDate = DateTime.UtcNow;

            if (!inFuture)
            {
                contents =
                    this._db.SyndicatedContents.Where(
                        s =>
                        s.SyndicatedContentTypeId == (int)contentType 
                        && s.BravoVetsCountryId == (int)country
                        && s.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                        && s.PublishDateUtc <= currentUtcDate);

                recordCount =
                    this._db.SyndicatedContents.Count(
                        s =>
                        s.SyndicatedContentTypeId == (int)contentType 
                        && s.BravoVetsCountryId == (int)country
                        && s.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                        && s.PublishDateUtc <= currentUtcDate);
            }
            else
            {
                contents =
                    this._db.SyndicatedContents.Where(
                        s =>
                        s.SyndicatedContentTypeId == (int)contentType 
                        && s.BravoVetsCountryId == (int)country
                        && s.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                        && s.PublishDateUtc >= currentUtcDate);

                recordCount =
                    this._db.SyndicatedContents.Count(
                        s =>
                        s.SyndicatedContentTypeId == (int)contentType 
                        && s.BravoVetsCountryId == (int)country
                        && s.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                        && s.PublishDateUtc >= currentUtcDate);
            }

            switch (sort)
            {
                case ContentSortEnum.Topic:
                    contents = contents.OrderBy(s => s.Title);
                    break;
                case ContentSortEnum.TotalFavorites:
                    contents = contents.OrderByDescending(s => s.NumberOfFavorites);
                    break;
                case ContentSortEnum.TotalShares:
                    contents = contents.OrderByDescending(s => s.NumberOfShares);
                    break;
                case ContentSortEnum.TotalViews:
                    contents = contents.OrderByDescending(s => s.NumberOfViews);
                    break;
                case ContentSortEnum.ContentDateAscending:
                    contents = contents.OrderBy(s => s.PublishDateUtc);
                    break;
                default:
                    contents = contents.OrderByDescending(s => s.PublishDateUtc);
                    break;
            }

            return contents.Skip(skipper).Take(taker).ToList();
        }

        public bool DeleteSyndicatedContentAttachment(int syndicatedContentAttachmentId)
        {
            var oldContentAttachment =
                this._db.SyndicatedContentAttachments.Find(syndicatedContentAttachmentId);


            bool didDelete = false;

            if (oldContentAttachment != null)
            {
                try
                {
                    // delete any associated links
                    var associatedLinks =
                        this._db.SyndicatedContentLinks.Where(
                            l => l.SyndicatedContentAttachmentId == oldContentAttachment.SyndicatedContentAttachmentId)
                            .ToList();

                    this._db.SyndicatedContentLinks.RemoveRange(associatedLinks);

                    this._db.SyndicatedContentAttachments.Remove(oldContentAttachment);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex,
                        "SyndicatedContentRepository.DeleteSyndicatedContentAttachment");
                    throw processedError;
                }
            }
            else
            {
                Logger.Info(string.Format("Did not find a syndicatedContentAttachment to delete {0}",
                    syndicatedContentAttachmentId));
            }

            return didDelete;
        }

        public SyndicatedContentAttachment GetAttachment(int syndicatedContentAttachmentId)
        {
            return this._db.SyndicatedContentAttachments.Find(syndicatedContentAttachmentId);
        }

        #region private methods

        private List<SyndicatedContentUser> ApplyTheFilter(int bravoVetsUserId, ContentFilterEnum filter, List<SyndicatedContentUser> syndicatedContentUsers,
            ref bool shouldExcludeHidden,
            ref bool shouldExcludeShared)
        {
            switch (filter)
            {
                case ContentFilterEnum.Favorites:
                    syndicatedContentUsers = this._syndicatedContentRepository.GetByUserAndAction(
                        bravoVetsUserId,
                        (int)ActivityTypeEnum.Favorite);
                    shouldExcludeHidden = true;
                    shouldExcludeShared = true;
                    break;
                case ContentFilterEnum.Hidden:
                    syndicatedContentUsers = this._syndicatedContentRepository.GetByUserAndAction(
                        bravoVetsUserId,
                        (int)ActivityTypeEnum.Hide);
                    shouldExcludeHidden = false;
                    shouldExcludeShared = true;
                    break;
                case ContentFilterEnum.FacebookShare:
                    syndicatedContentUsers = this._syndicatedContentRepository.GetByUserAndAction(
                        bravoVetsUserId,
                        (int)ActivityTypeEnum.FacebookShare);
                    shouldExcludeHidden = true;
                    shouldExcludeShared = false;
                    break;
                case ContentFilterEnum.TwitterShare:
                    syndicatedContentUsers = this._syndicatedContentRepository.GetByUserAndAction(
                        bravoVetsUserId,
                        (int)ActivityTypeEnum.TwitterShare);
                    shouldExcludeHidden = true;
                    shouldExcludeShared = false;
                    break;
                case ContentFilterEnum.GenericShare:
                    syndicatedContentUsers = this._syndicatedContentRepository.GetByUserAndAction(
                        bravoVetsUserId,
                        (int)ActivityTypeEnum.TwitterShare);
                    var additionalUserContent = this._syndicatedContentRepository.GetByUserAndAction(
                        bravoVetsUserId,
                        (int)ActivityTypeEnum.FacebookShare);
                    foreach (
                        var syndicatedContentUser in
                            additionalUserContent.Where(
                                syndicatedContentUser => !syndicatedContentUsers.Contains(syndicatedContentUser)))
                    {
                        syndicatedContentUsers.Add(syndicatedContentUser);
                    }
                    shouldExcludeHidden = true;
                    shouldExcludeShared = false;
                    break;
            }
            return syndicatedContentUsers;
        }

        private IQueryable<SyndicatedContent> ExtractSyndicatedContent(
            int bravoVetsUserId,
            int bravoVetsCountryId,
            ContentSortEnum sort,
            int syndicatedContentTypeId,
            bool excludeHidden,
            bool excludeShared)
        {
            IQueryable<SyndicatedContent> contents;

            var contentIds = new List<int> {-1};
            var currentUtcDate = DateTime.UtcNow;

            if (excludeHidden)
            {
                // find the hidden items for this user
                var scus = this._syndicatedContentRepository.GetByUserAndAction(
                    bravoVetsUserId,
                    (int) ContentFilterEnum.Hidden);

                var hiddentContentIds = scus.Select(scu => scu.SyndicatedContentId).ToList();

                foreach (var hiddentContentId in hiddentContentIds.Where(hiddentContentId => !contentIds.Contains(hiddentContentId)))
                {
                    contentIds.Add(hiddentContentId);
                }
            }

            if (excludeShared)
            {
                // find the shared items for this user
                var scus = this._syndicatedContentRepository.GetByUserAndAction(
                    bravoVetsUserId,
                    (int)ContentFilterEnum.FacebookShare);

                var fbIds = scus.Select(scu => scu.SyndicatedContentId).ToList();

                foreach (var fbId in fbIds.Where(id => !contentIds.Contains(id)))
                {
                    contentIds.Add(fbId);
                }

                // find the shared items for this user
                var sxcus = this._syndicatedContentRepository.GetByUserAndAction(
                    bravoVetsUserId,
                    (int)ContentFilterEnum.TwitterShare);

                var twitIds = sxcus.Select(scu => scu.SyndicatedContentId).ToList();

                foreach (var twitId in twitIds.Where(id => !contentIds.Contains(id)))
                {
                    contentIds.Add(twitId);
                }
            }

            contents =
                this._db.SyndicatedContents.Where(
                    s =>
                        s.BravoVetsCountryId == bravoVetsCountryId
                        && s.SyndicatedContentTypeId == syndicatedContentTypeId 
                        && s.Deleted == false
                        && s.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                        && s.PublishDateUtc <= currentUtcDate
                        && !contentIds.Contains(s.SyndicatedContentId));


            switch (sort)
            {
                case ContentSortEnum.Topic:
                    contents = contents.OrderBy(s => s.Title);
                    break;
                case ContentSortEnum.TotalFavorites:
                    contents = contents.OrderByDescending(s => s.NumberOfFavorites);
                    break;
                case ContentSortEnum.TotalShares:
                    contents = contents.OrderByDescending(s => s.NumberOfShares);
                    break;
                case ContentSortEnum.TotalViews:
                    contents = contents.OrderByDescending(s => s.NumberOfViews);
                    break;
                case ContentSortEnum.ContentDateAscending:
                    contents = contents.OrderBy(s => s.PublishDateUtc);
                    break;
                default:
                    contents = contents.OrderByDescending(s => s.PublishDateUtc);
                    break;
            }
            return contents;
        }

        private void DeleteUnpublishedRelatedQueueItems(int syndicatedContentId)
        {
            var unpublishedQueueContent =
                this._db.QueueContents.Where(
                    q => q.SyndicatedContentId == syndicatedContentId && q.IsPublished == false).ToList();
            if (unpublishedQueueContent.Count > 0)
            {
                var queueContentRepository = new QueueContentRepository();
                var queueIdList = unpublishedQueueContent.Select(queueContent => queueContent.QueueContentId).ToList();
                foreach (var i in queueIdList)
                {
                    queueContentRepository.Delete(i);
                }
            }
        }

        #endregion



    }
}
