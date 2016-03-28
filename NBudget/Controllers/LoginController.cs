using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBudget.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Log in";

            return View();
        }
    }
}
