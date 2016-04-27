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

        // PUT: api/Invitations/5
        [ResponseType(typeof(void))]
        [Obsolete]
        public IHttpActionResult PutInvitationStatus(int id, UpdateInvitationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Invitation invitation = db.Invitations.Find(id);

            if (!OwnInvitationExists(invitation))
            {
                return NotFound();
            }

            invitation.Status = model.InvitationStatus;
            db.Entry(invitation).State = EntityState.Modified;

            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
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
            AddInviteeRelationship(invitation, model.RecipientEmail);


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

            db.Invitations.Remove(invitation);
            db.SaveChanges();

            return Ok(invitation);
        }

        private bool OwnInvitationExists(Invitation invitation)
        {
            return (invitation == null && invitation.Sender.Id == User.Identity.GetUserId());
        }

        private void AddInviteeRelationship(Invitation invitation, string recipientEmail)
        {
            ApplicationUser recipientUser = UserManager.FindByEmail(recipientEmail);
            if (recipientUser == null)
            {
                return;
            }

            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

            recipientUser.Inviters.Add(currentUser);
            currentUser.Invitees.Add(recipientUser);

            UserManager.Update(recipientUser);
            UserManager.Update(currentUser);
            invitation.Status = InvitationStatus.Active;
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