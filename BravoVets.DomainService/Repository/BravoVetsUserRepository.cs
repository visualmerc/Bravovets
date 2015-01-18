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
    public class BravoVetsUserRepository : RepositoryBase, IBravoVetsUserRepository
    {
        private readonly BravoVetsDbEntities _db;

        public BravoVetsUserRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public BravoVetsUser Get(int bravoVetsUserId)
        {
            var user =
                this._db.BravoVetsUsers.Find(bravoVetsUserId);

            return user;
        }

        public BravoVetsUser Get(int bravoVetsUserId, bool fullGraph)
        {
            var user =
                this._db.BravoVetsUsers.Find(bravoVetsUserId);

            return user;
        }

        public BravoVetsUser GetByMerckId(int lfwUserId)
        {
            var user =
                this._db.BravoVetsUsers.FirstOrDefault(bravoVetsUser => bravoVetsUser.MerckId == lfwUserId);

            return user;
        }

        public BravoVetsUser Create(BravoVetsUser user)
        {
            try
            {
                var newUser = this._db.BravoVetsUsers.Add(user);
                this._db.SaveChanges();
                return newUser;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "BravoVetsUserRepository.Create");
                throw processedError;
            }
        }

        public BravoVetsUser Update(BravoVetsUser user)
        {
            var oldUser =
                this._db.BravoVetsUsers.Find(user.BravoVetsUserId);

            if (oldUser != null)
            {
                try
                {
                    this._db.Entry(oldUser).CurrentValues.SetValues(user);
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "BravoVetsUserRepository.Update");
                    throw processedError;
                } 
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to update");
            }

            return user;
        }

        public bool Delete(int bravoVetsUserId)
        {
            var oldUser =
                this._db.BravoVetsUsers.Find(bravoVetsUserId);
            bool didDelete = false;

            if (oldUser != null)
            {
                try
                {
                    oldUser.Deleted = true;
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "BravoVetsUserRepository.Delete");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to delete");
            }

            return didDelete;
        }


        public bool AcceptTermsAndConditions(int bravoVetsUserId)
        {
            var oldUser =
                 this._db.BravoVetsUsers.Find(bravoVetsUserId);
            bool didAccept = false;

            if (oldUser != null)
            {
                try
                {
                    oldUser.AcceptedTandC = true;
                    this._db.SaveChanges();
                    didAccept = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "BravoVetsUserRepository.AcceptTermsAndConditions");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to accept terms and condtions");
            }

            return didAccept;
        }

        public bool ReverseTermsAndConditions(int bravoVetsUserId)
        {
            var oldUser =
                 this._db.BravoVetsUsers.Find(bravoVetsUserId);
            bool didAccept = false;

            if (oldUser != null)
            {
                oldUser.AcceptedTandC = false;
                this._db.SaveChanges();
                didAccept = true;
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to accept terms and condtions");
            }

            return didAccept;
            
        }
    }
}
