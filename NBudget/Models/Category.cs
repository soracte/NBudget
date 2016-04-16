using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class Category : OwnedEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}