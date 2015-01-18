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

    public class SyndicatedContentLinkRepository : RepositoryBase, ISyndicatedContentLinkRepository
    {
        private readonly BravoVetsDbEntities _db;

        public SyndicatedContentLinkRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public SyndicatedContentLink Get(int id)
        {
            return this._db.SyndicatedContentLinks.Find(id);
        }

        public SyndicatedContentLink Create(SyndicatedContentLink syndicatedContentLink)
        {
            try
            {
                var newLink = this._db.SyndicatedContentLinks.Add(syndicatedContentLink);
                this._db.SaveChanges();
                return newLink;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentLinkRepository.Create");
                throw processedError;
            }
        }

        public SyndicatedContentLink Update(SyndicatedContentLink syndicatedContentLink)
        {
            var oldLink =
                this._db.SyndicatedContentLinks.Find(syndicatedContentLink.SyndicatedContentLinkId);

            if (oldLink != null)
            {
                try
                {
                    this._db.Entry(oldLink).CurrentValues.SetValues(syndicatedContentLink);
                    this._db.SaveChanges();
                    return oldLink;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentLinkRepository.Update");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find syndicated content link to update");
            }
        }

        public bool Delete(int syndicatedContentLinkId)
        {
            var oldLink = this._db.SyndicatedContentLinks.Find(syndicatedContentLinkId);
            bool didDelete = false;

            if (oldLink != null)
            {
                try
                {
                    if (oldLink.SyndicatedContentAttachmentId.HasValue)
                    {
                        var associatedAttachment =
                            this._db.SyndicatedContentAttachments.Find(oldLink.SyndicatedContentAttachmentId.Value);
                        if (associatedAttachment != null)
                        {
                            this._db.SyndicatedContentAttachments.Remove(associatedAttachment);
                        }
                    }

                    this._db.SyndicatedContentLinks.Remove(oldLink);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentLinkRepository.Delete");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find syndicated content link to delete");
            }

            return didDelete;
        }
    }
}
