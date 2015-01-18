using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject.Infrastructure;
using BravoVets.DomainService.Contract;
using BravoVets.DomainService.Repository;
using BravoVets.DomainService.RepositoryContract;

namespace BravoVets.DomainService.Service
{
    using System.Security.Cryptography;

    using BravoVets.DomainObject.Enum;

    using DomainObject;

    using QueueContentRepository = BravoVets.DomainService.Repository.QueueContentRepository;

    public class PublishQueueDomainService : DomainServiceBase, IPublishQueueDomainService
    {
        private readonly IQueueContentRepository _queueRepository;

        private readonly ISyndicatedContentRepository _contentRepository;

        private readonly IQueueContentAttachmentRepository _attachmentRepository;

        #region ctor

        public PublishQueueDomainService() :
            this(new QueueContentRepository(), new SyndicatedContentRepository(), new QueueContentAttachmentRepository())
        {
        }

        public PublishQueueDomainService(IQueueContentRepository queueRepository) :
            this(queueRepository, new SyndicatedContentRepository(), new QueueContentAttachmentRepository())
        {
        }

        public PublishQueueDomainService(
            IQueueContentRepository queueRepository,
            ISyndicatedContentRepository contentRepository) :
            this(queueRepository, contentRepository, new QueueContentAttachmentRepository())
        {
        }

        public PublishQueueDomainService(
            IQueueContentRepository queueRepository,
            ISyndicatedContentRepository contentRepository,
            IQueueContentAttachmentRepository attachmentRepository)
        {
            this._queueRepository = queueRepository;
            this._contentRepository = contentRepository;
            this._attachmentRepository = attachmentRepository;
        }

        #endregion

        public QueueContent QueueContentForPublish(QueueContent queue)
        {
            try
            {
                if (queue.QueueContentId > 0)
                {
                    this._queueRepository.Update(queue);
                }
                else
                {
                    this._queueRepository.Create(queue);
                }
                return queue;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.QueueContentForPublish");
                throw processedError;
            }
        }

        public QueueContent CloneQueuedContent(int queueContentId)
        {
            try
            {
                var source = this._queueRepository.Get(queueContentId);
                var dest = new QueueContent
                               {
                                   AccessCode = source.AccessCode,
                                   AccountName = source.AccountName,
                                   BravoVetsCountryId = source.BravoVetsCountryId,
                                   BravoVetsStatusId = source.BravoVetsStatusId,
                                   BravoVetsUserId = source.BravoVetsUserId,
                                   ContentText = source.ContentText,
                                   CreateDateUtc = DateTime.UtcNow,
                                   Deleted = false,
                                   IsPublished = false,
                                   LinkUrl = source.LinkUrl,
                                   ModifiedDateUtc = DateTime.UtcNow,
                                   PlatformPublishId = source.PlatformPublishId,
                                   PublishDateUtc = DateTime.UtcNow.AddDays(15),
                                   PublishError = string.Empty,
                                   SyndicatedContentId = source.SyndicatedContentId,
                                   SyndicatedContentPostTypeId = source.SyndicatedContentPostTypeId
                               };

                var newQueue = this._queueRepository.Create(dest);

                foreach (
                    var newAttach in
                        source.QueueContentAttachments.Select(
                            qa =>
                            new QueueContentAttachment
                                {
                                    AttachmentExtension = qa.AttachmentExtension,
                                    AttachmentFile = qa.AttachmentFile,
                                    AttachmentFileName = qa.AttachmentFileName,
                                    CreateDateUtc = DateTime.UtcNow,
                                    ModifiedDateUtc = DateTime.UtcNow,
                                    QueueContentId = newQueue.QueueContentId,
                                    SyndicatedContentAttachmentId =
                                        qa.SyndicatedContentAttachmentId
                                })
                            .Select(attach => this._attachmentRepository.Create(attach)))
                {
                    newQueue.QueueContentAttachments.Add(newAttach);
                }

                return newQueue;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "PublishQueueDomainService.CloneQueuedContent");
                throw processedError;
            }

        }


