using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Service
{
    using BravoVets.DomainObject;
    using BravoVets.DomainService.Contract;
    using BravoVets.DomainService.Repository;
    using BravoVets.DomainService.RepositoryContract;

    public class LookupDomainService : DomainServiceBase, ILookupDomainService
    {
        private IBravoVetsCountryRepository _countryRepository;

        #region ctor 

        public LookupDomainService() : this(new BravoVetsCountryRepository())
        {            
        }

        public LookupDomainService(IBravoVetsCountryRepository countryRepository)
        {
            this._countryRepository = countryRepository;
        }

        #endregion

        public List<BravoVetsCountry> GetBravoVetsCountries()
        {
            try
            {
                return this._countryRepository.GetAll();
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "LookupDomainService.GetBravoVetsCountries");
                throw processedError;
            }
        }

        public List<ActivityType> GetActivityTypes()
        {
            throw new NotImplementedException();
        }
    }
}
