using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using BravoVets.DomainObject;
using ProfSite.Auth;
using ProfSite.Controllers;
using ProfSite.Models;
using ProfSite.Tests.Infrastructure;
using ProfSite.Utils;
using Xunit;

namespace ProfSite.Tests
{

    public class FacebookControllerFixture : AbstractMvcBaseTest
    {
        [ControllerFact]
        [ControllerInvocation(typeof(FacebookController), "OAuthRedirect")]
        public void CanOAuthRedirect(RedirectResult results)
        {
            Assert.Equal("https://www.facebook.com/dialog/oauth?client_id=219370301599005&redirect_uri=http://local.bravovets.co.uk/facebook/oauth_callback&scope=email,read_stream,user_likes,user_videos,user_status,user_friends,status_update,share_item,friends_status,manage_notifications,publish_actions,manage_pages", results.Url);
        }


        [ControllerFact]
        [ControllerInvocation(typeof(FacebookController), "oauth_callback", "en-US", "")]
        public void CanHandleOAuthRedirectDenied(RedirectResult results)
        {
            Assert.IsType<RedirectResult>(results);
        }

        [Fact]
        public void TokenExchangeoauth_callback_noPages()
        {
            SetClaimsPrincipal(2);

            bool accessTokenWasCalled = false;
            Func<string, dynamic, MockWebService.MockWebResponse> accessToken =
                (queryString, requestData) =>
                {
                    accessTokenWasCalled = true;
                    return new MockWebService.MockWebResponse { Data = "access_token=ssss&exires=23333" };
                };

            bool getProfileWasCalled = false;
            Func<string, dynamic, MockWebService.MockWebResponse> getProfile =
               (queryString, requestData) =>
               {
                   getProfileWasCalled = true;
                   return new MockWebService.MockWebResponse { Data = "{id:1234,likes:2}" };
               };

            bool getPagesWasCalled = false;

            Func<string, dynamic, MockWebService.MockWebResponse> getPages =
             (queryString, requestData) =>
             {
                 getPagesWasCalled = true;
                 return new MockWebService.MockWebResponse { Data = "{data:[]}" };
             };

            FacebookHelper.facebook_graph_api = "http://localhost:8080";

            var results = (ActionResult)For<FacebookController>()
                .SetupWebCall(new MockWebService.MockCall
                              {
                                  Url = "http://localhost:8080/oauth/access_token",
                                  WebAction = accessToken
                              },new MockWebService.MockCall
                               {
                                   Url = "http://localhost:8080/me",
                                   WebAction = getProfile
                               },new MockWebService.MockCall
                                 {
                                     Url = "http://localhost:8080/1234/accounts",
                                     WebAction = getPages
                                 })
                .Execute("oauth_callback", "393873");

            Assert.IsType(typeof(RedirectResult),results);
            Assert.Equal("/EditProfile",((RedirectResult)results).Url);

            Assert.NotNull(results);
            Assert.True(accessTokenWasCalled);
            Assert.True(getProfileWasCalled);
            Assert.True(getPagesWasCalled);
        }

        [Fact]
        public void TokenExchangeoauth_callback_withPages()
        {
            SetClaimsPrincipal(2);

            bool accessTokenWasCalled = false;
            Func<string, dynamic, MockWebService.MockWebResponse> accessToken =
                (queryString, requestData) =>
                {
                    accessTokenWasCalled = true;
                    return new MockWebService.MockWebResponse { Data = "access_token=ssss&exires=23333" };
                };

            bool getProfileWasCalled = false;
            Func<string, dynamic, MockWebService.MockWebResponse> getProfile =
               (queryString, requestData) =>
               {
                   getProfileWasCalled = true;
                   return new MockWebService.MockWebResponse { Data = "{id:1234,likes:2}" };
               };

            bool getPagesWasCalled = false;

            Func<string, dynamic, MockWebService.MockWebResponse> getPages =
             (queryString, requestData) =>
             {
                 getPagesWasCalled = true;
                 return new MockWebService.MockWebResponse { Data = "{data:[{id:12,name:'test',access_token:'123492',perms:[]}]}" };
             };

            FacebookHelper.facebook_graph_api = "http://localhost:8080";

            var results = (ActionResult)For<FacebookController>()
                .SetupWebCall(new MockWebService.MockCall
                {
                    Url = "http://localhost:8080/oauth/access_token",
                    WebAction = accessToken
                }, new MockWebService.MockCall
                {
                    Url = "http://localhost:8080/me",
                    WebAction = getProfile
                }, new MockWebService.MockCall
                {
                    Url = "http://localhost:8080/1234/accounts",
                    WebAction = getPages
                })
                .Execute("oauth_callback", "393873");

            
            Assert.IsType(typeof(ViewResult),results);
            var model = results.GetModel<List<FacebookPage>>();
            Assert.NotNull(model);
            Assert.Equal(2,model.Count);
            
            Assert.True(accessTokenWasCalled);
            Assert.True(getProfileWasCalled);
            Assert.True(getPagesWasCalled);
        }

    }
}