using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelper;

namespace BravoVets.DomainService.Test.Infrastructure
{
    using log4net.Config;

    public abstract class AbstractServiceBaseTest : AbstractBaseTest
    {
        protected AbstractServiceBaseTest()
        {
            var ceManager = new SqlCeManager();
            ceManager.CreateBravoVetsCeDatabase();
            XmlConfigurator.Configure();

        }
    }
}
