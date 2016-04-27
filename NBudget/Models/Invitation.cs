using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class Invitation 
    {
        public int Id { get; set; }
        public ApplicationUser Sender { get; set; }
        public string RecipientEmail { get; set; }
        public InvitationStatus Status { get; set; }
    }

    public enum InvitationStatus
    {
        Pending, Active, Inactive
    }
}