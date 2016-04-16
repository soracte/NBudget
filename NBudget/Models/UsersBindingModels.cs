using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NBudget.Models
{
    // Models used as parameters to AccountController actions.

    public class UpdateRolesBindingModel
    {
        [Required]
        public string[] NewRoles { get; set; }
    }

    public class UpdateInviteesBindingModel
    {
        [Required]
        public string[] Invitees { get; set; }
    }

    public class UpdateInviterBindingModel
    {
        [Required]
        public string[] Inviters { get; set; }
    }
}
