﻿using System.Web.Mvc;

namespace NBudget.Controllers
{
    public class TransactionListController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}