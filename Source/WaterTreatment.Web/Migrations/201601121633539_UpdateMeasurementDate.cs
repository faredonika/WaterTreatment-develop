namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMeasurementDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Report", "MeasurementDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Report", "MeasurementDate", c => c.DateTime(nullable: false));
        }
    }
}
