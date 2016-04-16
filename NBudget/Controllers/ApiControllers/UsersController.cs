using Microsoft.AspNet.Identity.Owin;
using NBudget.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBudget.Controllers.ApiControllers
{
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        [Route("")]
        public IHttpActionResult GetUsers()
        {
            return Ok(UserManager.Users);
        }

        [Route("UpdateRoles/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRolesForUser(string id, [FromBody] UpdateRolesBindingModel roles)
        {
            await UserManager.AddToRolesAsync(id, roles.NewRoles);
            return Ok();

        }
    }
}