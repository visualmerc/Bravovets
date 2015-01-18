using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using Xunit.Sdk;

namespace ProfSite.Tests.Infrastructure
{
    /// <summary>
    ///     Contains specifications of the action result
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionResultSpecificationAttribute : Attribute
    {
        public ActionResultSpecificationAttribute(Type resultType, bool allowNull = false)
        {
            if (!typeof(ActionResult).IsAssignableFrom(resultType))
            {
                throw new InvalidOperationException("resultType must be an ActionResult");
            }

            ResultType = resultType;
            AllowNull = allowNull;
        }

        internal Type ResultType { get; set; }
        internal bool AllowNull { get; set; }
    }



    /// <summary>
    ///     Contains specifications for the model attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ModelSpecificationAttribute : Attribute
    {
        public ModelSpecificationAttribute(Type modelType, bool allowNull = false, string modelPropertyName = "Model")
        {
            ModelType = modelType;
            AllowNull = allowNull;
            ModelPropertyName = modelPropertyName;
        }

        public Type ModelType { get; set; }
        public bool AllowNull { get; set; }
        public string ModelPropertyName { get; set; }
    }

    public class ControllerCommand : FactCommand
    {
        private readonly ActionResultSpecificationAttribute _actionResultSpecs;
        private readonly ControllerFactAttribute _controllerFact;
        private readonly ControllerInvocationAttribute _controllerInvocation;
        private readonly IMethodInfo _methodInfo;
        private readonly ModelSpecificationAttribute _modelSpec;

        public ControllerCommand(IMethodInfo methodInfo, ControllerFactAttribute controllerFact)
            : base(methodInfo)
        {
            _methodInfo = methodInfo;
            _controllerFact = controllerFact;
            _controllerInvocation = ReflectionUtils.GetAttribute<ControllerInvocationAttribute>(methodInfo.MethodInfo);
            _actionResultSpecs = ReflectionUtils.GetAttribute<ActionResultSpecificationAttribute>(methodInfo.MethodInfo);
            _modelSpec = ReflectionUtils.GetAttribute<ModelSpecificationAttribute>(methodInfo.MethodInfo);
        }

        public override MethodResult Execute(object testClass)
        {
            object testerObject = SetupTester(testClass);

            if (_controllerInvocation != null)
            {
                return ProceedExecutionWithResult(testClass, testerObject);
            }
            return WrapWithResultAndExceptionHandling(() => _methodInfo.Invoke(testClass, testerObject));
        }

        private MethodResult ProceedExecutionWithResult(object testClass, dynamic testerObject)
        {
            return WrapWithResultAndExceptionHandling(() =>
                                                      {
                                                          if (_methodInfo.MethodInfo.GetParameters().Length > 1)
                                                          {
                                                              throw new ApplicationException(
                                                                  "Test methods must accept only one result");
                                                          }

                                                          if (!string.IsNullOrEmpty(_controllerInvocation.Language))
                                                          {
                                                              Thread.CurrentThread.CurrentUICulture = new CultureInfo(_controllerInvocation.Language);
                                                              Thread.CurrentThread.CurrentCulture = new CultureInfo(_controllerInvocation.Language);
                                                          }

                                                          object resultValue =
                                                              testerObject.Execute(
                                                                  _controllerInvocation.ActionToInvoke,
                                                                  _controllerInvocation.Parameters);

                                                          if (_actionResultSpecs != null && resultValue is ActionResult)
                                                          {
                                                              ActionResultTestHelper.ConformsToSpecifications(
                                                                  (ActionResult)resultValue, _actionResultSpecs);
                                                          }

                                                          if (_modelSpec != null && resultValue is ActionResult)
                                                          {
                                                              var model = ActionResultTestHelper
                                                                  .ConformsToModelSpecifications(
                                                                      (ActionResult)resultValue,
                                                                      _modelSpec)
                                                                  .GetModel<object>(_modelSpec.AllowNull,
                                                                      _modelSpec.ModelPropertyName);
                                                              _methodInfo.Invoke(testClass, model);
                                                          }
                                                          else
                                                          {
                                                              _methodInfo.Invoke(testClass, resultValue);
                                                          }
                                                      });
        }

        private MethodResult WrapWithResultAndExceptionHandling(Action toInvoke)
        {
            try
            {
                toInvoke();
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is AssertException)
                {
                    return new FailedResult(_methodInfo, ex.InnerException, DisplayName);
                }
                throw new ApplicationException("Unit test failed not from an assertion", ex.InnerException);
            }
            return new PassedResult(_methodInfo, DisplayName);
        }

        private object SetupTester(object testClass)
        {
            if (!(testClass is AbstractMvcBaseTest))
            {
                throw new InvalidOperationException(
                    "TestClass that uses DataScenarioFact must derive from AbstractMVCBaseTest");
            }

            Type testerType = typeof(AbstractMvcBaseTest.Tester<>);
            // creating Tester<TController>
            Type testerGenericType = testerType.MakeGenericType(_controllerInvocation == null
                ? _methodInfo.MethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0]
                : _controllerInvocation.ControllerType);

            object tester = Activator.CreateInstance(testerGenericType, (AbstractMvcBaseTest)testClass);
            
            dynamic testerDynamic = tester;
            _controllerFact.DataScenarios.ToList().ForEach(e =>
                                                           {
                                                               MethodInfo genericMethodInfo =
                                                                   testerGenericType.GetMethod("SetupDataScenario")
                                                                       .MakeGenericMethod(e);
                                                               genericMethodInfo.Invoke(tester, null);
                                                           });

            return tester;
        }
    }
}