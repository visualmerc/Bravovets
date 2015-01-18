using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProfSite.Startup))]
namespace ProfSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
