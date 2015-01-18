using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using BravoVets.DomainObject;
using ProfSite.Controllers;
using TestHelper;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading;

namespace ProfSite.Tests.Infrastructure
{
    public abstract class AbstractMvcBaseTest : AbstractBaseTest
    {
        protected AbstractMvcBaseTest()
        {
            CreateHttpContext();
        }

        private static void CreateHttpContext()
        {
            TextWriter tw = new StringWriter();
            HttpWorkerRequest wr = new SimpleWorkerRequest("/webappt", "c:\\inetpub\\wwwroot\\webapp\\", "default.aspx",
                "", tw);

            HttpContext.Current = new HttpContext(wr)
                                  {
                                      User =
                                          new GenericPrincipal(
                                          new GenericIdentity(AbstractDataScenario.TestUserName), null)
                                  };
        }


        protected BravoVetsUser SetClaimsPrincipal(int bravoVetsUserId)
        {
            var existingUser = new BravoVetsUser { AcceptedTandC = false, BravoVetsUserId = bravoVetsUserId, };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.FirstName + " " + existingUser.Lastname),
                new Claim("MerckLfwId", existingUser.MerckId.ToString(CultureInfo.InvariantCulture)),
                new Claim("BravoVetsUserId", existingUser.BravoVetsUserId.ToString(CultureInfo.InvariantCulture))
            };

            if (existingUser.AcceptedTandC)
            {
                var acceptRole = new Claim(ClaimTypes.Role, "AcceptedToc");
                claims.Add(acceptRole);
            }

            var identity = new ClaimsIdentity(claims, "Custom");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            var sessionToken = new SessionSecurityToken(claimsPrincipal, TimeSpan.FromHours(8));

            Thread.CurrentPrincipal = claimsPrincipal;

            return existingUser;

        }

        protected Tester<TController> For<TController>()
            where TController : AbstractBaseController, new()
        {
            return new Tester<TController>(this);
        }

        public class Tester<TController>
            where TController : AbstractBaseController, new()
        {
            private readonly AbstractMvcBaseTest _parent;

            private readonly List<Action<TController>> actions = new List<Action<TController>>();
            private readonly MockWebService mockWebService = new MockWebService();
            private readonly List<AbstractDataScenario> dataScenarios = new List<AbstractDataScenario>();

            public Tester(AbstractMvcBaseTest parent)
            {
                _parent = parent;
            }

            public TResult Execute<TResult>(Func<TController, TResult> action)
                where TResult : ActionResult
            {
                try
                {

                    TController ctrl = InitializeController();
                    TResult result = action(ctrl);
                    SaveChanges(ctrl);
                    return result;
                }
                finally
                {
                    mockWebService.Stop();
                }
            }


            public object Execute(string actionName, params object[] parametrs)
            {
                try
                {
                    TController ctrl = InitializeController();
                    MethodInfo info = typeof(TController).GetMethod(actionName);
                    if (info == null)
                    {
                        throw new InvalidOperationException(
                            string.Format("{0} action was not found on the controller {1}",
                                actionName, typeof(TController).Name));
                    }
                    SaveChanges(ctrl);
                    object result = info.Invoke(ctrl, parametrs);
                    // Save changes to store anything affected during the action execution
                    SaveChanges(ctrl);

                    return result;
                }
                finally { mockWebService.Stop(); }
            }

            public Tester<TController> SetupWebCall(params MockWebService.MockCall[] mockCalls)
            {
                mockWebService.Run(mockCalls);
                return this;
            }

            private TController InitializeController()
            {
                var ceManager = new SqlCeManager();
                ceManager.CreateBravoVetsCeDatabase();


                foreach (AbstractDataScenario ds in dataScenarios)
                {
                    ds.CreateData();
                }

                var ctrl = new TController();

                foreach (var act in actions)
                {
                    act(ctrl);
                }

                return ctrl;
            }

            private void SaveChanges(TController controller)
            {
                _parent.SentMessages = controller.MessagesToSend;
            }

            public Tester<TController> Setup(Action<TController> action)
            {
                actions.Add(action);
                return this;
            }

            public Tester<TController> SetupDataScenario<TDataScenario>()
                where TDataScenario : AbstractDataScenario, new()
            {
                dataScenarios.Add(new TDataScenario());
                return this;
            }
        }
    }
}