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

namespace NBudget.Controllers
{
    public class InvitationsController : BaseApiController
    {
        private NBudgetContext db = new NBudgetContext();

        // GET: api/Invitations
        public IQueryable<Invitation> GetInvitations()
        {
            return db.Invitations.Where(i => i.Sender.Id == User.Identity.GetUserId());
        }

        // GET: api/Invitations/5
        [ResponseType(typeof(Invitation))]
        public IHttpActionResult GetInvitation(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            if (!ownInvitationExists(invitation))
            {
                return NotFound();
            }

            return Ok(invitation);
        }

        // PUT: api/Invitations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutInvitationStatus(int id, InvitationStatus status)
        {
            Invitation invitation = db.Invitations.Find(id);

            if (!ownInvitationExists(invitation))
            {
                return NotFound();
            }

            invitation.Status = status;
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

            db.Invitations.Add(invitation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = invitation.Id }, invitation);
        }

        // DELETE: api/Invitations/5
        [ResponseType(typeof(Invitation))]
        public IHttpActionResult DeleteInvitation(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            if (!ownInvitationExists(invitation))
            {
                return NotFound();
            }

            db.Invitations.Remove(invitation);
            db.SaveChanges();

            return Ok(invitation);
        }

        private bool ownInvitationExists(Invitation invitation)
        {
            return (invitation == null && invitation.Sender.Id == User.Identity.GetUserId());
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