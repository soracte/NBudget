using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NBudget.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBudget.Controllers.ApiControllers
{
    [RoutePrefix("api/Users")]
    public class UsersController : BaseApiController
    {
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

        [Route("SetInvitees")]
        [HttpPut]
        public async Task<IHttpActionResult> PutInviteesForCurrentUser([FromBody] UpdateInviteesBindingModel inviteeIds)
        {
            ApplicationUser currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            List<ApplicationUser> invitees = currentUser.Invitees;

            if (invitees == null)
            {
                invitees = new List<ApplicationUser>();
                currentUser.Invitees = invitees;
            }
            else
            {
                invitees.Clear();
            }

            foreach (string inv in inviteeIds.Invitees)
            {
                ApplicationUser user = UserManager.FindById(inv);
                if (user == null)
                {
                    return NotFound();

                }

                invitees.Add(user);
            }

            await UserManager.UpdateAsync(currentUser);

            return Ok();
        }
    }
}