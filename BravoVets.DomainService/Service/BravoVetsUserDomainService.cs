using System;
using BravoVets.DomainObject;
using BravoVets.DomainService.Contract;
using BravoVets.DomainService.Repository;
using BravoVets.DomainService.RepositoryContract;

namespace BravoVets.DomainService.Service
{
    using System.Linq;
    using System.Runtime.InteropServices;

    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;

    public class BravoVetsUserDomainService : DomainServiceBase, IBravoVetsUserDomainService
    {
        private readonly IBravoVetsUserRepository _bravoVetsUserRepository;

        private IVeterinarianRepository _veterinarianRepository;

        private IBravoVetsCountryRepository _bravoVetsCountryRepository;

        #region ctor

        public BravoVetsUserDomainService(IBravoVetsUserRepository repository, IVeterinarianRepository vetRepository, IBravoVetsCountryRepository countryRepository)
        {
            this._bravoVetsUserRepository = repository;
            this._veterinarianRepository = vetRepository;
            this._bravoVetsCountryRepository = countryRepository;
        }

        public BravoVetsUserDomainService(IBravoVetsUserRepository repository, IVeterinarianRepository vetRepository) : this(repository, vetRepository, new BravoVetsCountryRepository())
        {
            this._bravoVetsUserRepository = repository;
            this._veterinarianRepository = vetRepository;
        }

        public BravoVetsUserDomainService(IBravoVetsUserRepository repository)
            : this(repository, new VeterinarianRepository(), new BravoVetsCountryRepository())
        {
            this._bravoVetsUserRepository = repository;
        }

        public BravoVetsUserDomainService() :
            this(new BravoVetsUserRepository(), new VeterinarianRepository(), new BravoVetsCountryRepository())
        {
        }

        #endregion

        public IVeterinarianRepository VeterinarianRepository
        {
            get { return _veterinarianRepository; }
            set { _veterinarianRepository = value; }
        }

        public BravoVetsUser CreateBravoVetsUserFromLfw(MerckUser merckUser)
        {
            try
            {
                var country = this._bravoVetsCountryRepository.GetByCountryCode(merckUser.CountryOrigin);

                if (country == null)
                {
                    throw new Exception(string.Format("Country not found by ISO code: {0}", merckUser));
                }

                var vet = CreateNewVetFromMerckUser(merckUser, country);

                var user = CreateNewUserFromMerckUser(merckUser, country);

                user.Veterinarian = vet;

                return this._bravoVetsUserRepository.Create(user);

            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.CreateBravoVetsUserFromLfw");
                throw processedError;
            }

        }

        public BravoVetsUser UpdateBravoVetsUser(BravoVetsUser bravoVetsUser)
        {
            try
            {
                return this._bravoVetsUserRepository.Update(bravoVetsUser);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.UpdateBravoVetsUser");
                throw processedError;
            }
        }

        public BravoVetsUser UpdateUserFromProfile(BravoVetsUser bravoVetsUserDto)
        {
            try
            {
                var oldUser = this._bravoVetsUserRepository.Get(bravoVetsUserDto.BravoVetsUserId);

                oldUser.Veterinarian.BusinessName = bravoVetsUserDto.Veterinarian.BusinessName;

                if (bravoVetsUserDto.Veterinarian.EditableFacility != null && bravoVetsUserDto.Veterinarian.EditableFacility.IsEditable)
                {
                    var facilityDto = bravoVetsUserDto.Veterinarian.EditableFacility;
                    if (facilityDto.VeterinarianFacilityId > 0)
                    {
                        foreach (var oldFacility in oldUser.Veterinarian.VeterinarianFacilities.Where(oldFacility => oldFacility.VeterinarianFacilityId == facilityDto.VeterinarianFacilityId))
                        {
                            PopulateFacility(oldFacility, facilityDto);
                        }                        
                    }
                    else
                    {
                        VeterinarianFacility addFacility = new VeterinarianFacility(); // in case a facility needs to be added
                        PopulateFacility(addFacility, facilityDto);
                        addFacility.CreateDateUtc = DateTime.UtcNow;
                        addFacility.Deleted = false;
                        addFacility.IsEditable = false;
                        //addFacility.VeterinarianId = facility.VeterinarianId;
                        oldUser.Veterinarian.VeterinarianFacilities.Add(addFacility);
                    }
                }

                return this._bravoVetsUserRepository.Update(oldUser);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.UpdateUserFromProfile");
                throw processedError;
            }
        }

        public BravoVetsUser GetBravoVetsUser(int bravoVetsUserId)
        {
            try
            {
                // TODO: check to see if this is deleted
                var user = this._bravoVetsUserRepository.Get(bravoVetsUserId);
                SetSocialHelperValues(user);

                return user;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "BravoVetsUserDomainService.GetBravoVetsUser");
                throw processedError;
            }
        }

