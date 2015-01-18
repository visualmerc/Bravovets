using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProfSiteUS.Startup))]
namespace ProfSiteUS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
