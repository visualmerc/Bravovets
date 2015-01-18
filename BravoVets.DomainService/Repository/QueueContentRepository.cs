using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.Dal;
using BravoVets.DomainObject;
using BravoVets.DomainService.RepositoryContract;

namespace BravoVets.DomainService.Repository
{
    using BravoVets.DomainObject.Enum;

    public class QueueContentRepository : RepositoryBase, IQueueContentRepository
    {
        private readonly BravoVetsDbEntities _db;

        public QueueContentRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public QueueContent Get(int queueContentId)
        {
            return this._db.QueueContents.Find(queueContentId);
        }

        public QueueContent Create(QueueContent queueContent)
        {
            try
            {
                var newQueue = this._db.QueueContents.Add(queueContent);
                this._db.SaveChanges();
                return newQueue;

            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentRepository.Create");
                throw processedError;
            }
        }

        public QueueContent Update(QueueContent queueContent)
        {
            var oldQueue =
                this._db.QueueContents.Find(queueContent.QueueContentId);

            if (oldQueue != null)
            {
                try
                {
                    this._db.Entry(oldQueue).CurrentValues.SetValues(queueContent);
                    this._db.SaveChanges();
                    return oldQueue;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentRepository.Update");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find queued content to update");
            }
        }

        public bool Delete(int queueContentid)
        {
            var oldQueue = this._db.QueueContents.Find(queueContentid);
            bool didDelete = false;

            if (oldQueue != null)
            {
                try
                {
                    var attachments =
                        this._db.QueueContentAttachments.Where(q => q.QueueContentId == oldQueue.QueueContentId)
                            .ToList();

                    this._db.QueueContentAttachments.RemoveRange(attachments);

                    this._db.QueueContents.Remove(oldQueue);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentRepository.Delete");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find queued content to delete");
            }

            return didDelete;

        }

        public List<QueueContent> GetQueuedContentForUserByDateRange(int bravoVetsUserId, DateTime startDate, DateTime endDate)
        {
            var queues = this._db.QueueContents.Where(
                q =>
                    q.BravoVetsUserId == bravoVetsUserId 
                    && q.PublishDateUtc >= startDate
                    && q.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                    && q.PublishDateUtc <= endDate).ToList();
                    //&& q.IsPublished == false
            return queues;
        }


        public List<QueueContent> GetUnpublishedQueueContents(DateTime publishDateTime)
        {
            return
                this._db.QueueContents.Where(
                q => q.PublishDateUtc <= publishDateTime 
                    && q.IsPublished == false
                    && q.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active)
                    .ToList();
        }

        public List<QueueContent> GetUnpublishedQueueContents(DateTime publishDateTime, int bravoVetsCountryId)
        {
            return
                this._db.QueueContents.Where(
                q => q.PublishDateUtc <= publishDateTime 
                    && q.IsPublished == false
                    && q.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active
                    && q.BravoVetsCountryId == bravoVetsCountryId)
                    .ToList();
        }


        public bool UpdateContentAsPublished(List<QueueContent> publishedItems)
        {
            try
            {
                var madeUpdate = false;
                var deliverySessionId = Guid.NewGuid();

                foreach (var foundItem in publishedItems.Select(
                    publishedItem => this._db.QueueContents.Find(publishedItem.QueueContentId))
                    .Where(foundItem => foundItem != null))
                {
                    foundItem.IsPublished = true;
                    foundItem.ModifiedDateUtc = DateTime.UtcNow;
                    madeUpdate = true;

                    var qdl = new QueueContentDeliveryLog
                    {
                        DeliverySessionId = deliverySessionId,
                        DeliverySessionStartTimeUtc = DateTime.UtcNow,
                        DeliverySessionTimeUtc = DateTime.UtcNow,
                        PublishError = string.Empty,
                        QueueContentId = foundItem.QueueContentId,
                        WasDelivered = true
                    };

                    this._db.QueueContentDeliveryLogs.Add(qdl);
                }

                if (madeUpdate)
                {
                    this._db.SaveChanges();
                }

                return madeUpdate;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentRepository.UpdateContentAsPublished");
                throw processedError;
            }

        }

        public List<QueueContent> GetPublishedContentsByUser(int bravoVetsUserId, DomainObject.Infrastructure.PagingToken pagingToken)
        {
            var publishedQueue = this._db.QueueContents.Where(q => q.BravoVetsUserId == bravoVetsUserId && q.IsPublished == true)
                .OrderByDescending(q => q.PublishDateUtc)
                    .Skip(pagingToken.StartRecord)
                    .Take(pagingToken.TotalRecords).ToList();
            return publishedQueue;

        }

        public int GetQueuedContentCountForUserByDateRange(int bravoVetsUserId, DateTime startDate, DateTime endDate)
        {
            var contentCount =
                this._db.QueueContents.Count(
                q => q.PublishDateUtc >= startDate 
                    && q.PublishDateUtc <= endDate 
                    && q.BravoVetsUserId == bravoVetsUserId
                    && q.BravoVetsStatusId == (int)BravoVetsStatusEnum.Active);
            return contentCount;
        }

        public void CreateQueueLog(QueueContentDeliveryLog queueContentDeliveryLog)
        {
            try
            {
                var newQueue = this._db.QueueContentDeliveryLogs.Add(queueContentDeliveryLog);
                this._db.SaveChanges();
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentRepository.CreateQueueLog");
                throw processedError;
            }
        }
    }
}
