using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProfSite.Controllers
{
    public class BravopetsController : AbstractBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}