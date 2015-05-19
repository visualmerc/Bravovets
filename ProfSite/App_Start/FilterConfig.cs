using System.Web;
using System.Web.Mvc;
using ProfSite.Auth;

namespace ProfSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //TODO TEMP
            //IBravoVetsAuthManager authManager = new BravoVetsAuthManager();
            IBravoVetsAuthManager authManager = new BravoVetsTestableAuthManager();

            // TODO: Make a custom error filter
            filters.Add(new HandleErrorAttribute());
            filters.Add(new BravoVetsFilter(authManager));
        }
    }
}
