using Microsoft.AspNet.Identity;
using NBudget.Attributes;
using NBudget.Models;
using System.Collections.Generic;
using System.Linq;
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
            List<ApplicationUser> users = new List<ApplicationUser>(UserManager.Users.ToList());
            var retval = users.Select(user => new
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Invitees = user.Invitees.Select(inv => inv.Id),
                Inviter = user.Inviters.Select(inv => inv.Id)
            });

            return Ok(retval);
        }

        [Route("UpdateRoles/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRolesForUser(string id, [FromBody] UpdateRolesBindingModel roles)
        {
            await UserManager.AddToRolesAsync(id, roles.NewRoles);
            return Ok();
        }

        [Route("ModifyInviter/{id}")]
        public async Task<IHttpActionResult> PutInviters(string id, [FromBody] UpdateInviterBindingModel invModel)
        {
            ApplicationUser currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            List<ApplicationUser> inviters = currentUser.Inviters;

            if (inviters == null)
            {
                inviters = new List<ApplicationUser>();
                currentUser.Inviters = inviters;
            }
            else
            {
                inviters.Clear();
            }

            foreach (string inv in invModel.Inviters)
            {
                ApplicationUser inviter = UserManager.FindById(inv);
                if (inviter == null)
                {
                    return NotFound();

                }

                inviters.Add(inviter);
                inviter.Invitees.Add(currentUser);

                await UserManager.UpdateAsync(inviter);
            }

            await UserManager.UpdateAsync(currentUser);

            return Ok();
            
        }
        

        [Route("Invitees")]
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
                ApplicationUser invited = UserManager.FindById(inv);
                if (invited == null)
                {
                    return NotFound();

                }

                invitees.Add(invited);
                invited.Inviters.Add(currentUser);

                await UserManager.UpdateAsync(invited);
            }

            await UserManager.UpdateAsync(currentUser);

            return Ok();
        }

        [Route("Invitees")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteInviteesOfCurrentUser([FromUri] string[] inviteesToDelete)
        {
            ApplicationUser currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            List<ApplicationUser> currentInvitees = currentUser.Invitees;
            if (currentInvitees == null)
            {
                return Ok();
            }

            foreach (ApplicationUser invitee in currentInvitees)
            {
                invitee.Inviters.RemoveAll(inviter => inviter.Id == currentUser.Id);
                await UserManager.UpdateAsync(invitee);
            }

            currentInvitees.RemoveAll(inv => inviteesToDelete.Contains(inv.Id));
            await UserManager.UpdateAsync(currentUser);
            return Ok();
        }


    }
}