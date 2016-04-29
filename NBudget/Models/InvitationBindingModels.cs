using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class NewInvitationBindingModel 
    {
        [Required]
        public string RecipientEmail { get; set; }
    }

    public class UpdateInvitationBindingModel
    {
        [Required]
        public InvitationStatus InvitationStatus { get; set; }
    }
}