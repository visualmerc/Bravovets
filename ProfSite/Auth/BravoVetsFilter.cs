using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BravoVets.DomainObject;

namespace ProfSite.Auth
{
    public class BravoVetsFilter : ActionFilterAttribute
    {
        private IBravoVetsAuthManager _authManager;

        public BravoVetsFilter(IBravoVetsAuthManager authManager)
        {
            this._authManager = authManager;
        }

        public BravoVetsFilter() : this(new BravoVetsAuthManager())
        {        
        }

        /// <summary>
        /// The main method that is fired by MVC. Checks values injected by Merck's Login Framework, to determine
        /// if the current user has been authorized
        /// </summary>
        /// <param name="context">The web context, passed by MVC</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            this._authManager.SyncUserWithLfw();
        }

    }
}