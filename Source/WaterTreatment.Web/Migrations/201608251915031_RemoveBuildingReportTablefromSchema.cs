namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBuildingReportTablefromSchema : DbMigration
    {
        public override void Up()
        {
            //run custom sql - update to SystemMeasurement table
            Sql("ALTER TABLE [dbo].[SystemMeasurement] DROP CONSTRAINT [FK_dbo.SystemMeasurement_dbo.BuildingReport_Report_Id]");
            Sql("UPDATE [dbo].[SystemMeasurement] SET [dbo].[SystemMeasurement].[Report_Id] = dbo.BuildingReport.Report_Id FROM [dbo].[SystemMeasurement] INNER JOIN dbo.BuildingReport ON dbo.BuildingReport.Id = dbo.SystemMeasurement.Report_Id");
            //run rest of migration
            DropForeignKey("dbo.BuildingReport", "Building_Id", "dbo.Building");
            DropForeignKey("dbo.BuildingReport", "Report_Id", "dbo.Report");
            DropForeignKey("dbo.SystemMeasurement", "Report_Id", "dbo.BuildingReport");
            DropIndex("dbo.BuildingReport", new[] { "Building_Id" });
            DropIndex("dbo.BuildingReport", new[] { "Report_Id" });
            DropTable("dbo.BuildingReport");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BuildingReport",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Building_Id = c.Int(),
                        Report_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.BuildingReport", "Report_Id");
            CreateIndex("dbo.BuildingReport", "Building_Id");
            AddForeignKey("dbo.BuildingReport", "Report_Id", "dbo.Report", "Id");
            AddForeignKey("dbo.BuildingReport", "Building_Id", "dbo.Building", "Id");
        }
    }
}
