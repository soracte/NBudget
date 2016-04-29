using System;
using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class Invitation 
    {
        public int Id { get; set; }

        [Required]
        public ApplicationUser Sender { get; set; }

        [Required]
        public string RecipientEmail { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public InvitationStatus Status { get; set; }

        internal Invitation Include()
        {
            throw new NotImplementedException();
        }
    }

    public enum InvitationStatus
    {
        Pending, Active, Inactive
    }
}