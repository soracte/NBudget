namespace NBudget.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddReportHeader : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        ReportDocumentId = c.String(),
                        Owner_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportHeaders", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ReportHeaders", new[] { "Owner_Id" });
            DropTable("dbo.ReportHeaders");
        }
    }
}
