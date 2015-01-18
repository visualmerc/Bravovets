using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.RepositoryContract
{
    using BravoVets.DomainObject;

    public interface IVeterinarianFacilityRepository : IBaseRepository<VeterinarianFacility>
    {
        List<VeterinarianFacility> GetVeterinarianFacilitiesByVetId(int veterinarianId);
    }
}
