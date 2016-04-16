using Microsoft.AspNet.Identity.EntityFramework;

namespace NBudget.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IdentityUser Owner { get; set; }
    }
}