using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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