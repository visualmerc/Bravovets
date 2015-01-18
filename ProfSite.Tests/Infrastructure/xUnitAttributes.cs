using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace ProfSite.Tests.Infrastructure
{
    /// <summary>
    ///     Facts that invoke a controller can be decorated with this attribute in order to invoke action autmatically
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ControllerInvocationAttribute : Attribute
    {
        /// <summary>
        ///     Initializes ControllerInvocationAttribute attribute
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="actionToInvoke"></param>
        /// <param name="parameters"></param>
        public ControllerInvocationAttribute(Type controllerType, string actionToInvoke, string language = "en-US", params object[] parameters)
        {
            ControllerType = controllerType;
            ActionToInvoke = actionToInvoke;
            Parameters = parameters;
            Language = language;
        }

        public ControllerInvocationAttribute(Type controllerType, string actionToInvoke, params object[] parameters)
        {
            ControllerType = controllerType;
            ActionToInvoke = actionToInvoke;
            Parameters = parameters;            
        }

        public string Language { get; set; }

        /// <summary>
        ///     What action to invoke on the controller
        /// </summary>
        public string ActionToInvoke { get; private set; }

        /// <summary>
        ///     Parameters to the action
        /// </summary>
        public object[] Parameters { get; private set; }

        /// <summary>
        ///     Controller to invoke
        /// </summary>
        public Type ControllerType { get; private set; }
    }

    public class ControllerFactAttribute : DataScenarioFactAttribute
    {
        public ControllerFactAttribute(params Type[] dataScenarios) : base(dataScenarios)
        {
        }

        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            yield return new ControllerCommand(method, this);
        }
    }

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
            if (dataScenarios.Any(e => !e.IsSubclassOf(typeof (AbstractDataScenario))))
            {
                throw new InvalidOperationException(
                    "All types passed in DataScenarios must derive from AbstractDataScenario");
            }
        }
    }
}