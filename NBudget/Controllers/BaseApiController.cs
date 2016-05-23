using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using NBudget.Models;
using Microsoft.AspNet.Identity;

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

        protected void ProcessInviteeRelationship(Invitation invitation)
        {
            ApplicationUser recipient = UserManager.FindByEmail(invitation.RecipientEmail);
            if (recipient == null)
            {
                return;
            }

            ApplicationUser sender = UserManager.FindByEmail(invitation.Sender.Email);

            if (invitation.Status == InvitationStatus.Pending)
            {
                recipient.Inviters.Add(sender);
                sender.Invitees.Add(recipient);
                invitation.Status = InvitationStatus.Active;
            }

            else if (invitation.Status == InvitationStatus.Active)
            {
                recipient.Inviters.Remove(sender);
                sender.Invitees.Remove(recipient);
                invitation.Status = InvitationStatus.Inactive;
            }

            UserManager.Update(recipient);
            UserManager.Update(sender);
        }

    }
}
