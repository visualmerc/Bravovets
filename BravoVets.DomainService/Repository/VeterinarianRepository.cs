using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.Dal;
using BravoVets.DomainService.RepositoryContract;
using BravoVets.DomainObject;

namespace BravoVets.DomainService.Repository
{
    using BravoVets.DomainObject.Enum;

    public class VeterinarianRepository : RepositoryBase, IVeterinarianRepository
    {
        private BravoVetsDbEntities _db;

        public VeterinarianRepository()
        {
            this._db = new BravoVetsDbEntities();
        }

        public Veterinarian Get(int veterinarianId)
        {
            return this._db.Veterinarians.Find(veterinarianId);
        }

        public Veterinarian Create(Veterinarian vet)
        {
            try
            {
                var newVet = this._db.Veterinarians.Add(vet);
                this._db.SaveChanges();
                return newVet;
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianRepository.Create");
                throw processedError;
            }

        }

        public Veterinarian Update(Veterinarian vet)
        {
            var oldVet =
                this._db.Veterinarians.Find(vet.VeterinarianId);

            if (oldVet != null)
            {
                try
                {
                    this._db.Entry(oldVet).CurrentValues.SetValues(vet);
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianRepository.Update");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to update");
            }

            return vet;
        }

        public bool Delete(int veterinarianId)
        {
            var oldVet =
                this._db.Veterinarians.Find(veterinarianId);
            bool didAccept = false;

            if (oldVet != null)
            {
                try
                {
                    oldVet.Deleted = true;
                    this._db.SaveChanges();
                    didAccept = true;
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianRepository.Update");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find user to update");
            }

            return didAccept;
        }

        public VeterinarianSocialIntegration SaveFacebookTokens(int veterinarianId, string code, string token,
            string userId, string pageId,
            string pageToken, int numberOfFollowers)
        {
            return SaveSocialIntegrationToken(veterinarianId, code, token, userId, pageId, pageToken, "Facebook", numberOfFollowers);
        }


        public VeterinarianSocialIntegration SaveTwitterTokens(int veterinarianId, string accessToken,
            string tokenSecret,
            string userId, int numberOfFollowers)
        {
            return SaveSocialIntegrationToken(veterinarianId, accessToken, tokenSecret, userId, string.Empty,
                string.Empty, "Twitter", numberOfFollowers);
        }

        private VeterinarianSocialIntegration SaveSocialIntegrationToken(int veterinarianId, string code, string token,
            string userId, string pageId, string pageToken, string socialPlatformName
            , int numberOfFollowers)
        {
            var platform =
                this._db.SocialPlatforms.FirstOrDefault(s => s.SocialPlatformName == socialPlatformName);

            if (platform == null)
            {
                throw new ObjectNotFoundException(string.Format("Did not find {0} social platform to update",
                    socialPlatformName));
            }

            int socialPlatformId = platform.SocialPlatformId;

            var socialIntegration = this._db.VeterinarianSocialIntegrations.FirstOrDefault(
                v => v.SocialPlatformId == socialPlatformId
                     && v.VeterinarianId == veterinarianId);

            if (socialIntegration == null)
            {
                var fbInt = new VeterinarianSocialIntegration
                {
                    AccessCode = code,
                    AccessToken = token,
                    AccountName = userId,
                    AccountPassword = string.Empty,
                    CreateDateUtc = DateTime.Now.ToUniversalTime(),
                    Deleted = false,
                    ModifiedDateUtc = DateTime.Now.ToUniversalTime(),
                    SocialPlatformId = platform.SocialPlatformId,
                    VeterinarianId = veterinarianId,
                    PageAccessToken = pageToken,
                    PageId = pageId, 
                    NumberOfFollowers = numberOfFollowers
                };

                this._db.VeterinarianSocialIntegrations.Add(fbInt);
                this._db.SaveChanges();
                return fbInt;
            }

            socialIntegration.AccessCode = code;
            socialIntegration.AccessToken = token;
            socialIntegration.PageAccessToken = pageToken;
            socialIntegration.PageId = pageId;
            socialIntegration.NumberOfFollowers = numberOfFollowers;
            this._db.SaveChanges();

            return socialIntegration;
        }


        public VeterinarianSocialIntegration RetrieveFacebookInfo(int veterinarianId)
        {
            return RetrieveSocialIntegrationInfo(veterinarianId, "Facebook");
        }

        public VeterinarianSocialIntegration RetrieveTwitterInfo(int veterinarianId)
        {
            return RetrieveSocialIntegrationInfo(veterinarianId, "Twitter");
        }

        public bool DeleteFacebookIntegration(int veterinarianId)
        {
            const int socialPlatformId = (int)SocialPlatformEnum.Facebook;
            try
            {
                return this.DeleteSocialIntegration(veterinarianId, socialPlatformId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianRepository.DeleteFacebookIntegration");
                throw processedError;
            }
        }

        private bool DeleteSocialIntegration(int veterinarianId, int socialPlatformId)
        {
            var vet = this._db.Veterinarians.Find(veterinarianId);

            if (vet == null)
            {
                return false;
            }

            List<VeterinarianSocialIntegration> integrations = vet.VeterinarianSocialIntegrations.ToList();

            for (int i = integrations.Count - 1; i >= 0; i--)
            {
                if (integrations[i].SocialPlatformId == socialPlatformId)
                {
                    var integration =
                        this._db.VeterinarianSocialIntegrations.Find(integrations[i].VeterinarianSocialIntegrationId);
                    this._db.VeterinarianSocialIntegrations.Remove(integration);
                    this._db.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public bool DeleteTwitterIntegration(int veterinarianId)
        {
            const int socialPlatformId = (int)SocialPlatformEnum.Twitter;
            try
            {
                return this.DeleteSocialIntegration(veterinarianId, socialPlatformId);
            }
            catch (Exception ex)
            {
                var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianRepository.DeleteFacebookIntegration");
                throw processedError;
            }
        }
        
        private VeterinarianSocialIntegration RetrieveSocialIntegrationInfo(int veterinarianId, string socialPlatformName)
        {
            var platform =
                this._db.SocialPlatforms.FirstOrDefault(s => s.SocialPlatformName == socialPlatformName);

            if (platform == null)
            {
                throw new ObjectNotFoundException(string.Format("Did not find {0} social platform to update",socialPlatformName));
            }

            var socialPlatformId = platform.SocialPlatformId;

            var socialIntegration = this._db.VeterinarianSocialIntegrations.FirstOrDefault(
                v => v.SocialPlatformId == socialPlatformId
                     && v.VeterinarianId == veterinarianId);

            return socialIntegration;
        }

        public VeterinarianSocialIntegration UpdateVeterinarianSocialIntegration(VeterinarianSocialIntegration veterinarianSocialIntegration)
        {
            var oldSocialIntegration =
                this._db.VeterinarianSocialIntegrations.Find(veterinarianSocialIntegration.VeterinarianSocialIntegrationId);

            if (oldSocialIntegration != null)
            {
                try
                {
                    this._db.Entry(oldSocialIntegration).CurrentValues.SetValues(veterinarianSocialIntegration);
                    this._db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var processedError = GenerateBravoVetsRepositoryException(ex, "VeterinarianRepository.UpdateVeterinarianSocialIntegration");
                    throw processedError;
                }
            }
            else
            {
                throw new ObjectNotFoundException("Did not find socialIntegration to update");
            }

            return veterinarianSocialIntegration;
        }
    }
}
