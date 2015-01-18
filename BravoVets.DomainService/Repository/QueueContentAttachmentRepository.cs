using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Core;

    using BravoVets.Dal;
    using BravoVets.DomainService.RepositoryContract;
    using DomainObject;

    public class QueueContentAttachmentRepository : RepositoryBase, IQueueContentAttachmentRepository
    {
        private readonly BravoVetsDbEntities _db;

        public QueueContentAttachmentRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public QueueContentAttachment Get(int queueContentAttachmentId)
        {
            return this._db.QueueContentAttachments.Find(queueContentAttachmentId);
        }

        public QueueContentAttachment Create(QueueContentAttachment queueContentAttachment)
        {
            try
            {
                var contentAttachment = this._db.QueueContentAttachments.Add(queueContentAttachment);
                this._db.SaveChanges();
                return contentAttachment;

            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentAttachmentRepository.Create");
                throw processedError;
            }
        }

        public QueueContentAttachment Update(QueueContentAttachment queueContentAttachment)
        {
            var oldAttachment =
                this._db.QueueContentAttachments.Find(queueContentAttachment.QueueContentAttachmentId);

            if (oldAttachment != null)
            {
                try
                {
                    this._db.Entry(oldAttachment).CurrentValues.SetValues(queueContentAttachment);
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "QueueContentAttachmentRepository.Update");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find QueueContentAttachments to update");
            }

            return queueContentAttachment;
        }

        public bool Delete(int queueContentAttachmentId)
        {
            var oldAttachment =
                this._db.QueueContentAttachments.Find(queueContentAttachmentId);
            bool didDelete = false;

            if (oldAttachment != null)
            {
                try
                {
                    this._db.QueueContentAttachments.Remove(oldAttachment);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "QueueContentAttachmentRepository.Delete");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find QueueContentAttachments to delete");
            }

            return didDelete;
        }

        public List<QueueContentAttachment> GetAttachmentsByQueueId(int queueContentId)
        {
            return this._db.QueueContentAttachments.Where(q => q.QueueContentId == queueContentId).ToList();
        }
    }
}
