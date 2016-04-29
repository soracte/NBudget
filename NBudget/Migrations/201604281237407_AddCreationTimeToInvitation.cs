namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddCreationTimeToInvitation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invitations", "Sender_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Invitations", new[] { "Sender_Id" });
            AddColumn("dbo.Invitations", "CreationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Invitations", "RecipientEmail", c => c.String(nullable: false));
            AlterColumn("dbo.Invitations", "Sender_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Invitations", "Sender_Id");
            AddForeignKey("dbo.Invitations", "Sender_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invitations", "Sender_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Invitations", new[] { "Sender_Id" });
            AlterColumn("dbo.Invitations", "Sender_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Invitations", "RecipientEmail", c => c.String());
            DropColumn("dbo.Invitations", "CreationDate");
            CreateIndex("dbo.Invitations", "Sender_Id");
            AddForeignKey("dbo.Invitations", "Sender_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
