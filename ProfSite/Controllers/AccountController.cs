using System.Web.Mvc;

namespace ProfSite.Controllers
{
    public class AccountController : AbstractBaseController
    {
        public ActionResult LogOut()
        {
            this.AuthManager.InvalidateCurrentIdentityInfo();
            return Redirect("/bravecto");
        }
    }
}