using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Test.Infrastructure
{
    using Xunit;

    public abstract class DataScenarioFactAttribute : FactAttribute
    {
        protected DataScenarioFactAttribute(params Type[] dataScenarios)
        {
            DataScenarioAttribute.Validate(dataScenarios);
            DataScenarios = dataScenarios;
        }

        /// <summary>
        ///     List of DataScenarios to load
        /// </summary>
        public Type[] DataScenarios { get; private set; }
    }


    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DataScenarioAttribute : Attribute
    {
        public DataScenarioAttribute(params Type[] dataScenarios)
        {
            Validate(dataScenarios);
            DataScenarios = dataScenarios;
        }

        public Type[] DataScenarios { get; private set; }

        public static void Validate(Type[] dataScenarios)
        {
            if (dataScenarios.Any(e => !e.IsSubclassOf(typeof(AbstractDataScenario))))
            {
                throw new InvalidOperationException(
                    "All types passed in DataScenarios must derive from AbstractDataScenario");
            }
        }
    }
}
