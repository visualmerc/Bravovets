using System;
using BravoVets.DomainService.Contract;
using BravoVets.DomainService.Service;
using BravoVets.DomainService.Test.Infrastructure;
using Xunit;

namespace BravoVets.DomainService.Test
{
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;

    public class VeterinianDomainServiceTest : AbstractServiceBaseTest
    {
        private IVeterinarianDomainService _vetDomainService;

        public VeterinianDomainServiceTest()
        {
            this._vetDomainService = new VeterinarianDomainService();
        }

        [Fact]
        public void VetServiceCanGetVet()
        {
            var rand = new Random();          
            var vet = this._vetDomainService.GetVeterinarian(rand.Next(1, 5));
            Assert.True(vet.VeterinarianId > 0, "Did not find a vet");
        }

        [Fact]
        public void VetServiceCanCreateVet()
        {
            var vet = CreateTestVet();
            var newVet = this._vetDomainService.AddOrUpdateVeterinarian(vet);

            Assert.True(newVet.VeterinarianId > 0);
        }

        [Fact]
        public void VetServiceCanUpdateVet()
        {
            var newVetName = string.Format("Vet {0} Practice", Guid.NewGuid().ToString("N").Substring(0, 8));
            var rand = new Random();
            var vetId = rand.Next(1, 5);
            var vet = CreateTestVet();
            vet.BusinessName = newVetName;
            vet.VeterinarianId = vetId;

            var updatedVet = this._vetDomainService.AddOrUpdateVeterinarian(vet);
            Assert.True(newVetName == updatedVet.BusinessName, "Did not update vet name");
        }

        [Fact]
        public void VetServiceCanCreateVetFacility()
        {
            var vet = CreateTestVet();
            var newVet = this._vetDomainService.AddOrUpdateVeterinarian(vet);

            Assert.True(newVet.VeterinarianId > 0, "Did not create vet");

            var facility = CreateTestFacility(vet.VeterinarianId);
            var newFacility = this._vetDomainService.AddOrUpdateFacility(facility);
            Assert.True(newFacility.VeterinarianFacilityId > 0, "did not create facility");
        }

        [Fact]
        public void VetServiceCanAddPhones()
        {
            var vet = CreateTestVet();
            var newVet = this._vetDomainService.AddOrUpdateVeterinarian(vet);

            Assert.True(newVet.VeterinarianId > 0);

            var facility = CreateTestFacility(vet.VeterinarianId);
            var newFacility = this._vetDomainService.AddOrUpdateFacility(facility);
            Assert.True(newFacility.VeterinarianFacilityId > 0, "did not create facility");

            Random rand = new Random();

            for (int i = 0; i < 3; i++)
            {
                var phone = CreatePhoneNumber(rand.Next(11111, 99999).ToString());
                newFacility.VeterinarianFacilityChannels.Add(phone);
            }

            var finalFacility = this._vetDomainService.AddOrUpdateFacility(newFacility);
            Assert.True(finalFacility.VeterinarianFacilityChannels.Count > 0, "Did not add facilities");
        }

        [Fact]
        public void VetServiceCanDeleteVet()
        {
            var rand = new Random();
            var vet = this._vetDomainService.GetVeterinarian(rand.Next(1, 5));
            Assert.True(vet.VeterinarianId > 0, "Did not find a vet");

            Assert.True(this._vetDomainService.DeleteVeterinarian(vet.VeterinarianId));
        }

        [Fact]
        public void VetServiceCanRetrieveVetFacilities()
        {
            var vet = CreateTestVet();
            var newVet = this._vetDomainService.AddOrUpdateVeterinarian(vet);

            Assert.True(newVet.VeterinarianId > 0);

            for (int i = 0; i < 3; i++)
            {
                var facility = CreateTestFacility(vet.VeterinarianId);
                var newFacility = this._vetDomainService.AddOrUpdateFacility(facility);
                Assert.True(newFacility.VeterinarianFacilityId > 0, "did not create facility");

                var rand = new Random();

                for (int j = 0; j < 3; j++)
                {
                    var phone = CreatePhoneNumber(rand.Next(11111, 99999).ToString());
                    newFacility.VeterinarianFacilityChannels.Add(phone);
                }

                var finalFacility = this._vetDomainService.AddOrUpdateFacility(newFacility);
                Assert.True(finalFacility.VeterinarianFacilityChannels.Count > 0, "Did not add facilities");
            }

            var facilityList = this._vetDomainService.GetVeterinarianFacilitiesByVetId(newVet.VeterinarianId);
            Assert.True(facilityList.Count > 0, "Did not retrieve facilities");
        }



        #region private methods

        private static Veterinarian CreateTestVet()
        {
            var rand = new Random();
            var vet = new Veterinarian
            {
                BusinessName = string.Format("Vet {0} Practice", Guid.NewGuid().ToString("N").Substring(0, 8)),
                BravoVetsCountryId = rand.Next(1, 5),
                CreateDateUtc = DateTime.Now.ToUniversalTime(),
                JoinDate = DateTime.Now,
                Deleted = false,
                ModifiedDateUtc = DateTime.Now.ToUniversalTime(),
            };

            return vet;
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

        private static VeterinarianFacilityChannel CreatePhoneNumber(string phoneNumber)
        {
            var phone = new VeterinarianFacilityChannel
            {
                ChannelTypeId = (int)ChannelTypeEnum.BusinessPhone,
                ChannelValue = phoneNumber,
                CreateDateUtc = DateTime.UtcNow,
                Deleted = false,
                ModifiedDateUtc = DateTime.UtcNow
            };

            return phone;
        }
        #endregion

    }
}