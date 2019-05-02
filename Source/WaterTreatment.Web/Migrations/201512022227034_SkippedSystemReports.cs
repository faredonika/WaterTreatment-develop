namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkippedSystemReports : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemMeasurement", "Skipped", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemMeasurement", "Skipped");
        }
    }
}
