using System.ComponentModel.DataAnnotations;

namespace NBudget.Models
{
    public class CategoryDTO
    {
        [Required]
        public string Name { get; set; }
    }
}