        public BravoVetsUser GetBravoVetsUserFromLfwId(int loginFrameworkId)
        {
            try
            {
                return this._bravoVetsUserRepository.GetByMerckId(loginFrameworkId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.GetBravoVetsUserFromLfwId");
                throw processedError;
            }
        }

        public BravoVetsUser GetBravoVetsUserFullGraph(int bravoVetsUserId)
        {
            try
            {
                // TODO: check to see if this is deleted
                var user = this._bravoVetsUserRepository.Get(bravoVetsUserId, true); 
                SetSocialHelperValues(user);

                return user;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.GetBravoVetsUserFullGraph");
                throw processedError;
            }
        }


        public BravoVetsUser GetBravoVetsUserForProfileEdit(int bravoVetsUserId)
        {
            try
            {
                var user = this._bravoVetsUserRepository.Get(bravoVetsUserId, true);
                SetSocialHelperValues(user);

                // Put a placeholder facilityDto for user profile editing
                var facility = new VeterinarianFacility { VeterinarianId = user.VeterinarianId, IsEditable = true };
                user.Veterinarian.EditableFacility = facility;
                return user;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "BravoVetsUserDomainService.GetBravoVetsUserForProfileEdit");
                throw processedError;
            }
        }

        public BravoVetsUser GetBravoVetsUserForProfileEdit(int bravoVetsUserId, int veterinarianFacilityId)
        {
            try
            {
                var user = this._bravoVetsUserRepository.Get(bravoVetsUserId, true);
                SetSocialHelperValues(user);
                var facilityList = user.Veterinarian.VeterinarianFacilities.ToList();

                for (int i = facilityList.Count - 1; i >= 0; i--)
                {
                    var checkFacility = facilityList[i];
                    if (checkFacility.VeterinarianFacilityId == veterinarianFacilityId)
                    {
                        checkFacility.IsEditable = true;
                        user.Veterinarian.EditableFacility = checkFacility;
                        facilityList.RemoveAt(i);
                        break;
                    }
                }

                user.Veterinarian.VeterinarianFacilities = facilityList;

                return user;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(
                    ex,
                    "BravoVetsUserDomainService.GetBravoVetsUserForProfileEdit");
                throw processedError;
            }
        }


