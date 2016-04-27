namespace NBudget.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvitations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientEmail = c.String(),
                        Status = c.Int(nullable: false),
                        Sender_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Sender_Id)
                .Index(t => t.Sender_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invitations", "Sender_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Invitations", new[] { "Sender_Id" });
            DropTable("dbo.Invitations");
        }
    }
}
