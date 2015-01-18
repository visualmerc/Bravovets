using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.Dal;
using BravoVets.DomainService.RepositoryContract;
using BravoVets.DomainObject;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Core;

    public class BravoVetsCountryRepository : RepositoryBase, IBravoVetsCountryRepository
    {
        private BravoVetsDbEntities _db;

        public BravoVetsCountryRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public BravoVetsCountry GetByCountryCode(string isoCountryCode)
        {
            var country =
                this._db.BravoVetsCountries.FirstOrDefault(
                bravoVetsCountry => bravoVetsCountry.CountryIsoCode == isoCountryCode);

            return country;
        }

        public List<BravoVetsCountry> GetAll()
        {
            var countries = this._db.BravoVetsCountries.Where(c => c.Active == true && c.Deleted == false).ToList();
            return countries;
        }
        
        public BravoVetsCountry Get(int id)
        {
            return this._db.BravoVetsCountries.Find(id);
        }

        public BravoVetsCountry Create(BravoVetsCountry country)
        {
            try
            {
                var newFacility = this._db.BravoVetsCountries.Add(country);
                this._db.SaveChanges();
                return newFacility;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "BravoVetsCountryRepository.Create");
                throw processedError;
            }
        }

        public BravoVetsCountry Update(BravoVetsCountry country)
        {
            var oldCountry =
                this._db.BravoVetsCountries.Find(country.BravoVetsCountryId);

            if (oldCountry != null)
            {
                try
                {
                    this._db.Entry(oldCountry).CurrentValues.SetValues(country);
                    this._db.SaveChanges();
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "BravoVetsCountryRepository.Update");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find country to update");
            }

            return country;
        }

        public bool Delete(int countryId)
        {
            var oldCountry =
                this._db.BravoVetsCountries.Find(countryId);
            bool didDelete = false;

            if (oldCountry != null)
            {
                try
                {
                    this._db.BravoVetsCountries.Remove(oldCountry);
                    this._db.SaveChanges();
                    didDelete = true;
                }
                catch (Exception dbException)
                {
                    var exception = GenerateBravoVetsRepositoryException(dbException, "BravoVetsCountryRepository.Delete");
                    throw exception;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find country to delete");
            }

            return didDelete;
        }



    }
}
