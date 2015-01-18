using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Contract
{
    using BravoVets.DomainObject;

    public interface ILookupDomainService
    {
        List<BravoVetsCountry> GetBravoVetsCountries();

        List<ActivityType> GetActivityTypes();
    }
}
