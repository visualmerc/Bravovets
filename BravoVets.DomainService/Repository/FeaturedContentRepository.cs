using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Core;

    using BravoVets.Dal;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainService.RepositoryContract;
    using DomainObject;

    public class FeaturedContentRepository : RepositoryBase, IFeaturedContentRepository
    {
        private BravoVetsDbEntities _db;

        public FeaturedContentRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public FeaturedContent Get(int id)
        {
            return this._db.FeaturedContents.Find(id);
        }

        public FeaturedContent Create(FeaturedContent featuredContent)
        {
            try
            {
                var newContent = this._db.FeaturedContents.Add(featuredContent);
                this._db.SaveChanges();
                return newContent;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "FeaturedContentRepository.Create");
                throw processedError;
            }
        }

        public FeaturedContent Update(FeaturedContent featuredContent)
        {
            var oldContent =
                this._db.FeaturedContents.Find(featuredContent.FeaturedContentId);

            if (oldContent != null)
            {
                try
                {
                    this._db.Entry(oldContent).CurrentValues.SetValues(featuredContent);
                    this._db.SaveChanges();
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "FeaturedContentRepository.Update");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find content to update");
            }

            return featuredContent;
        }

        public bool Delete(int featuredContentId)
        {
            var oldContent =
                this._db.FeaturedContents.Find(featuredContentId);
            bool didDelete = false;

            if (oldContent != null)
            {
                try
                {
                    this._db.FeaturedContents.Remove(oldContent);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "FeaturedContentRepository.Delete");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find featuredContent to update");
            }

            return didDelete;
        }

        public List<FeaturedContentSlim> GetFeaturedContent(SyndicatedContentTypeEnum contentType, BravoVetsCountryEnum country)
        {
            var fullList = this._db.FeaturedContents.Where(f => f.SyndicatedContentTypeId == (int)contentType && f.BravoVetsCountryId == (int)country);

            return
                fullList.Select(
                    fc =>
                    new FeaturedContentSlim
                        {
                            ContentExtension = fc.ContentExtension,
                            ContentFileName = fc.ContentFileName,
                            ContentThumbnail = fc.ContentThumbnail,
                            FeaturedContentId = fc.FeaturedContentId,
                            SyndicatedContentTypeId = fc.SyndicatedContentTypeId
                        }).ToList();
        }
    }
}
