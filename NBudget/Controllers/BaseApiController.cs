using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using NBudget.Models;

namespace NBudget.Controllers.ApiControllers
{
    public class BaseApiController : ApiController
    {
        public NBudgetContext db
        {
            get
            {
                return Request.GetOwinContext().Get<NBudgetContext>();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return Request.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

    }
}
