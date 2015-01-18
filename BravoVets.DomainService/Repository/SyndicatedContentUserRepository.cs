using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject.Enum;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Core;

    using BravoVets.Dal;
    using BravoVets.DomainService.RepositoryContract;
    using BravoVets.DomainObject;

    public class SyndicatedContentUserRepository : RepositoryBase, ISyndicatedContentUserRepository
    {
        private BravoVetsDbEntities _db;

        public SyndicatedContentUserRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public List<SyndicatedContentUser> GetByUser(int bravoVetsUserId)
        {
            return this._db.SyndicatedContentUsers.Where(s => s.BravoVetsUserId == bravoVetsUserId).ToList();
        }
        public List<SyndicatedContentUser> GetByUserAndScope(int bravoVetsUserId, List<int> eligibleList)
        {
            return this._db.SyndicatedContentUsers.Where(
                s => s.BravoVetsUserId == bravoVetsUserId
                    && eligibleList.Contains(s.SyndicatedContentId)
                ).ToList();
        }


        public List<SyndicatedContentUser> GetByUserAndAction(int bravoVetsUserId, int activityTypeId)
        {
            return this._db.SyndicatedContentUsers.Where(s => s.BravoVetsUserId == bravoVetsUserId && s.ActivityTypeId == activityTypeId).ToList();
        }

        public SyndicatedContentUser GetByUserActionContent(int bravoVetsUserId, int activityTypeId, int syndicatedContentId)
        {
            return this._db.SyndicatedContentUsers.FirstOrDefault(
                s => s.BravoVetsUserId == bravoVetsUserId
                     && s.ActivityTypeId == activityTypeId
                     && s.SyndicatedContentId == syndicatedContentId);
        }
        
        public SyndicatedContentUser Get(int syndicatedContentUserid)
        {
            return this._db.SyndicatedContentUsers.Find(syndicatedContentUserid);
        }

        public SyndicatedContentUser Create(SyndicatedContentUser syndicatedContentUser)
        {
            try
            {
                var newSyndicatedContentUser = this._db.SyndicatedContentUsers.Add(syndicatedContentUser);
                this._db.SaveChanges();
                return newSyndicatedContentUser;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentUserRepository.Create");
                throw processedError;
            }
        }

        public DomainObject.SyndicatedContentUser Update(DomainObject.SyndicatedContentUser obj)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            var userContent = this._db.SyndicatedContentUsers.Find(id);

            bool didDelete = false;

            if (userContent != null)
            {
                try
                {
                    this._db.SyndicatedContentUsers.Remove(userContent);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentUserRepository.Delete");
                    throw processedError;
                }
            }

            return didDelete;
        }

        public bool DeleteByUserActionContent(int bravoVetsUserId, int activityTypeId, int syndicatedContentId)
        {
            var userContent =
                this._db.SyndicatedContentUsers.FirstOrDefault(b => b.ActivityTypeId == activityTypeId && b.BravoVetsUserId == bravoVetsUserId && b.SyndicatedContentId == syndicatedContentId);

            bool didDelete = false;

            if (userContent != null)
            {
                try
                {
                    this._db.SyndicatedContentUsers.Remove(userContent);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentUserRepository.DeleteByUserActionContent");
                    throw processedError;
                }
            }
 
            return didDelete;
        }

        public bool UserHasContent(SyndicatedContentTypeEnum contentType, ContentFilterEnum filter, int bravoVetsUserId)
        {
            try
            {
                bool hasContent;

                switch (filter)
                {
                    case ContentFilterEnum.Hidden:
                        hasContent = this._db.SyndicatedContentUsers.Any(s => s.ActivityTypeId == (int)ActivityTypeEnum.Hide
                                                                 && s.BravoVetsUserId == bravoVetsUserId
                                                                 && s.SyndicatedContent.SyndicatedContentTypeId == (int)contentType);
                        return hasContent;
                        break;
                    case ContentFilterEnum.GenericShare:
                        hasContent = this._db.SyndicatedContentUsers.Any(s =>
                            (s.ActivityTypeId == (int)ActivityTypeEnum.TwitterShare || s.ActivityTypeId == (int)ActivityTypeEnum.FacebookShare)
                                                                 && s.BravoVetsUserId == bravoVetsUserId
                                                                 && s.SyndicatedContent.SyndicatedContentTypeId == (int)contentType);
                        return hasContent;
                        break;
                    default:
                        return false;
                        break;
                }
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "SyndicatedContentUserRepository.UserHasContent");
                throw processedError;
            } 
            

        }

        public int GetCountByActionContent(int activityTypeId, int syndicatedContentId)
        {
            return
                this._db.SyndicatedContentUsers.Count(s => s.ActivityTypeId == activityTypeId && s.SyndicatedContentId == syndicatedContentId);
        }
    }
}
