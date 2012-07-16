using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FalconWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
