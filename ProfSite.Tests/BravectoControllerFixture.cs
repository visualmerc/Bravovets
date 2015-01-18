using System;
using System.Linq;
using System.Web.Mvc;
using BravoVets.DomainObject;
using BravoVets.DomainService.Contract;
using BravoVets.DomainService.Repository;
using BravoVets.DomainService.Service;
using ProfSite.Controllers;
using ProfSite.Models;
using ProfSite.Tests.Infrastructure;
using Xunit;

namespace ProfSite.Tests
{


    public class DataSourceTest:AbstractDataScenario
    {

        private IBravoVetsUserDomainService _bravoVetsUserService;

        public override void CreateData()
        {
            this._bravoVetsUserService = new BravoVetsUserDomainService(new BravoVetsUserRepository(), new VeterinarianRepository());

            var merckUser = CreateMerckUser();

            var newUser = this._bravoVetsUserService.CreateBravoVetsUserFromLfw(merckUser);
        }


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
                Occupation = "Vet",           
                
            };
            return merckUser;
        }
    }

    public class BravectoControllerFixture : AbstractMvcBaseTest
    {



        [ControllerFact()]
        [ControllerInvocation(typeof(BravectoController), "Home")]
        public void HomeTest(ViewResult viewResult)
        {
            var menu = viewResult.ViewBag.Menu as BravectoMenu;

            Assert.NotNull(menu);
            Assert.Equal("home", menu.ActiveItem);

        }

            
        [ControllerFact]
        [ControllerInvocation(typeof(BravectoController), "Innovation")]
        public void InnovationTest(ViewResult viewResult)
        {
            var menu = viewResult.ViewBag.Menu as BravectoMenu;

            Assert.NotNull(menu);
            Assert.Equal("innovation", menu.ActiveItem);

        }

        [ControllerFact]
        [ControllerInvocation(typeof(BravectoController), "Compliance")]
        public void ComplianceTest(ViewResult viewResult)
        {
            var menu = viewResult.ViewBag.Menu as BravectoMenu;

            Assert.NotNull(menu);
            Assert.Equal("compliance", menu.ActiveItem);

        }

        [ControllerFact]
        [ControllerInvocation(typeof(BravectoController), "NewBusiness")]
        public void NewBusinessTest(ViewResult viewResult)
        {
            var menu = viewResult.ViewBag.Menu as BravectoMenu;

            Assert.NotNull(menu);
            Assert.Equal("newbusiness", menu.ActiveItem);

        }

    }
}