using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Contract
{
    using BravoVets.DomainObject;

    public interface IVeterinarianDomainService
    {
        Veterinarian GetVeterinarian(int veterinarianId);

        Veterinarian AddOrUpdateVeterinarian(Veterinarian veterinarian);

        bool DeleteVeterinarian(int veterinarianId);

        VeterinarianFacility AddOrUpdateFacility(VeterinarianFacility veterinarianFacility);

        List<VeterinarianFacility> AddOrUpdateFacilities(List<VeterinarianFacility> facilities);

        VeterinarianFacility GetVeterinarianFacility(int veterinarianFacilityId);

        List<VeterinarianFacility> GetVeterinarianFacilitiesByVetId(int veterinarianId);

        bool DeleteVeterinarianFacility(int veterinarianFacilityId);

    }
}
