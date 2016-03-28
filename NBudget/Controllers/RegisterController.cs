using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBudget.Controllers
{
    public class RegisterController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Register";

            return View();
        }
    }
}