        public bool DequeueContent(int queueContentId)
        {
            try
            {
                var attachments = this._attachmentRepository.GetAttachmentsByQueueId(queueContentId);

                var deleteList = attachments.Select(q => q.QueueContentAttachmentId).ToList();
                foreach (var i in deleteList)
                {
                    this._attachmentRepository.Delete(i);
                }

                return this._queueRepository.Delete(queueContentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.DequeueContent");
                throw processedError;
            }
        }

        public QueueContent GetQueuedMessage(int queueContentId)
        {
            try
            {
                var queuedMessage = this._queueRepository.Get(queueContentId);
                queuedMessage.QueueContentAttachments = this._attachmentRepository.GetAttachmentsByQueueId(queueContentId);

                return queuedMessage;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetQueuedMessage");
                throw processedError;
            }
        }

        public Dictionary<DateTime, List<QueueContent>> GetQueuedContentForUserByDay(int bravoVetsUserId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var processEndDate = endDate.Date.AddDays(1).AddSeconds(-1);
                var contentQueue = new Dictionary<DateTime, List<QueueContent>>();
                var queuedItems = this._queueRepository.GetQueuedContentForUserByDateRange(bravoVetsUserId, startDate,
                    processEndDate);
                TimeSpan diffSpan = endDate - startDate;
                var daysDiff = (int)diffSpan.TotalDays;

                for (int i = 0; i <= daysDiff; i++)
                {
                    var currentDate = startDate.AddDays(i);
                    var dayContent = queuedItems.Where(q => q.PublishDateUtc.Date == currentDate.Date).ToList();
                    contentQueue.Add(currentDate, dayContent);
                }

                return contentQueue;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetQueuedContentForUserByDay");
                throw processedError;
            }
        }

        public Dictionary<DateTime, int> GetQueuedContentCountForUserByMonth(
            int bravoVetsUserId,
            DateTime startMonth,
            DateTime endMonth)
        {
            try
            {
                var goodDictionary = new Dictionary<DateTime, int>();
                var monthDiff = (endMonth.Month - startMonth.Month) + 12 * (endMonth.Year - startMonth.Year);

                for (int i = 0; i <= monthDiff; i++)
                {
                    var baseMonth = startMonth.AddMonths(i);
                    var startDate = new DateTime(baseMonth.Year, baseMonth.Month, 1);
                    var endDate = startDate.AddMonths(1).AddSeconds(-1);
                    var queueCount = this._queueRepository.GetQueuedContentCountForUserByDateRange(
                        bravoVetsUserId,
                        startDate,
                        endDate);
                    goodDictionary.Add(startDate, queueCount);
                }
                return goodDictionary;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetQueuedContentCountForUserByMonth");
                throw processedError;
            }
        }

