namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class TransactionUserRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Transactions", new[] { "Owner_Id" });
            AlterColumn("dbo.Transactions", "Owner_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Transactions", "Owner_Id");
            AddForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Transactions", new[] { "Owner_Id" });
            AlterColumn("dbo.Transactions", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Transactions", "Owner_Id");
            AddForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
