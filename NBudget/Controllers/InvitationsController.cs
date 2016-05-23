using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using Microsoft.AspNet.Identity;
using NBudget.Controllers.ApiControllers;
using System;
using System.Net.Mail;
using System.Net;
using SendGrid;
using System.Threading.Tasks;

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

        

        private async Task NotifyRecipientByEmail(string recipientEmail, string ownerName)
        {
            // Create the email object first, then add the properties.
            var message = new SendGridMessage();

            // Add the message properties.
            message.From = new MailAddress("info@nbudget.com");
            message.To = new MailAddress[] { new MailAddress(recipientEmail) };
            message.Subject = "NBudget invitation";
            message.Text = "You've been invited to edit " + ownerName + "'s budget. Please register or log in to NBudget at http://nbudget.azurewebsites.net to accept the invitation.";

            var credentials = new NetworkCredential("azure_d53d802de84c56249d05133a8c55abf4@azure.com", "AlmaKorte91");
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(message);
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