using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProfSite.Tests.Infrastructure
{
    public static class ReflectionUtils
    {
        public static T GetAttribute<T>(MethodInfo methodInfo)
            where T : Attribute
        {
            // trying to get attribute first from a fact than from a fixture it is in.
            return
                (T) methodInfo.GetCustomAttributes(typeof (T), false).FirstOrDefault() ??
                (T) methodInfo.DeclaringType.GetCustomAttributes(typeof (T), false).FirstOrDefault();
        }


        public static List<T> GetAttributes<T>(MethodInfo methodInfo)
            where T : Attribute
        {
            var attributes = new List<T>();

            var fixtureAttribute = (T) methodInfo.DeclaringType.GetCustomAttributes(typeof (T), false).FirstOrDefault();
            if (fixtureAttribute != null)
                attributes.Add(fixtureAttribute);


            var methodAttribute = (T) methodInfo.GetCustomAttributes(typeof (T), false).FirstOrDefault();

            if (methodAttribute != null)
                attributes.Add(methodAttribute);


            return attributes;
        }
    }
}