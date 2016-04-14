using System.Web.Mvc;

namespace NBudget.Controllers
{
    public class CategoryManagementController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Categories";

            return View();
        }
    }
}
