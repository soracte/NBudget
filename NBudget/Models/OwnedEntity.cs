using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class OwnedEntity
    {
        [Required]
        public virtual ApplicationUser Owner { get; set; }

    }
}