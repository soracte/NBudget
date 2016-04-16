namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class TransactionOwner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Transactions", "Owner_Id");
            AddForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Transactions", new[] { "Owner_Id" });
            DropColumn("dbo.Transactions", "Owner_Id");
        }
    }
}
