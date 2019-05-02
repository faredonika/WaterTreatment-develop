namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BuildingReport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuildingReport",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Buiding_Id = c.Int(),
                        Report_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Building", t => t.Buiding_Id)
                .ForeignKey("dbo.Report", t => t.Report_Id)
                .Index(t => t.Buiding_Id)
                .Index(t => t.Report_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BuildingReport", "Report_Id", "dbo.Report");
            DropForeignKey("dbo.BuildingReport", "Buiding_Id", "dbo.Building");
            DropIndex("dbo.BuildingReport", new[] { "Report_Id" });
            DropIndex("dbo.BuildingReport", new[] { "Buiding_Id" });
            DropTable("dbo.BuildingReport");
        }
    }
}
