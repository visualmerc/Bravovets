using System.Linq;
using ProfSite.Controllers;
using ProfSite.Models;
using ProfSite.Tests.Infrastructure;
using Xunit;

namespace ProfSite.Tests
{
    public class BravovetsControllerFixture : AbstractMvcBaseTest
    {
        [ControllerFact]
        [ModelSpecification(typeof (SelectCountry))]
        [ControllerInvocation(typeof (BravovetsController), "SelectCountry", "es-MX", "test")]
        public void LoadCorrectCountries(SelectCountry model)
        {
            Assert.Equal(6, model.SupportedCountries.Count());
        }
    }
}