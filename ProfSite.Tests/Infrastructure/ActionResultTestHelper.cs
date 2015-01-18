using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Xunit.Sdk;

namespace ProfSite.Tests.Infrastructure
{
    public static class ActionResultTestHelper
    {
        public static ActionResult ConformsToSpecifications(this ActionResult actionResult)
        {
            MethodBase callingMethod = new StackTrace().GetFrame(1).GetMethod();
            ActionResultSpecificationAttribute actionResultSpec =
                callingMethod.GetCustomAttributes(typeof (ActionResultSpecificationAttribute), false).OfType
                    <ActionResultSpecificationAttribute>().FirstOrDefault();
            ModelSpecificationAttribute modelScpec =
                callingMethod.GetCustomAttributes(typeof (ModelSpecificationAttribute), false).OfType
                    <ModelSpecificationAttribute>().FirstOrDefault();

            if (actionResultSpec == null)
            {
                throw new InvalidOperationException(
                    "ConformsToSpecifications was called in a method that does not have ActionResultSepecificationAttribute in the declaration");
            }

            ActionResult @return = ConformsToSpecifications(actionResult, actionResultSpec);

            if (modelScpec != null)
            {
                @return = ConformsToModelSpecifications(@return, modelScpec);
            }

            return @return;
        }

        public static T GetModel<T>(this ActionResult actionResult, bool modelNullable = false,
            string modelPropertyName = "Model")
        {
            MethodBase callingMethod = new StackTrace().GetFrame(1).GetMethod();
            ModelSpecificationAttribute modelScpec =
                callingMethod.GetCustomAttributes(typeof (ModelSpecificationAttribute), false).OfType
                    <ModelSpecificationAttribute>().FirstOrDefault();

            object obj = GetModel(actionResult, modelScpec == null ? modelPropertyName : modelScpec.ModelPropertyName,
                modelScpec == null ? false : modelScpec.AllowNull);

            if (!typeof (T).IsAssignableFrom(obj.GetType()))
            {
                throw new AssertException(string.Format("Expecting model type '{0}'. But actual model type is '{1}'",
                    typeof (T), obj.GetType()));
            }

            return (T) obj;
        }

        public static object GetModel(ActionResult actionResult, string modelPropertyName, bool allowNull = false)
        {
            PropertyInfo modelProperty = actionResult.GetType().GetProperty(modelPropertyName);

            if (modelProperty == null)
            {
                throw new AssertException(string.Format("Model property is not found on '{0}' ActionResult",
                    actionResult.GetType()));
            }

            if (modelProperty.GetGetMethod() == null)
            {
                throw new AssertException(string.Format("Model property does not have a getter on '{0}' ActionResult",
                    actionResult.GetType()));
            }

            object obj = modelProperty.GetGetMethod().Invoke(actionResult, null);

            if (!allowNull && obj == null)
            {
                throw new AssertException("Model cannot be null");
            }

            return obj;
        }

        internal static TActionResult ConformsToSpecifications<TActionResult>(TActionResult actionResult,
            ActionResultSpecificationAttribute specs)
            where TActionResult : ActionResult
        {
            if (!specs.AllowNull && actionResult == null)
            {
                throw new AssertException("ActionResult must not be null");
            }

            if (specs.ResultType != actionResult.GetType())
            {
                throw new AssertException(
                    string.Format("ActionResult is excpected to be of type '{0}' but was of type '{1}'",
                        specs.ResultType, actionResult.GetType()));
            }

            return actionResult;
        }

        internal static TActionResult ConformsToModelSpecifications<TActionResult>(TActionResult actionResult,
            ModelSpecificationAttribute specs)
        {
            PropertyInfo modelProperty = actionResult.GetType().GetProperty(specs.ModelPropertyName);

            if (modelProperty == null)
            {
                throw new AssertException(
                    string.Format("Model property '{0}' was not found on an ActionResult of type '{1}'",
                        specs.ModelPropertyName, actionResult.GetType()));
            }

            if (modelProperty.GetGetMethod() == null)
            {
                throw new AssertException(string.Format("Model property '{0}' must have a getter", modelProperty.Name));
            }

            object actualModel = modelProperty.GetGetMethod().Invoke(actionResult, null);

            if (!specs.AllowNull && actualModel == null)
            {
                throw new AssertException(string.Format("Model of the ActionResult cannot be null"));
            }

            if (!actualModel.IsAnonymous())
            {
                if (specs.ModelType != actualModel.GetType())
                {
                    throw new AssertException(string.Format(
                        "Model property must be of type '{0}' but was of type '{1}'",
                        specs.ModelType, modelProperty.GetGetMethod().ReturnType));
                }
            }

            return actionResult;
        }

        public static bool IsAnonymous(this object type)
        {
            return type.GetType().Name.Contains("AnonymousType");
        }
    }
}