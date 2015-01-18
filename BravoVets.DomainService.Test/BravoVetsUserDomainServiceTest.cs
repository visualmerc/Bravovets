using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Test
{
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Infrastructure;
    using BravoVets.DomainService.Contract;
    using BravoVets.DomainService.Service;
    using BravoVets.DomainService.Test.Infrastructure;

    using Xunit;

    public class BravoVetsUserDomainServiceTest : AbstractServiceBaseTest
    {
        private IBravoVetsUserDomainService _bravoVetsUserService;

        private int _testUserId = 1;

        public BravoVetsUserDomainServiceTest()
        {
            this._bravoVetsUserService = new BravoVetsUserDomainService();
        }

        [Fact]
        public void UserServiceCanCreateUserFromLfw()
        {
            var merckUser = CreateMerckUser();

            var newUser = this._bravoVetsUserService.CreateBravoVetsUserFromLfw(merckUser);

            Assert.True(newUser.BravoVetsUserId > 0, "Did not create a new user");
        }

        [Fact]
        public void UserServiceCanThrowCustomError()
        {
            var badUser = CreateMerckUser();

            badUser.CountryOrigin = "xx";

            Exception ex = Assert.Throws<BravoVetsException>(() => this._bravoVetsUserService.CreateBravoVetsUserFromLfw(badUser));

        }

        [Fact]
        public void UserServiceCanRetrieveUser()
        {
            var merckUser = this._bravoVetsUserService.GetBravoVetsUser(this._testUserId);
            Assert.True(merckUser.BravoVetsUserId > 0, "Did not retrieve user");
        }

        [Fact]
        public void UserServiceCanRetrieveUserAndVet()
        {
            var merckUser = this._bravoVetsUserService.GetBravoVetsUserFullGraph(this._testUserId);
            Assert.True(merckUser.Veterinarian.VeterinarianId > 0, "Did not retrieve vet");
        }

        [Fact]
        public void UserServiceCanUpdateUser()
        {
            var merckUser = this._bravoVetsUserService.GetBravoVetsUser(this._testUserId);
            var updatedName = string.Format("Updated{0}", Guid.NewGuid().ToString().Substring(0, 3));
            merckUser.FirstName = updatedName;
            var updateUser = this._bravoVetsUserService.UpdateBravoVetsUser(merckUser);
            Assert.True(updateUser.FirstName == updatedName, "Did not update user name");
        }

        [Fact]
        public void UserServiceCanUpdateFromUserProfile()
        {
            var bizName = string.Format("Biz Name {0}", Guid.NewGuid().ToString("N").Substring(0, 8));

            var randUserId = new Random().Next(1, 5);
            var merckUser = this._bravoVetsUserService.GetBravoVetsUserFullGraph(randUserId);
            var facility = CreateTestFacility(merckUser.VeterinarianId);
            facility.IsEditable = true;
            merckUser.Veterinarian.VeterinarianFacilities.Add(facility);
            merckUser.Veterinarian.BusinessName = bizName;

            var updateUser = this._bravoVetsUserService.UpdateUserFromProfile(merckUser);

            Assert.True(updateUser.Veterinarian.BusinessName == bizName, "Did not update vet business name.");
        }

        [Fact]
        public void UserServiceCanDeleteUser()
        {
            var didDelete = this._bravoVetsUserService.DeleteBravoVetsUser(2);
            Assert.True(didDelete, "Did not delete the user");
        }

        [Fact]
        public void UserServiceCanAcceptTerms()
        {
            var didAccept = this._bravoVetsUserService.AcceptTermsAndConditions(this._testUserId);
            var merckUser = this._bravoVetsUserService.GetBravoVetsUser(this._testUserId);
            Assert.True(didAccept, "Did accept terms");
            Assert.True(merckUser.AcceptedTandC, "Did accept terms");
        }

        [Fact]
        public void UserServiceCanSaveFacebookTokens()
        {
            var merckUser = this._bravoVetsUserService.GetBravoVetsUser(this._testUserId);
            var token = Guid.NewGuid().ToString("N");
            var axCode = Guid.NewGuid().ToString("N");
            var userId = Guid.NewGuid().ToString("N");
            var pageId = Guid.NewGuid().ToString("N");
            var pageToken = Guid.NewGuid().ToString("N");

            var socialIntegration = this._bravoVetsUserService.SaveFacebookTokens(merckUser,
                axCode, token,userId,pageId,pageToken, 50);

            Assert.True(socialIntegration.VeterinarianSocialIntegrationId > 0, "Failed on saving the integration");
            Assert.True(socialIntegration.AccessToken == token, "Failed on saving the integration");

        }

        [Fact]
        public void UserServiceCanDeleteFacebookTokens()
        {
            Random rand = new Random();
            var merckUser = this._bravoVetsUserService.GetBravoVetsUser(rand.Next(1,4));
            var token = Guid.NewGuid().ToString("N");
            var axCode = Guid.NewGuid().ToString("N");
            var fbUserId = Guid.NewGuid().ToString("N");
            var pageId = Guid.NewGuid().ToString("N");
            var pageToken = Guid.NewGuid().ToString("N");

            var socialIntegration = this._bravoVetsUserService.SaveFacebookTokens(merckUser, axCode, token,fbUserId,pageId,pageToken, 50);
            Assert.True(socialIntegration.VeterinarianSocialIntegrationId > 0, "Failed on saving the integration");
            Assert.True(socialIntegration.AccessToken == token, "Failed on saving the integration");

            var didDelete = this._bravoVetsUserService.DeleteFacebookIntegration(merckUser.BravoVetsUserId);
            Assert.True(didDelete, "Did not delete the facebook integration");
        }

 

        #region private methods

        private static MerckUser CreateMerckUser()
        {
            var rand = new Random();
            var merckUser = new MerckUser
            {
                CountryOrigin = "GB",
                EmailAddress = string.Format("rob{0}@test.com", Guid.NewGuid().ToString().Substring(0, 6)),
                FirstName = string.Format("Rock{0}", Guid.NewGuid().ToString().Substring(0, 3)),
                LastName = string.Format("Columns{0}", Guid.NewGuid().ToString().Substring(0, 3)),
                MerckUserId = rand.Next(25, 5000),
                Occupation = "Vet"
            };
            return merckUser;
        }

        private static BravoVetsUser CreateBravoVetsUser()
        {
            var user = new BravoVetsUser
            {
                AcceptedTandC = false,
                CreateDateUtc = DateTime.Now.ToUniversalTime(),
                Deleted = false,
                Email = "Test@test.com",
                EmailOptIn = false,
                FirstName = "Rock",
                BravoVetsCountryId = 3,
                Lastname = "Columns",
                MerckId = 1, 
                ModifiedDateUtc = DateTime.Now.ToUniversalTime(),
                BravoVetsStatusId = 1 
            };

            return user;
            // user.VeterinarianId = 1;
        }

        private static VeterinarianFacility CreateTestFacility(int vetId)
        {
            var vetFacility =
                new VeterinarianFacility
                {
                    City = "Paris",
                    Country = "France",
                    CreateDateUtc = DateTime.UtcNow,
                    Deleted = false,
                    EmailAddress = "chien@paris.com",
                    FacilityName =
                        string.Format(
                            "Le Vet {0}",
                            Guid.NewGuid().ToString("N").Substring(0, 8)),
                    ModifiedDateUtc = DateTime.UtcNow,
                    PostalCode = "+ 15 89999",
                    StreetAddress1 = "Rue De mar",
                    StreetAddress2 = "Pierre Rue",
                    VeterinarianId = vetId
                };

            return vetFacility;
        }
        #endregion
    }
}
