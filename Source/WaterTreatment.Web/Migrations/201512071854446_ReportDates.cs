namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Report", "MeasurementDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Report", "SubmissionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Report", "SubmissionDate");
            DropColumn("dbo.Report", "MeasurementDate");
        }
    }
}
