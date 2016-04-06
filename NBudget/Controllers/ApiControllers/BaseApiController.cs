using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;

namespace NBudget.Controllers.ApiControllers
{
    public class BaseApiController : ApiController
    {
        private ApplicationRoleManager _roleManager = null;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>(); 
            }
        }
    }
}
