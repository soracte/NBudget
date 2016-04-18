namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RequiredOwnerAndCategoryName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Categories", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Categories", new[] { "Owner_Id" });
            DropIndex("dbo.Transactions", new[] { "Owner_Id" });
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Categories", "Owner_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Transactions", "Owner_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Categories", "Owner_Id");
            CreateIndex("dbo.Transactions", "Owner_Id");
            AddForeignKey("dbo.Categories", "Owner_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Categories", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Transactions", new[] { "Owner_Id" });
            DropIndex("dbo.Categories", new[] { "Owner_Id" });
            AlterColumn("dbo.Transactions", "Owner_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Categories", "Owner_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Categories", "Name", c => c.String());
            CreateIndex("dbo.Transactions", "Owner_Id");
            CreateIndex("dbo.Categories", "Owner_Id");
            AddForeignKey("dbo.Transactions", "Owner_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Categories", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
