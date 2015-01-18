using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfSite.Utils;

namespace ProfSite.Controllers
{
    public class ScheduledJobController : Controller
    {
        //
        // GET: /ScheduledJob/
        public ActionResult DeliverQueuedMessages()
        {
            QueueContentHelper.DeliverEligibleQueuedMessages();
            return View();
        }
	}
}