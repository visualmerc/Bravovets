using System;
using System.Collections.Generic;
using System.Web;

namespace ProfSite.Tests.Infrastructure
{
    public abstract class AbstractBaseTest : IDisposable
    {
        //TODO:Flush out application data needs and expand data scenarios

        protected List<object[]> SentMessages = new List<object[]>();

        protected AbstractBaseTest()
        {
            HttpContext.Current = null;
        }

        public void Dispose()
        {
            HttpContext.Current = null;
        }
    }
}