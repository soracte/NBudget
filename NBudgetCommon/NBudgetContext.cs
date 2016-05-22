using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Azure;
using Microsoft.WindowsAzure;

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
            System.Console.WriteLine("Paramless NBudgetContext ctor invoked.");
        }

        public NBudgetContext(string connString) : base(connString)
        {
        }

        public static NBudgetContext Create()
        {
            return new NBudgetContext("Server=tcp:nbsqldb.database.windows.net,1433;Data Source=nbsqldb.database.windows.net;Initial Catalog=nbudgetsqldb;Persist Security Info=False;User ID=nbudget;Password=AlmaKorte91;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        public System.Data.Entity.DbSet<Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<Category> Categories { get; set; }

        public System.Data.Entity.DbSet<Invitation> Invitations { get; set; }

        public System.Data.Entity.DbSet<ReportHeader> ReportHeaders { get; set; }

    }
}
