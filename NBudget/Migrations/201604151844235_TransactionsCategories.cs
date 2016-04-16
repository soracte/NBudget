namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class TransactionsCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Int(nullable: false),
                        Reason = c.String(nullable: false),
                        Category_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id, cascadeDelete: true)
                .Index(t => t.Category_Id);
            
            AddColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Categories", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Transactions", new[] { "Category_Id" });
            DropIndex("dbo.Categories", new[] { "Owner_Id" });
            DropColumn("dbo.AspNetUsers", "Discriminator");
            DropTable("dbo.Transactions");
            DropTable("dbo.Categories");
        }
    }
}
