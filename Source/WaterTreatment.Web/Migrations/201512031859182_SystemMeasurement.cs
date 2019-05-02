namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemMeasurement : DbMigration
    {
        public override void Up()
        {

            DropForeignKey("dbo.SystemMeasurement", "Report_Id", "dbo.Report");
            DropIndex("dbo.SystemMeasurement", new[] { "Report_Id" });
            CreateIndex("dbo.SystemMeasurement", "Report_Id");
            AddForeignKey("dbo.SystemMeasurement", "Report_Id", "dbo.BuildingReport", "Id");
        }
        
        public override void Down()
        {

            DropForeignKey("dbo.SystemMeasurement", "Report_Id", "dbo.BuildingReport");
            DropIndex("dbo.SystemMeasurement", new[] { "Report_Id" });
            CreateIndex("dbo.SystemMeasurement", "Report_Id");
            AddForeignKey("dbo.SystemMeasurement", "Report_Id", "dbo.Report", "Id");
        }
    }
}
