namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MultipleInviters : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Inviter_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "Inviter_Id" });
            CreateTable(
                "dbo.ApplicationUserApplicationUsers",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id1 = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.ApplicationUser_Id1 })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            DropColumn("dbo.AspNetUsers", "Inviter_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Inviter_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserApplicationUsers", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserApplicationUsers", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.ApplicationUserApplicationUsers", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserApplicationUsers");
            CreateIndex("dbo.AspNetUsers", "Inviter_Id");
            AddForeignKey("dbo.AspNetUsers", "Inviter_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
