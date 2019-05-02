namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportSubscription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportSubscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Site_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.Site_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportSubscription", "User_Id", "dbo.User");
            DropForeignKey("dbo.ReportSubscription", "Site_Id", "dbo.Site");
            DropIndex("dbo.ReportSubscription", new[] { "User_Id" });
            DropIndex("dbo.ReportSubscription", new[] { "Site_Id" });
            DropTable("dbo.ReportSubscription");
        }
    }
}
