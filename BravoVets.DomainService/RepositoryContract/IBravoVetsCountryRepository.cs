using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;

namespace BravoVets.DomainService.RepositoryContract
{
    public interface IBravoVetsCountryRepository : IBaseRepository<BravoVetsCountry>
    {
        BravoVetsCountry GetByCountryCode(string isoCountryCode);

        List<BravoVetsCountry> GetAll();
    }
}
