using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using Microsoft.AspNet.Identity;
using NBudget.Controllers.ApiControllers;
using System;
using System.Net.Mail;

namespace NBudget.Controllers
{
    public class InvitationsController : BaseApiController
    {
        public delegate void ProcessUserDelegate(ApplicationUser user);

        // GET: api/Invitations
        public IHttpActionResult GetInvitations()
        {
            string currentUserId = User.Identity.GetUserId();
            var invs = db.Invitations.Where(i => i.Sender.Id == currentUserId).Select(i => new
            {
                Id = i.Id,
                CreationDate = i.CreationDate,
                RecipientEmail = i.RecipientEmail,
                Status = i.Status.ToString()
            });

            return Ok(invs);
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
            Invitation invitation = new Invitation
            {
                RecipientEmail = model.RecipientEmail,
                Sender = currentUser,
                Status = InvitationStatus.Pending,
                CreationDate = DateTime.Now
            };

            ProcessInviteeRelationship(invitation);
            NotifyRecipientByEmail(model.RecipientEmail, currentUser.FirstName + ' ' + currentUser.LastName);

            db.Invitations.Add(invitation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = invitation.Id }, new { id = invitation.Id });
        }

        // DELETE: api/Invitations/5
        [ResponseType(typeof(Invitation))]
        public IHttpActionResult DeleteInvitation(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Entry(invitation).Reference(i => i.Sender).Load();

            if (!OwnInvitationExists(invitation))
            {
                return NotFound();
            }

            ProcessInviteeRelationship(invitation);

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

        private void NotifyRecipientByEmail(string recipientEmail, string ownerName)
        {
            SmtpClient smtpClient = new SmtpClient("localhost", 25);

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("nbudget@example.com", "NBudget");
            mail.To.Add(new MailAddress(recipientEmail));
            mail.Body = "You've been invited to edit " + ownerName + "'s budget. Please register or log in to NBudget to accept the invitation.";

            smtpClient.Send(mail);
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