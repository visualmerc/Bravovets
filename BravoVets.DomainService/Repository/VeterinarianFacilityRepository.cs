using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Core;

    using BravoVets.Dal;
    using BravoVets.DomainObject;
    using BravoVets.DomainService.RepositoryContract;

    public class VeterinarianFacilityRepository : RepositoryBase, IVeterinarianFacilityRepository
    {
        private BravoVetsDbEntities _db;

        public VeterinarianFacilityRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public VeterinarianFacility Get(int veterinarianFacilityId)
        {
            return this._db.VeterinarianFacilities.Find(veterinarianFacilityId);
        }

        public VeterinarianFacility Create(VeterinarianFacility veterinarianFacility)
        {
            try
            {
                var newFacility = this._db.VeterinarianFacilities.Add(veterinarianFacility);
                this._db.SaveChanges();
                return newFacility;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianFacilityRepository.Create");
                throw processedError;
            }
        }

        public VeterinarianFacility Update(VeterinarianFacility veterinarianFacility)
        {
            var oldFacility =
                this._db.VeterinarianFacilities.Find(veterinarianFacility.VeterinarianFacilityId);

            if (oldFacility != null)
            {
                try
                {
                    this._db.Entry(oldFacility).CurrentValues.SetValues(veterinarianFacility);
                    this._db.SaveChanges();
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "VeterinarianFacilityRepository.Update");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find facility to update");
            }

            return veterinarianFacility;
        }



        public bool Delete(int veterinarianFacilityId)
        {
            var oldFacility =
                this._db.VeterinarianFacilities.Find(veterinarianFacilityId);
            bool didDelete = false;

            if (oldFacility != null)
            {
                try
                {
                    this._db.VeterinarianFacilities.Remove(oldFacility);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "VeterinarianFacilityRepository.Delete");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find facility to delete");
            }

            return didDelete;
        }

        public List<VeterinarianFacility> GetVeterinarianFacilitiesByVetId(int veterinarianId)
        {
            var facilities = this._db.VeterinarianFacilities.Where(v => v.VeterinarianId == veterinarianId).ToList();
            return facilities;
        }

    }
}
