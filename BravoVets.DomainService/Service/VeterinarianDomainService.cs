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

    public class VeterinarianDomainService : DomainServiceBase, IVeterinarianDomainService
    {
        private IVeterinarianRepository _vetRepository;

        private IVeterinarianFacilityRepository _facilityRepository;

        public VeterinarianDomainService() : this(new VeterinarianRepository(), new VeterinarianFacilityRepository())
        {            
        }

        public VeterinarianDomainService(IVeterinarianRepository veterinarianRepository) : 
            this(veterinarianRepository, new VeterinarianFacilityRepository())
        {            
        }

        public VeterinarianDomainService(IVeterinarianRepository veterinarianRepository, IVeterinarianFacilityRepository facilityRepository)
        {
            this._vetRepository = veterinarianRepository;
            this._facilityRepository = facilityRepository;
        }

        public Veterinarian GetVeterinarian(int veterinarianId)
        {
            try
            {
                return this._vetRepository.Get(veterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.GetVeterinarian");
                throw processedError;
            }        
        }

        public Veterinarian AddOrUpdateVeterinarian(Veterinarian veterinarian)
        {
            try
            {
                return veterinarian.VeterinarianId > 0 ? 
                    this._vetRepository.Update(veterinarian) : 
                    this._vetRepository.Create(veterinarian);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.AddOrUpdateVeterinarian");
                throw processedError;
            }
        }

        public bool DeleteVeterinarian(int veterinarianId)
        {
            try
            {
                return this._vetRepository.Delete(veterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.DeleteVeterinarian");
                throw processedError;
            }
        }
        

        public bool DeleteVeterinarianFacility(int veterinarianFacilityId)
        {
            try
            {
                return this._facilityRepository.Delete(veterinarianFacilityId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.DeleteVeterinarianFacility");
                throw processedError;
            }
        }
        
        public VeterinarianFacility AddOrUpdateFacility(VeterinarianFacility veterinarianFacility)
        {
            try
            {
                return veterinarianFacility.VeterinarianFacilityId > 0 ?
                    this._facilityRepository.Update(veterinarianFacility) :
                    this._facilityRepository.Create(veterinarianFacility);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.AddOrUpdateFacility");
                throw processedError;
            }
        }

        public List<VeterinarianFacility> AddOrUpdateFacilities(List<VeterinarianFacility> facilities)
        {
            return facilities.Select(this.AddOrUpdateFacility).ToList();
        }

        public VeterinarianFacility GetVeterinarianFacility(int veterinarianFacilityId)
        {
            try
            {
                return this._facilityRepository.Get(veterinarianFacilityId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.GetVeterinarianFacility");
                throw processedError;
            }
        }

        public List<VeterinarianFacility> GetVeterinarianFacilitiesByVetId(int veterinarianId)
        {
            try
            {
                return this._facilityRepository.GetVeterinarianFacilitiesByVetId(veterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "VeterinarianDomainService.GetVeterinarianFacilitiesByVetId");
                throw processedError;
            }
        }



    }
}
