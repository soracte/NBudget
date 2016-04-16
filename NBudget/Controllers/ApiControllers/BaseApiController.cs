using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;

namespace NBudget.Controllers.ApiControllers
{
    public class BaseApiController : ApiController
    {
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
