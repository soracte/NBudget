namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddInviter : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUsers", name: "ApplicationUser_Id", newName: "Inviter_Id");
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_ApplicationUser_Id", newName: "IX_Inviter_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_Inviter_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "Inviter_Id", newName: "ApplicationUser_Id");
        }
    }
}
