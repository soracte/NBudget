using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using Microsoft.AspNet.Identity;
using NBudget.Controllers.ApiControllers;
using System.Threading.Tasks;

namespace NBudget.Controllers
{
    public class InvitationsController : BaseApiController
    {
        public delegate void ProcessUserDelegate(ApplicationUser user);

        // GET: api/Invitations
        public IQueryable<Invitation> GetInvitations()
        {
            string currentUserId = User.Identity.GetUserId();
            return db.Invitations.Where(i => i.Sender.Id == currentUserId);
        }

        // GET: api/Invitations/5
        [ResponseType(typeof(Invitation))]
        public IHttpActionResult GetInvitation(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            if (!OwnInvitationExists(invitation))
            {
                return NotFound();
            }

            return Ok(invitation);
        }

        // POST: api/Invitations
        [ResponseType(typeof(Invitation))]
        public IHttpActionResult PostInvitation(NewInvitationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            Invitation invitation = new Invitation { RecipientEmail = model.RecipientEmail, Sender = currentUser, Status = InvitationStatus.Pending };
            ProcessInviteeRelationship(invitation);


            db.Invitations.Add(invitation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = invitation.Id }, new { id = invitation.Id });
        }

        // DELETE: api/Invitations/5
        [ResponseType(typeof(Invitation))]
        public IHttpActionResult DeleteInvitation(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            if (!OwnInvitationExists(invitation))
            {
                return NotFound();
            }

            ProcessInviteeRelationship(invitation);

            db.Invitations.Remove(invitation);
            db.SaveChanges();

            return Ok(invitation);
        }

        private bool OwnInvitationExists(Invitation invitation)
        {
            return (invitation != null && invitation.Sender.Id == User.Identity.GetUserId());
        }

        private void ProcessInviteeRelationship(Invitation invitation)
        {
            ApplicationUser recipientUser = UserManager.FindByEmail(invitation.RecipientEmail);
            if (recipientUser == null)
            {
                return;
            }

            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (invitation.Status == InvitationStatus.Pending)
            {
                recipientUser.Inviters.Add(currentUser);
                currentUser.Invitees.Add(recipientUser);
                invitation.Status = InvitationStatus.Active;
            }

            else if (invitation.Status == InvitationStatus.Active)
            {
                recipientUser.Inviters.Remove(currentUser);
                currentUser.Invitees.Remove(recipientUser);
                invitation.Status = InvitationStatus.Inactive;
            }

            UserManager.Update(recipientUser);
            UserManager.Update(currentUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}