        public bool DeleteBravoVetsUser(int bravoVetsUserId)
        {
            try
            {
                return this._bravoVetsUserRepository.Delete(bravoVetsUserId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.DeleteBravoVetsUser");
                throw processedError;
            }

        }

        public bool AcceptTermsAndConditions(int bravoVetsUserId)
        {
            try
            {
                return this._bravoVetsUserRepository.AcceptTermsAndConditions(bravoVetsUserId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.AcceptTermsAndConditions");
                throw processedError;
            }
        }

        public VeterinarianSocialIntegration GetTwitterSocialIntegration(BravoVetsUser bravoVetsUser)
        {
            try
            {
                var fullUser = this.GetBravoVetsUserFullGraph(bravoVetsUser.BravoVetsUserId);
                return this._veterinarianRepository.RetrieveTwitterInfo(fullUser.Veterinarian.VeterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.GetTwitterTokens");
                throw processedError;
            }
        }

        public VeterinarianSocialIntegration GetFacebookSocialIntegration(BravoVetsUser bravoVetsUser)
        {
            try
            {
                var fullUser = this.GetBravoVetsUserFullGraph(bravoVetsUser.BravoVetsUserId);
                return this._veterinarianRepository.RetrieveFacebookInfo(fullUser.Veterinarian.VeterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.GetFacebookTokens");
                throw processedError;
            }
        }

        public VeterinarianSocialIntegration SaveFacebookTokens(BravoVetsUser bravoVetsUser, string code, string token,
            string userId, string pageId, string pageToken,
            int numberOfFollowers)
        {
            try
            {
                var fullUser = this.GetBravoVetsUserFullGraph(bravoVetsUser.BravoVetsUserId);
                return this._veterinarianRepository.SaveFacebookTokens(fullUser.Veterinarian.VeterinarianId, code, token,
                    userId, pageId, pageToken, numberOfFollowers);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex,
                    "BravoVetsUserDomainService.SaveFacebookTokens");
                throw processedError;
            }

        }

        public VeterinarianSocialIntegration SaveTwitterTokens(BravoVetsUser bravoVetsUser, string accessToken,
            string tokenSecret, string userId, int numberOfFollowers)
        {
            try
            {
                var fullUser = this.GetBravoVetsUserFullGraph(bravoVetsUser.BravoVetsUserId);
                return this._veterinarianRepository.SaveTwitterTokens(fullUser.Veterinarian.VeterinarianId, accessToken,
                    tokenSecret, userId, numberOfFollowers);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex,
                    "BravoVetsUserDomainService.SaveTwitterTokens");
                throw processedError;
            }
        }

        public bool DeleteFacebookIntegration(int bravoVetsUserId)
        {
            try
            {
                var fullUser = this.GetBravoVetsUser(bravoVetsUserId);
                return this._veterinarianRepository.DeleteFacebookIntegration(fullUser.VeterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.DeleteFacebookIntegration");
                throw processedError;
            }
        }

        public bool DeleteTwitterIntegration(int bravoVetsUserId)
        {
            try
            {
                var fullUser = this.GetBravoVetsUser(bravoVetsUserId);
                return this._veterinarianRepository.DeleteTwitterIntegration(fullUser.VeterinarianId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.DeleteTwitterIntegration");
                throw processedError;
            }
        }

        public bool RejectTermsAndConditions(int bravoVetsUserId)
        {
            try
            {
                return this._bravoVetsUserRepository.ReverseTermsAndConditions(bravoVetsUserId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.AcceptTermsAndConditions");
                throw processedError;
            }
        }

        public VeterinarianSocialIntegration UpdateVeterinarianSocialIntegration(VeterinarianSocialIntegration veterinarianSocial)
        {
            try
            {
                return this._veterinarianRepository.UpdateVeterinarianSocialIntegration(veterinarianSocial);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsServiceException(ex, "BravoVetsUserDomainService.UpdateVeterinarianSocialIntegration");
                throw processedError;
            }
        }
 
        #region private methods

        private static void SetSocialHelperValues(BravoVetsUser user)
        {
            foreach (var vsi in user.Veterinarian.VeterinarianSocialIntegrations)
            {
                switch (vsi.SocialPlatformId)
                {
                    case (int)SocialPlatformEnum.Facebook:
                        if (!string.IsNullOrEmpty(vsi.AccessToken))
                        {
                            user.Veterinarian.IsFacebookLinked = true;
                            user.Veterinarian.AccessToken = vsi.AccessToken;
                            user.Veterinarian.AccessTokenSecret = vsi.AccessCode;
                        }
                        break;
                    case (int)SocialPlatformEnum.Twitter:
                        if (!string.IsNullOrEmpty(vsi.AccessToken))
                        {
                            user.Veterinarian.IsTwitterLinked = true;
                            user.Veterinarian.AccessToken = vsi.AccessToken;
                            user.Veterinarian.AccessTokenSecret = vsi.AccessCode;
                        }
                        break;
                }
            }
        }

        private static void PopulateFacility(VeterinarianFacility facility, VeterinarianFacility facilityDto)
        {
            facility.City = facilityDto.City;
            facility.Country = facilityDto.Country;
            facility.EmailAddress = facilityDto.EmailAddress;
            facility.FacilityName = facilityDto.FacilityName;
            facility.FaxNumber = facilityDto.FaxNumber;
            facility.PostalCode = facilityDto.PostalCode;
            facility.PrimaryPhoneNumber = facilityDto.PrimaryPhoneNumber;
            facility.SecondaryPhoneNumber = facilityDto.SecondaryPhoneNumber;
            facility.StateProvince = facilityDto.StateProvince;
            facility.StreetAddress1 = facilityDto.StreetAddress1;
            facility.StreetAddress2 = facilityDto.StreetAddress2;
            facility.ModifiedDateUtc = DateTime.UtcNow;
        }

        private static BravoVetsUser CreateNewUserFromMerckUser(MerckUser merckUser, BravoVetsCountry country)
        {
            var user = new BravoVetsUser
            {
                AcceptedTandC = false,
                CreateDateUtc = DateTime.Now.ToUniversalTime(),
                Deleted = false,
                Email = merckUser.EmailAddress,
                EmailOptIn = false,
                FirstName = merckUser.FirstName,
                BravoVetsCountryId = country.BravoVetsCountryId,
                CultureName = country.CultureName,
                Lastname = merckUser.LastName,
                MerckId = merckUser.MerckUserId,
                ModifiedDateUtc = DateTime.Now.ToUniversalTime(),
                BravoVetsStatusId = 1
            };
            return user;
        }


        private static Veterinarian CreateNewVetFromMerckUser(MerckUser merckUser, BravoVetsCountry country)
        {
            var vet = new Veterinarian
            {
                BusinessName = merckUser.FirstName + " " + merckUser.LastName,
                BravoVetsCountryId = country.BravoVetsCountryId,
                CreateDateUtc = DateTime.Now.ToUniversalTime(),
                JoinDate = DateTime.Now,
                Deleted = false,
                ModifiedDateUtc = DateTime.Now.ToUniversalTime()
            };
            return vet;
        }
        
        #endregion

    }
}
