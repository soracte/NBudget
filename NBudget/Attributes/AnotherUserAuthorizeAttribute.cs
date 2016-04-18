using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NBudget.Models;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NBudget.Attributes
{
    public class AnotherUserAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            ApplicationUserManager userManager = actionContext.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string currentUserId = actionContext.RequestContext.Principal.Identity.GetUserId();
            ApplicationUser currentUser = userManager.FindById(currentUserId);

            string requestedUserId = actionContext.Request.GetRouteData().Values["userId"] as string;

            return currentUser.Id == requestedUserId || currentUser.Inviters.Select(inv => inv.Id).Contains(requestedUserId);
        }
    }
}