        public List<QueueContent> GetQueuedContentForUser(int bravoVetsUserId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var processEndDate = endDate.Date.AddDays(1).AddSeconds(-1);
                var queuedItems = this._queueRepository.GetQueuedContentForUserByDateRange(bravoVetsUserId, startDate,
                    processEndDate);
                return queuedItems;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetQueuedContentForUser");
                throw processedError;
            }
        }

        public List<QueueContent> GetAllQueuedContentForPublish(DateTime currentUtcDateTime, out Guid deliverySessionId)
        {
            deliverySessionId = Guid.NewGuid();
            try
            {
                var unpublishedQueueContents = this._queueRepository.GetUnpublishedQueueContents(currentUtcDateTime);
                foreach (var queueContent in unpublishedQueueContents)
                {
                    queueContent.QueueContentAttachments =
                        this._attachmentRepository.GetAttachmentsByQueueId(queueContent.QueueContentId);
                }
                return unpublishedQueueContents;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetAllQueuedContentForPublish");
                throw processedError;
            }
        }

        public List<QueueContent> GetAllQueuedContentForPublish(DateTime currentUtcDateTime, int bravoVetsCountryId, out Guid deliverySessionId)
        {
            deliverySessionId = Guid.NewGuid();
            try
            {
                var unpublishedQueueContents = this._queueRepository.GetUnpublishedQueueContents(currentUtcDateTime, bravoVetsCountryId);
                foreach (var queueContent in unpublishedQueueContents)
                {
                    queueContent.QueueContentAttachments =
                        this._attachmentRepository.GetAttachmentsByQueueId(queueContent.QueueContentId);
                }
                return unpublishedQueueContents;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetAllQueuedContentForPublish");
                throw processedError;
            }
        }

        public bool MarkContentAsPublished(List<QueueContent> deliveredQueue)
        {
            try
            {
                return this._queueRepository.UpdateContentAsPublished(deliveredQueue);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.MarkContentAsPublished");
                throw processedError;
            }
        }

        public bool AddContentToErroredQueue(List<QueueContent> erroredQueue)
        {
            throw new NotImplementedException();
        }

        public void AssociateSyndicatedContentAttachments(int queueContentId, int syndicatedContentId)
        {
            try
            {
                var attachments = this._contentRepository.GetContentAttachmentsById(syndicatedContentId);

                foreach (var sca in attachments)
                {
                    var qa = new QueueContentAttachment();
                    qa.AttachmentExtension = sca.AttachmentExtension;
                    qa.AttachmentFile = sca.AttachmentFile;
                    qa.AttachmentFileName = sca.AttachmentFileName;
                    qa.CreateDateUtc = sca.CreateDateUtc;
                    qa.ModifiedDateUtc = DateTime.UtcNow;
                    qa.QueueContentId = queueContentId;
                    qa.SyndicatedContentAttachmentId = syndicatedContentId;

                    this._attachmentRepository.Create(qa);
                }
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.AssociateSyndicatedContentAttachment");
                throw processedError;
            }
        }

        public void DeleteAttachment(int queueContentAttachmentId)
        {
            try
            {
                this._attachmentRepository.Delete(queueContentAttachmentId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.DeleteAttachment");
                throw processedError;
            }
        }

        public void AddNewAttachments(List<QueueContentAttachment> attachment)
        {
            try
            {
                foreach (var queueContentAttachment in attachment)
                {
                    this._attachmentRepository.Create(queueContentAttachment);
                }
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.SaveSyndicatedContentAttachments");
                throw processedError;
            }
        }

        public List<QueueContent> GetPublishedContentsByUser(int bravoVetsUserId, PagingToken pagingToken)
        {
            try
            {
                var publishedContent = this._queueRepository.GetPublishedContentsByUser(bravoVetsUserId, pagingToken);
                foreach (var queueContent in publishedContent)
                {
                    queueContent.QueueContentAttachments =
                        this._attachmentRepository.GetAttachmentsByQueueId(queueContent.QueueContentId);
                }
                return publishedContent;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "PublishQueueDomainService.GetPublishedContentsByUser");
                throw processedError;
            }
        }


        public QueueContent GetMinimalEmptyQueueContent(BravoVetsCountryEnum country, int bravoVetsUserId)
        {
            var tc = new QueueContent();
            tc.BravoVetsCountryId = (int)country;
            tc.BravoVetsStatusId = (int)BravoVetsStatusEnum.InProcess;
            tc.BravoVetsUserId = bravoVetsUserId;
            tc.ContentText = string.Empty;
            tc.CreateDateUtc = DateTime.UtcNow;
            tc.Deleted = false;
            tc.IsPublished = false;
            tc.ModifiedDateUtc = DateTime.UtcNow;
            tc.PlatformPublishId = (int)SocialPlatformEnum.Facebook;
            tc.PublishDateUtc = DateTime.UtcNow.AddDays(7);
            tc.SyndicatedContentPostTypeId = (int)SyndicatedContentPostTypeEnum.Original;

            return tc;
        }



        public void LogQueuedContentDelivery(
            Guid deliverySessionId,
            int queueContentId,
            bool wasSuccess,
            string errorMessage)
        {
            var q = new QueueContentDeliveryLog
                        {
                            DeliverySessionId = deliverySessionId,
                            DeliverySessionStartTimeUtc = DateTime.UtcNow,
                            DeliverySessionTimeUtc = DateTime.UtcNow,
                            PublishError = errorMessage,
                            QueueContentId = queueContentId,
                            WasDelivered = wasSuccess
                        };
            this._queueRepository.CreateQueueLog(q);

            var queueContent = this._queueRepository.Get(queueContentId);

            if (wasSuccess)
            {
                queueContent.PublishDateUtc = DateTime.UtcNow;
                queueContent.IsPublished = true;
                queueContent.PublishError = string.Empty;
                this._queueRepository.Update(queueContent);
            }
            else
            {
                queueContent.IsPublished = false;
                queueContent.PublishError = errorMessage;
                this._queueRepository.Update(queueContent);
            }
        }


        public QueueContentAttachment GetQueueContentAttachment(int queueContentAttachmentId)
        {
            return this._attachmentRepository.Get(queueContentAttachmentId);
        }

    }
}
