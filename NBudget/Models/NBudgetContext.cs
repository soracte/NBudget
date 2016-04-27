using Microsoft.AspNet.Identity.EntityFramework;

namespace NBudget.Models
{
    public class NBudgetContext : IdentityDbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public NBudgetContext() : base("name=NBudgetContext")
        {
        }

        public static NBudgetContext Create()
        {
            return new NBudgetContext();
        }

        public System.Data.Entity.DbSet<NBudget.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<NBudget.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<NBudget.Models.Invitation> Invitations { get; set; }
    }
}
